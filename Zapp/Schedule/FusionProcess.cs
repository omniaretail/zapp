using EnsureThat;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zapp.Catalogue;
using Zapp.Config;
using Zapp.Core;
using Zapp.Core.Extensions;
using Zapp.Exceptions;
using Zapp.Fuse;
using Zapp.Hospital;
using WinProcess = System.Diagnostics.Process;

namespace Zapp.Schedule
{
    /// <summary>
    /// Represents an implementation of <see cref="IFusionProcess"/> for windows processes.
    /// </summary>
    public sealed class FusionProcess : IFusionProcess, IDisposable
    {
        private const int spawnThreshold = 10;
        private static readonly TimeSpan defaultProcessTimeout = TimeSpan.FromSeconds(20);

        private const string startupAction = "api/lifetime/startup";
        private const string teardownAction = "api/lifetime/teardown";
        private const string nurseStatusAction = "api/nurse/status";

        private readonly ILog logService;
        private readonly IConfigStore configStore;
        private readonly IFusionCatalogue fusionCatalogue;

        private readonly IHttpFailurePolicy httpFailurePolicy;

        private WinProcess process;
        private IDictionary<string, string> metaInfo;

        private int? restApiPort;
        private int nrOfSpawns = 0;

        private bool isAutoRestartEnabled = true;

        /// <summary>
        /// Represents the identity of the fusion.
        /// </summary>
        /// <inheritdoc />
        public string FusionId { get; }

        /// <summary>
        /// Represents the state of the fusion.
        /// </summary>
        /// <inheritdoc />
        public FusionProcessState State { get; private set; }

        /// <summary>
        /// Represents the timestamp when the process started.
        /// </summary>
        /// <inheritdoc />
        public DateTime? StartedAt { get; private set; }

        /// <summary>
        /// Represents a custom session implemention.
        /// </summary>
        /// <inheritdoc />
        public object Session { get; set; }

        /// <summary>
        /// Peformance counter for cpu.
        /// </summary>
        /// <inheritdoc />
        public PerformanceCounter CpuCounter { get; private set; }

        /// <summary>
        /// Peformance counter for memory.
        /// </summary>
        /// <inheritdoc />
        public PerformanceCounter MemoryCounter { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="FusionProcess"/>.
        /// </summary>
        /// <param name="fusionId">Identity of the fusion.</param>
        /// <param name="logService">Service used for logging.</param>
        /// <param name="configStore">Store used for loading configuration.</param>
        /// <param name="fusionCatalogue">Catalogue used to resolve locations of fusions.</param>
        /// <param name="httpFailurePolicy">Failure policy used for http request(s).</param>
        public FusionProcess(
            string fusionId,
            ILog logService,
            IConfigStore configStore,
            IFusionCatalogue fusionCatalogue,
            IHttpFailurePolicy httpFailurePolicy)
        {
            EnsureArg.IsNotNullOrEmpty(fusionId, nameof(fusionId));

            FusionId = fusionId;

            this.logService = logService;
            this.configStore = configStore;
            this.fusionCatalogue = fusionCatalogue;

            this.httpFailurePolicy = httpFailurePolicy;

            process = new WinProcess();

            CpuCounter = new PerformanceCounter("Process", "% Processor Time");
            MemoryCounter = new PerformanceCounter("Process", "Working Set - Private");

            ChangeState(FusionProcessState.None);
        }

        /// <summary>
        /// Tries to spawn an instance of the process.
        /// </summary>
        /// <inheritdoc />
        public void Spawn()
        {
            if (State == FusionProcessState.None)
            {
                SetupProcess(process);
            }

            if (process?.Start() != true)
            {
                throw new ScheduleException(ScheduleException.SpawnFailure, FusionId);
            }

            CpuCounter.InstanceName = process.ProcessName;
            MemoryCounter.InstanceName = process.ProcessName;

            ChangeState(FusionProcessState.Spawned);

            nrOfSpawns++;
        }

        /// <summary>
        /// Announces the port of the process.
        /// </summary>
        /// <param name="port">Port that was received from the process.</param>
        /// <inheritdoc />
        public void Announce(int port)
        {
            restApiPort = port;

            logService.Info($"Fusion: '{FusionId}' it's rest port ({port}) has been received successfully.");

            ChangeState(FusionProcessState.Announced);

            // if scheduler is not deploying start for your own!
        }

        /// <summary>
        /// Runs the startup event on the process.
        /// </summary>
        /// <param name="token">Token used to cancel the http request.</param>
        /// <inheritdoc />
        public async Task StartupAsync(CancellationToken token)
        {
            using (var client = new HttpClient().AsLocalhost(restApiPort))
            {
                await client.GetWithFailurePolicyAsync(startupAction, httpFailurePolicy, token);

                logService.Info($"Fusion: '{FusionId}' it's statup event has been called with success.");
            }
        }

        /// <summary>
        /// Runs the terminate event on the process.
        /// </summary>
        /// <param name="token">Token used to cancel the http request.</param>
        /// <inheritdoc />
        public async Task TerminateAsync(CancellationToken token)
        {
            isAutoRestartEnabled = false;

            if (State != FusionProcessState.Started)
            {
                logService.Warn($"Fusion: '{FusionId}' was not requested to terminate, because it's state was not passed '{nameof(FusionProcessState.Started)}'.");
                return;
            }

            using (var client = new HttpClient().AsLocalhost(restApiPort))
            {
                await client.GetWithFailurePolicyAsync(teardownAction, httpFailurePolicy, token);

                logService.Info($"Fusion: '{FusionId}' it's terminate event has been called with success.");
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<PatientStatus>> NurseStatusAsync(string patientPattern, CancellationToken token)
        {
            EnsureArg.IsNotNullOrEmpty(patientPattern, nameof(patientPattern));

            using (var client = new HttpClient().AsLocalhost(restApiPort))
            {
                var actualAction = $"{nurseStatusAction}?{nameof(patientPattern)}={patientPattern}";

                return await client.GetJsonAsync<IEnumerable<PatientStatus>>(actualAction, token);
            }
        }

        /// <summary>
        /// Gets the error output for a dead or exited process
        /// </summary>
        /// <inheritDoc />
        public string GetErrorOutput()
        {
            try
            {
                var processRootDirectory = fusionCatalogue
                    .GetActiveLocation(FusionId);

                var crashDumpFile = Path.Combine(
                    processRootDirectory, 
                    ZappVariables.CrashDumpFileName
                );

                if (!File.Exists(crashDumpFile))
                {
                    return "No error output has been received.";
                }

                return File.ReadAllText(crashDumpFile);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// Called when the interceptors are informed.
        /// </summary>
        /// <inheritdoc />
        public void OnInterceptorsInformed()
        {
            nrOfSpawns = 1;

            ChangeState(FusionProcessState.Started);
        }

        /// <summary>
        /// Called when a respawn has been failed.
        /// </summary>
        /// <inheritdoc />
        public void OnRespawnStartupFailed()
        {
            logService.Warn($"Fusion: '{FusionId}' it's startup failed, fast-forwarded respawn threshold to ({spawnThreshold}).");

            nrOfSpawns = spawnThreshold;

            OnExited();
        }

        private void OnExited()
        {
            ChangeState(FusionProcessState.Exited);

            var spawnThresholdReached = nrOfSpawns >= spawnThreshold;

            if (!isAutoRestartEnabled || spawnThresholdReached)
            {
                ChangeState(FusionProcessState.Dead);

                if (spawnThresholdReached)
                {
                    var builder = new StringBuilder();
                    builder.AppendLine($"Fusion: '{FusionId}' it's spawn threshold ({spawnThreshold}) has been reached. Cause: ");
                    builder.AppendLine(GetErrorOutput());

                    logService.Fatal(builder.ToString());
                }
            }
            else
            {
                Spawn();
            }
        }

        private void SetupProcess(WinProcess setupProcess)
        {
            var processRootDirectory = fusionCatalogue
                .GetActiveLocation(FusionId);

            metaInfo = LoadMetaInfo(processRootDirectory);

            var executableName = Path.Combine(processRootDirectory, metaInfo[FusionMetaEntry.ExecutableInfoKey]);

            setupProcess.StartInfo.Verb = "runas";
            setupProcess.StartInfo.FileName = executableName;
            setupProcess.StartInfo.RedirectStandardOutput = false;
            setupProcess.StartInfo.RedirectStandardError = false;

            var parentId = WinProcess.GetCurrentProcess().Id;
            var parentPort = configStore.Value?.Rest?.Port;

            setupProcess.StartInfo.EnvironmentVariables.Add(ZappVariables.FusionIdEnvKey, FusionId);
            setupProcess.StartInfo.EnvironmentVariables.Add(ZappVariables.ParentProcessIdEnvKey, Convert.ToString(parentId));
            setupProcess.StartInfo.EnvironmentVariables.Add(ZappVariables.ParentPortEnvKey, Convert.ToString(parentPort));

            setupProcess.StartInfo.UseShellExecute = false;
            setupProcess.StartInfo.CreateNoWindow = true;

            setupProcess.EnableRaisingEvents = true;
            setupProcess.Exited += (s, e) => OnExited();
        }

        private IDictionary<string, string> LoadMetaInfo(string fusionDir)
        {
            var metaContent = File.ReadAllText(Path.Combine(fusionDir, ZappVariables.FusionMetaEntyName));

            return JsonConvert.DeserializeObject<Dictionary<string, string>>(metaContent);
        }

        private bool ChangeState(FusionProcessState state)
        {
            if (State == state)
            {
                return false;
            }

            State = state;

            if (state == FusionProcessState.Started)
            {
                StartedAt = DateTime.Now;
            }
            else
            {
                StartedAt = null;
            }

            logService.Info($"Fusion: '{FusionId}' it's state changed to: '{State.ToString()}'.");
            return true;
        }

        /// <summary>
        /// Releases all used resouces by the <see cref="FusionProcess"/> instance.
        /// </summary>
        public void Dispose()
        {
            isAutoRestartEnabled = false;

            if (process?.HasExited == false)
            {
                try
                {
                    process?.Kill();
                }
                catch (Exception ex) when (ex is Win32Exception || ex is InvalidOperationException) { }

                try
                {
                    process?.WaitForExit((int)defaultProcessTimeout.TotalMilliseconds);
                }
                catch (Exception ex) when (ex is Win32Exception || ex is SystemException) { }
            }

            CpuCounter?.Dispose();
            CpuCounter = null;

            MemoryCounter?.Dispose();
            MemoryCounter = null;

            process?.Dispose();
            process = null;
        }
    }
}
