using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zapp.Config;
using Zapp.Core.Clauses;
using Zapp.Fuse;

namespace Zapp.Schedule
{
    /// <summary>
    /// Represents a implementation of <see cref="IScheduleService"/> used for process orchestration.
    /// </summary>
    public class ScheduleService : IScheduleService, IDisposable
    {
        private static readonly TimeSpan waitForStartTimeout = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan waitForStartInterval = TimeSpan.FromSeconds(1);

        private bool isGlobalExtractionCompleted = false;

        private readonly ILog logService;
        private readonly IConfigStore configStore;
        private readonly IFusionService fusionService;

        private readonly IFusionProcessFactory fusionProcessFactory;

        private readonly IEnumerable<IFusionProcessDrainer> drainers;
        private readonly IReadOnlyCollection<IFusionProcessInterceptor> interceptors;

        private IDictionary<string, IFusionProcess> processes;

        /// <summary>
        /// Represents the current running processes.
        /// </summary>
        /// <inheritdoc />
        public IReadOnlyCollection<IFusionProcess> Processes => processes?.Values?.ToList();

        private static readonly object syncLock = new object();

        /// <summary>
        /// Initializes a new <see cref="ScheduleService"/>.
        /// </summary>
        /// <param name="logService">Service used for logging.</param>
        /// <param name="configStore">Store used for loading configuration.</param>
        /// <param name="fusionService">Service used for resolving fusion extractions.</param>
        /// <param name="fusionProcessFactory">Factory used for creating <see cref="IFusionProcess"/> instances.</param>
        /// <param name="drainers">Collection of drainers that needs to determine if a <see cref="IFusionProcess"/> is ready for teardown.</param>
        /// <param name="interceptors">Collection of interceptors thats needs to be fired on certain events.</param>
        public ScheduleService(
            ILog logService,
            IConfigStore configStore,
            IFusionService fusionService,
            IFusionProcessFactory fusionProcessFactory,
            IEnumerable<IFusionProcessDrainer> drainers,
            IEnumerable<IFusionProcessInterceptor> interceptors)
        {
            this.logService = logService;
            this.configStore = configStore;
            this.fusionService = fusionService;

            this.fusionProcessFactory = fusionProcessFactory;

            this.drainers = drainers.ToList();
            this.interceptors = interceptors.ToList();

            processes = new SortedDictionary<string, IFusionProcess>(
                StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Schedules a collection of fusions
        /// </summary>
        /// <param name="fusionIds">Identities of the fusions.</param>
        /// <param name="isExtractionRequired">Indicates if extraction is required.</param>
        /// <exception cref="ArgumentNullException">Throw when <paramref name="fusionIds"/> is not set.</exception>
        /// <exception cref="ArgumentException">Throw when one of the <paramref name="fusionIds"/> is not set.</exception>
        /// <inheritdoc />
        public void Schedule(IReadOnlyCollection<string> fusionIds, bool isExtractionRequired = true)
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

                        ScheduleAll(isExtractionRequired: false);
                    }
                    else
                    {
                        logService.Fatal("Global extraction failed, waiting for next announcement.");
                    }
                }
                else
                {
                    DrainAndStop(fusionIds);

                    if (!isExtractionRequired || fusionService.TryExtractFusionBatch(fusionIds))
                    {
                        foreach (string fusionId in fusionIds)
                        {
                            Schedule(fusionId);
                        }

                        bool isFinished = WaitForProcessesToStartAsync()
                            .GetAwaiter()
                            .GetResult();

                        if (isFinished)
                        {
                            foreach (var drainer in drainers)
                            {
                                try
                                {
                                    drainer.Resume();

                                    logService.Info($"Drainer: resume {drainer.GetType().Name} finished.");
                                }
                                catch (Exception ex)
                                {
                                    logService.Error("Failed to run drainer for event Resume", ex);
                                }
                            }
                        }
                        else
                        {
                            logService.Fatal("Failed to spawn/start fusion batch.");
                        }
                    }
                    else
                    {
                        logService.Fatal("Failed to extraction fusion batch.");
                    }
                }
            }
        }

        /// <summary>
        /// Schedules all the configured fusions.
        /// </summary>
        /// <param name="isExtractionRequired">Indicates if extraction is required.</param>
        /// <inheritdoc />
        public void ScheduleAll(bool isExtractionRequired = true)
        {
            var fusionIds = configStore.Value?.Fuse?.Fusions?
                .OrderBy(f => f.Order)
                .Select(f => f.Id)?
                .ToList() ?? new List<string>();

            Schedule(fusionIds, isExtractionRequired);
        }

        /// <summary>
        /// Announces a running app on which rest port it's bound.
        /// </summary>
        /// <param name="fusionId">Identity of the fusion.</param>
        /// <param name="port">Port of the fusions' rest service.</param>
        /// <exception cref="ArgumentException">Throw when <paramref name="fusionId"/> is not set.</exception>
        /// <inheritdoc />
        public bool TryAnnounce(string fusionId, int port)
        {
            Guard.ParamNotNullOrEmpty(fusionId, nameof(fusionId));

            var process = default(IFusionProcess);

            if (!TryGetProcess(fusionId, out process))
            {
                return false;
            }

            if (!process.TryRequestStart(port))
            {
                logService.Error($"Failed to start process for fusion: {fusionId}, no interceptors informed.");
                return false;
            }

            foreach (var interceptor in interceptors)
            {
                try
                {
                    interceptor.OnStartupCalled(process);

                    logService.Info($"Interceptor: {interceptor.GetType().Name} finished.");
                }
                catch (Exception ex)
                {
                    logService.Error("Failed to run interceptor for event OnStartupCalled", ex);
                }
            }

            process.OnInterceptorsInformed();

            return true;
        }

        private void Schedule(string fusionId)
        {
            StopProcess(fusionId);
            StartProcess(fusionId);
        }

        private void StartProcess(string fusionId)
        {
            var process = fusionProcessFactory.CreateNew(fusionId);

            if (!process.TrySpawn())
            {
                logService.Error($"Failed to spawn process for fusion: {fusionId}, added anyway.");
            }

            processes.Add(fusionId, process);
        }

        private void StopProcess(string fusionId)
        {
            var process = default(IFusionProcess);

            if (!TryGetProcess(fusionId, out process))
            {
                return;
            }

            if (!process.TryRequestStop())
            {
                logService.Error($"Failed to stop process for fusion: {fusionId}, removed/disposed anyway.");
            }

            processes.Remove(fusionId);

            (process as IDisposable).Dispose();
        }

        private void DrainAndStop(IReadOnlyCollection<string> fusionIds)
        {
            var processes = GetProcesses(fusionIds).ToList();

            if (processes.Any())
            {
                foreach (var drainer in drainers)
                {
                    try
                    {
                        drainer.Drain(processes);

                        logService.Info($"Drainer: {drainer.GetType().Name} finished.");
                    }
                    catch (Exception ex)
                    {
                        logService.Error("Failed to run drainer.", ex);
                    }
                }
            }

            foreach (string fusionId in fusionIds)
            {
                StopProcess(fusionId);
            }
        }

        private void StopProcesses()
        {
            lock (syncLock)
            {
                var fusionIds = processes?.Keys?.ToList();

                DrainAndStop(fusionIds);
            }
        }

        private async Task<bool> WaitForProcessesToStartAsync()
        {
            var startedAt = DateTime.UtcNow;

            var isCompleted = false;

            while (!isCompleted &&
                (DateTime.UtcNow - startedAt) < waitForStartTimeout)
            {
                isCompleted = !processes.Values
                    .Where(p => p.StartedAt == null)
                    .Any();

                if (!isCompleted)
                {
                    await Task.Delay(waitForStartInterval);
                }
            }

            return isCompleted;
        }

        private bool TryGetProcess(string fusionId, out IFusionProcess process)
        {
            process = default(IFusionProcess);

            return processes?.TryGetValue(fusionId, out process) == true;
        }

        private IEnumerable<IFusionProcess> GetProcesses(IReadOnlyCollection<string> fusionIds)
        {
            var current = default(IFusionProcess);

            foreach (string fusionId in fusionIds)
            {
                if (TryGetProcess(fusionId, out current))
                {
                    yield return current;
                }
            }
        }

        /// <summary>
        /// Releases all resources used by the <see cref="ScheduleService"/> instance.
        /// </summary>
        public void Dispose()
        {
            try
            {
                StopProcesses();
            }
            catch (Exception ex)
            {
                logService?.Fatal("Failed to drain all processes", ex);
            }

            processes?.Clear();
            processes = null;
        }
    }
}
