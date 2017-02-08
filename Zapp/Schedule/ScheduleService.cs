using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using Zapp.Config;
using Zapp.Fuse;
using Zapp.Utils;

namespace Zapp.Schedule
{
    /// <summary>
    /// Represents a implementation of <see cref="IScheduleService"/> used for process orchestration.
    /// </summary>
    public class ScheduleService : IScheduleService
    {
        private bool isGlobalExtractionCompleted = false;

        private readonly ILog logService;
        private readonly IConfigStore configStore;
        private readonly IFusionService fusionService;

        private static readonly object syncLock = new object();

        /// <summary>
        /// Initializes a new <see cref="ScheduleService"/>.
        /// </summary>
        /// <param name="logService">Service used for logging.</param>
        /// <param name="configStore">Store used for loading configuration.</param>
        /// <param name="fusionService">Service used for resolving fusion extractions.</param>
        public ScheduleService(
            ILog logService,
            IConfigStore configStore,
            IFusionService fusionService)
        {
            this.logService = logService;
            this.configStore = configStore;
            this.fusionService = fusionService;
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
    }
}
