using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using Zapp.Config;
using Zapp.Fuse;
using Zapp.Core.Clauses;
using System.Net;

namespace Zapp.Schedule
{
    /// <summary>
    /// Represents a implementation of <see cref="IScheduleService"/> used for process orchestration.
    /// </summary>
    public class ScheduleService : IScheduleService, IDisposable
    {
        private bool isGlobalExtractionCompleted = false;

        private readonly ILog logService;
        private readonly IConfigStore configStore;
        private readonly IFusionService fusionService;

        private readonly IFusionProcessFactory fusionProcessFactory;

        private IDictionary<string, IFusionProcess> processes;

        private static readonly object syncLock = new object();

        /// <summary>
        /// Initializes a new <see cref="ScheduleService"/>.
        /// </summary>
        /// <param name="logService">Service used for logging.</param>
        /// <param name="configStore">Store used for loading configuration.</param>
        /// <param name="fusionService">Service used for resolving fusion extractions.</param>
        /// <param name="fusionProcessFactory">Factory used for creating <see cref="IFusionProcess"/> instances.</param>
        public ScheduleService(
            ILog logService,
            IConfigStore configStore,
            IFusionService fusionService,
            IFusionProcessFactory fusionProcessFactory)
        {
            this.logService = logService;
            this.configStore = configStore;
            this.fusionService = fusionService;

            this.fusionProcessFactory = fusionProcessFactory;

            processes = new SortedDictionary<string, IFusionProcess>(
                StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Schedules a collection of fusions
        /// </summary>
        /// <param name="fusionIds">Identities of the fusions.</param>
        /// <exception cref="ArgumentNullException">Throw when <paramref name="fusionIds"/> is not set.</exception>
        /// <inheritdoc />
        public void Schedule(IReadOnlyCollection<string> fusionIds)
        {
            Guard.ParamNotNull(fusionIds, nameof(fusionIds));

            foreach (string fusionId in fusionIds)
            {
                Guard.ParamNotNullOrEmpty(fusionId, nameof(fusionId));
            }

            lock (syncLock)
            {
                if (!isGlobalExtractionCompleted)
                {
                    logService.Info("Global extraction not completed, finishing that first.");

                    if ((isGlobalExtractionCompleted = fusionService.TryExtract()))
                    {
                        logService.Info("Global extraction completed, scheduling all fusions.");

                        ScheduleAll();
                    }
                    else
                    {
                        logService.Fatal("Global extraction failed, waiting for next announcement.");
                    }
                }
                else
                {
                    WaitForDrainPermission();

                    foreach (string fusionId in fusionIds)
                    {
                        Schedule(fusionId);
                    }
                }
            }
        }

        /// <summary>
        /// Schedules all the configured fusions.
        /// </summary>
        /// <inheritdoc />
        public void ScheduleAll()
        {
            var fusionIds = configStore.Value?.Fuse?.Fusions?
                .Select(f => f.Id)?
                .ToList() ?? new List<string>();

            Schedule(fusionIds);
        }

        /// <summary>
        /// Announces a running app on which rest port it's bound.
        /// </summary>
        /// <param name="fusionId">Identity of the fusion.</param>
        /// <param name="port">Port of the fusions' rest service.</param>
        /// <inheritdoc />
        public bool TryAnnounce(string fusionId, int port)
        {
            Guard.ParamNotNullOrEmpty(fusionId, nameof(fusionId));
            Guard.ParamNotOutOfRange(port, IPEndPoint.MinPort, IPEndPoint.MaxPort, nameof(port));

            var process = default(IFusionProcess);

            if (!TryGetProcess(fusionId, out process))
            {
                return false;
            }

            return process.TryRequestStart(port);
        }

        private void WaitForDrainPermission() { }

        private void Schedule(string fusionId)
        {
            var currentProcess = default(IFusionProcess);

            if (TryGetProcess(fusionId, out currentProcess))
            {
                if (currentProcess.TryRequestStop() &&
                    processes.Remove(fusionId))
                {
                    (currentProcess as IDisposable).Dispose();
                    currentProcess = null;
                }
            }

            currentProcess = fusionProcessFactory.CreateNew(fusionId);

            if (currentProcess.TrySpawn())
            {
                processes.Add(fusionId, currentProcess);
            }
        }

        private bool TryGetProcess(string fusionId, out IFusionProcess process) =>
            processes.TryGetValue(fusionId, out process);

        /// <summary>
        /// Releases all resources used by the <see cref="ScheduleService"/> instance.
        /// </summary>
        public void Dispose()
        {
            // processes: drain them!

            processes?.Clear();
            processes = null;
        }
    }
}
