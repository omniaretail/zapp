using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using Zapp.Config;
using Zapp.Core;
using Zapp.Core.Clauses;
using Zapp.Core.Http;
using Zapp.Fuse;
using WinProcess = System.Diagnostics.Process;

namespace Zapp.Schedule
{
    /// <summary>
    /// Represents an implementation of <see cref="IFusionProcess"/> for windows processes.
    /// </summary>
    public class FusionProcess : IFusionProcess, IDisposable
    {
        private const int maxNrOfRespawns = 10;
        private static readonly TimeSpan defaultProcessTimeout = TimeSpan.FromSeconds(20);

        private const string startupAction = "api/lifetime/startup";
        private const string teardownAction = "api/lifetime/teardown";

        private readonly ILog logService;
        private readonly IConfigStore configStore;

        private WinProcess process;
        private IDictionary<string, string> metaInfo;

        private int? restApiPort;
        private int nrOfRespawns = 0;

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
        /// <param name="logService">Service used for logging.</param>
        /// <param name="configStore">Store used for loading configuration.</param>
        /// <param name="fusionId">Identity of the fusion.</param>
        public FusionProcess(
            string fusionId,
            ILog logService,
            IConfigStore configStore)
        {
            Guard.ParamNotNullOrEmpty(fusionId, nameof(fusionId));

            FusionId = fusionId;

            this.logService = logService;
            this.configStore = configStore;

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
                var fusionDir = configStore.Value?.Fuse?
                    .GetActualFusionDirectory(FusionId);

                metaInfo = LoadMetaInfo(fusionDir);

                var executableName = Path.Combine(fusionDir, metaInfo[FusionMetaEntry.ExecutableInfoKey]);

                process.StartInfo.Verb = "runas";
                process.StartInfo.FileName = executableName;
                process.StartInfo.RedirectStandardOutput = false;
                process.StartInfo.RedirectStandardError = false;

                var parentId = WinProcess.GetCurrentProcess().Id;
                var parentPort = configStore.Value?.Rest?.Port;

                process.StartInfo.EnvironmentVariables.Add(ZappVariables.FusionIdEnvKey, FusionId);
                process.StartInfo.EnvironmentVariables.Add(ZappVariables.ParentProcessIdEnvKey, Convert.ToString(parentId));
                process.StartInfo.EnvironmentVariables.Add(ZappVariables.ParentPortEnvKey, Convert.ToString(parentPort));

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.EnableRaisingEvents = true;
                process.Exited += (s, e) => OnExited();
            }

            if (process?.Start() != true)
            {
                throw new Exception();
            }

            CpuCounter.InstanceName = process.ProcessName;
            MemoryCounter.InstanceName = process.ProcessName;

            ChangeState(FusionProcessState.Spawned);
        }

        /// <summary>
        /// Announces the port of the process.
        /// </summary>
        /// <param name="port">Port that was received from the process.</param>
        /// <inheritdoc />
        public void Announce(int port)
        {
            restApiPort = port;

            ChangeState(FusionProcessState.Announced);
        }

        /// <summary>
        /// Runs the startup event on the process.
        /// </summary>
        /// <inheritdoc />
        public void Startup()
        {
            using (var client = new HttpClient().AsLocalhost(restApiPort))
            {
                var isStartExecuted = client.ExpectOk(startupAction); // todo: throw if not 200
            }
        }

        /// <summary>
        /// Runs the terminate event on the process.
        /// </summary>
        /// <inheritdoc />
        public void Terminate()
        {
            isAutoRestartEnabled = false;

            using (var client = new HttpClient().AsLocalhost(restApiPort))
            {
                var isTeardownExecuted = client.ExpectOk(teardownAction); // todo: throw if not 200
            }
        }

        /// <summary>
        /// Called when the interceptors are informed.
        /// </summary>
        /// <inheritdoc />
        public void OnInterceptorsInformed() => ChangeState(FusionProcessState.Started);

        private void OnExited()
        {
            ChangeState(FusionProcessState.Exited);

            var spawnThresholdReached = nrOfRespawns >= maxNrOfRespawns;

            if (!isAutoRestartEnabled || spawnThresholdReached)
            {
                ChangeState(FusionProcessState.Dead);
                return;
            }

            nrOfRespawns++;
            Spawn();
        }

        private IDictionary<string, string> LoadMetaInfo(string fusionDir)
        {
            var metaContent = File.ReadAllText(Path.Combine(fusionDir, ZappVariables.FusionMetaEntyName));

            return JsonConvert.DeserializeObject<Dictionary<string, string>>(metaContent);
        }

        private void ChangeState(FusionProcessState state)
        {
            State = state;

            if (state == FusionProcessState.Started)
            {
                StartedAt = DateTime.Now;
            }
            else
            {
                StartedAt = null;
            }
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
