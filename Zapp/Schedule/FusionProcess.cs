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
        private const int maxNrOfRespawns = 3;
        private static readonly TimeSpan defaultProcessTimeout = TimeSpan.FromSeconds(20);

        private const string startupAction = "api/lifetime/startup";
        private const string teardownAction = "api/lifetime/teardown";

        private readonly string fusionId;

        private readonly ILog logService;
        private readonly IConfigStore configStore;

        private WinProcess process;
        private IDictionary<string, string> metaInfo;

        private int? processPort;

        private PerformanceCounter cpuCounter;
        private PerformanceCounter memoryCounter;

        private DateTime? startedAt;

        private int nrOfRespawns = 0;

        private bool isAutoRestartEnabled = true;

        /// <summary>
        /// Represents the identity of the fusion.
        /// </summary>
        /// <inheritdoc />
        public string FusionId => fusionId;

        /// <summary>
        /// Represents the timestamp when the process started.
        /// </summary>
        /// <inheritdoc />
        public DateTime? StartedAt => startedAt;

        /// <summary>
        /// Represents a custom session implemention.
        /// </summary>
        /// <inheritdoc />
        public object Session { get; set; }

        /// <summary>
        /// Peformance counter for cpu.
        /// </summary>
        /// <inheritdoc />
        public PerformanceCounter CpuCounter => cpuCounter;

        /// <summary>
        /// Peformance counter for memory.
        /// </summary>
        /// <inheritdoc />
        public PerformanceCounter MemoryCounter => memoryCounter;

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

            this.fusionId = fusionId;
            this.logService = logService;
            this.configStore = configStore;

            process = new WinProcess();
            cpuCounter = new PerformanceCounter("Process", "% Processor Time");
            memoryCounter = new PerformanceCounter("Process", "Working Set - Private");
        }

        /// <summary>
        /// Tries to spawn an instance of the process.
        /// </summary>
        /// <inheritdoc />
        public bool TrySpawn()
        {
            if (string.IsNullOrEmpty(process?.StartInfo?.FileName))
            {
                var fusionDir = configStore.Value?.Fuse?
                    .GetActualFusionDirectory(fusionId);

                metaInfo = LoadMetaInfo(fusionDir);

                var executableName = Path.Combine(fusionDir, metaInfo[FusionMetaEntry.ExecutableInfoKey]);

                process.StartInfo.Verb = "runas";
                process.StartInfo.FileName = executableName;
                process.StartInfo.RedirectStandardOutput = false;
                process.StartInfo.RedirectStandardError = false;

                var parentId = WinProcess.GetCurrentProcess().Id;
                var parentPort = configStore.Value?.Rest?.Port;

                process.StartInfo.EnvironmentVariables.Add(ZappVariables.FusionIdEnvKey, fusionId);
                process.StartInfo.EnvironmentVariables.Add(ZappVariables.ParentProcessIdEnvKey, Convert.ToString(parentId));
                process.StartInfo.EnvironmentVariables.Add(ZappVariables.ParentPortEnvKey, Convert.ToString(parentPort));

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.EnableRaisingEvents = true;
                process.Exited += (s, e) => OnExited();
            }

            bool isSpawned = process?.Start() == true;

            LogEvent("spawn", isSuccess: isSpawned);

            if (isSpawned)
            {
                cpuCounter.InstanceName = process.ProcessName;
                memoryCounter.InstanceName = process.ProcessName;
            }

            return isSpawned;
        }

        /// <summary>
        /// Tries to request the process to start.
        /// </summary>
        /// <param name="port">Port where the process is bound onto.</param>
        /// <inheritdoc />
        public bool TryRequestStart(int port)
        {
            processPort = port;

            using (var client = new HttpClient().AsLocalhost(processPort))
            {
                var isStartExecuted = client.ExpectOk(startupAction);

                LogEvent("startup", message: $"Port: {processPort}", isSuccess: isStartExecuted);

                if (isStartExecuted)
                {
                    startedAt = DateTime.Now;
                }
                else
                {
                    startedAt = null;
                }

                return isStartExecuted;
            }
        }

        /// <summary>
        /// Tries to request the process to stop.
        /// </summary>
        /// <inheritdoc />
        public bool TryRequestStop()
        {
            isAutoRestartEnabled = false;

            // todo: de-duplicate this code
            using (var client = new HttpClient().AsLocalhost(processPort))
            {
                var isTeardownExecuted = client.ExpectOk(teardownAction);

                LogEvent("teardown", isSuccess: isTeardownExecuted);

                return isTeardownExecuted;
            }
        }

        private void OnExited()
        {
            LogEvent("exited", isSuccess: !isAutoRestartEnabled);

            if (isAutoRestartEnabled)
            {
                bool isRespawned = TrySpawn();

                int pos = ++nrOfRespawns;

                if (pos >= maxNrOfRespawns)
                {
                    isAutoRestartEnabled = false;

                    LogEvent("spawn-restart", message: "reached respawn threshold", isSuccess: isRespawned);
                }
                else
                {
                    LogEvent("spawn-restart", isSuccess: isRespawned);
                }
            }
        }

        private IDictionary<string, string> LoadMetaInfo(string fusionDir)
        {
            var metaContent = File.ReadAllText(Path.Combine(fusionDir, ZappVariables.FusionMetaEntyName));

            return JsonConvert.DeserializeObject<Dictionary<string, string>>(metaContent);
        }

        private void LogEvent(string eventName, string message = null, bool? isSuccess = false)
        {
            var text = $"Process: {fusionId} Event: {eventName} Message: {message ?? "not provided"}";

            if (isSuccess.HasValue)
            {
                if (isSuccess.Value == true)
                {
                    logService.Info(text);
                }
                else
                {
                    logService.Error(text);
                }
            }
            else
            {
                logService.Info(text);
            }
        }

        /// <summary>
        /// Releases all used resouces by the <see cref="FusionProcess"/> instance.
        /// </summary>
        public void Dispose()
        {
            startedAt = null;
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

            cpuCounter?.Dispose();
            cpuCounter = null;

            memoryCounter?.Dispose();
            memoryCounter = null;

            process?.Dispose();
            process = null;
        }
    }
}
