using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Zapp.Config;
using Zapp.Core;
using Zapp.Core.Clauses;
using Zapp.Fuse;
using WinProcess = System.Diagnostics.Process;

namespace Zapp.Schedule
{
    /// <summary>
    /// Represents an implementation of <see cref="IFusionProcess"/> for windows processes.
    /// </summary>
    public class FusionProcess : IFusionProcess, IDisposable
    {
        private readonly string fusionId;

        private readonly ILog logService;
        private readonly IConfigStore configStore;

        private HttpClient client;
        private WinProcess process;
        private FusionProcessState state;

        /// <summary>
        /// Represents the state of the process.
        /// </summary>
        /// <inheritdoc />
        public FusionProcessState State => state;

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

            client = new HttpClient();
            process = new WinProcess();
            state = FusionProcessState.Instantiated;
        }

        /// <summary>
        /// Tries to spawn an instance of the process.
        /// </summary>
        /// <inheritdoc />
        public bool TrySpawn()
        {
            if (state != FusionProcessState.Instantiated) return false;

            var fusionDir = configStore.Value?.Fuse?
                .GetActualFusionDirectory(fusionId);

            // todo: move this logic to wrapper class -> read / write.
            var fusionMetaFile = Path.Combine(fusionDir, ZappVariables.FusionMetaEntyName);
            var fusionMetaFileContent = File.ReadAllText(fusionMetaFile);

            var fusionMetaInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(fusionMetaFileContent);

            var fusionExecutableFile = Path.Combine(fusionDir, fusionMetaInfo[FusionMetaEntry.ExecutableInfoKey]);

            process.StartInfo.Verb = "runas";
            process.StartInfo.FileName = fusionExecutableFile;
            process.StartInfo.RedirectStandardOutput = false;
            process.StartInfo.RedirectStandardError = false;

            var parentId = WinProcess.GetCurrentProcess().Id;
            var parentPort = configStore.Value?.Rest?.Port;

            process.StartInfo.EnvironmentVariables.Add(ZappVariables.ParentProcessIdEnvKey, Convert.ToString(parentId));
            process.StartInfo.EnvironmentVariables.Add(ZappVariables.ParentPortEnvKey, Convert.ToString(parentPort));

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.EnableRaisingEvents = true;
            process.Exited += (s, e) => logService.Warn($"Process: {fusionId} exited.");

            state = FusionProcessState.Spawned;
            logService.Info($"Process: {fusionId} spawned.");

            return process.Start();
        }

        /// <summary>
        /// Tries to request the process to start.
        /// </summary>
        /// <param name="port">Port where the process is bound onto.</param>
        /// <inheritdoc />
        public bool TryRequestStart(int port)
        {
            if (state != FusionProcessState.Spawned) return false;

            client.BaseAddress = new Uri($"http://localhost:{port}/");

            state = FusionProcessState.Running;
            logService.Info($"Process: {fusionId} started on port {port}.");
            return true;
        }

        /// <summary>
        /// Tries to request the process to stop.
        /// </summary>
        /// <inheritdoc />
        public bool TryRequestStop()
        {
            if (state != FusionProcessState.Running) return false;

            state = FusionProcessState.Stopping;
            logService.Warn($"Process: {fusionId} stopped.");
            return true;
        }

        /// <summary>
        /// Releases all used resouces by the <see cref="FusionProcess"/> instance.
        /// </summary>
        public void Dispose()
        {
            client?.Dispose();
            client = null;

            process?.Dispose();
            process = null;
        }
    }
}
