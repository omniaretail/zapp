using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zapp.Config;
using Zapp.Core.Clauses;
using Zapp.Extensions;
using Zapp.Fuse;

namespace Zapp.Schedule
{
    /// <summary>
    /// Represents a implementation of <see cref="IScheduleService"/> used for process orchestration.
    /// </summary>
    public class ScheduleService : IScheduleService, IDisposable
    {
        private static readonly TimeSpan waitForStartTimeout = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan waitForAnnouncementInterval = TimeSpan.FromSeconds(1);

        private readonly ILog logService;
        private readonly IConfigStore configStore;
        private readonly IFusionService fusionService;

        private readonly IFusionProcessFactory fusionProcessFactory;

        private readonly IEnumerable<IFusionProcessDrainer> drainers;
        private readonly IReadOnlyCollection<IFusionProcessInterceptor> interceptors;

        private IDictionary<string, IFusionProcess> processes;

        private object syncLock;

        /// <summary>
        /// Represents the current running processes.
        /// </summary>
        /// <inheritdoc />
        public IReadOnlyCollection<IFusionProcess> Processes => processes?.Values?.ToList();

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

            syncLock = new object();

            processes = new SortedDictionary<string, IFusionProcess>(
                StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Schedules all the configured fusions.
        /// </summary>
        /// <inheritdoc />
        public void ScheduleAll()
        {
            var fusionIds = configStore.Value?.Fuse?.Fusions?
                .OrderBy(f => f.Order)
                .Select(f => f.Id)?
                .ToList() ?? new List<string>();

            ScheduleMultiple(fusionIds);
        }

        /// <summary>
        /// Schedules a collection of fusions
        /// </summary>
        /// <param name="fusionIds">Identities of the fusions.</param>
        /// <exception cref="ArgumentNullException">Throw when <paramref name="fusionIds"/> is not set.</exception>
        /// <inheritdoc />
        public void ScheduleMultiple(IEnumerable<string> fusionIds)
        {
            Guard.ParamNotNull(fusionIds, nameof(fusionIds));

            var exhausedFusionIds = fusionIds.Exhaust();

            lock (syncLock)
            {
                var activeProcesses = GetActiveProcesses(exhausedFusionIds).Exhaust();

                TerminateMultiple(activeProcesses); // drains and stops the services

                fusionService.ExtractMultiple(exhausedFusionIds); // extracts the new fusions

                var newProcesses = SpawnMultiple(exhausedFusionIds).Exhaust(); // spawns the services

                WaitForAnnouncements(newProcesses).GetAwaiter().GetResult(); // wait for all port callbacks

                StartupMultiple(newProcesses); // calls all the startups

                HealthCheckMultiple(newProcesses); // asks for all the nurse statusses

                ResumeAll(); // execute resume of drainers
            }
        }

        /// <summary>
        /// Announces a running app on which rest port it's bound.
        /// </summary>
        /// <param name="fusionId">Identity of the fusion.</param>
        /// <param name="port">Port of the fusions' rest service.</param>
        /// <exception cref="ArgumentException">Throw when <paramref name="fusionId"/> is not set.</exception>
        /// <inheritdoc />
        public void Announce(string fusionId, int port)
        {
            Guard.ParamNotNullOrEmpty(fusionId, nameof(fusionId));

            var process = default(IFusionProcess);

            if (!TryGetActiveProcess(fusionId, out process))
            {
                throw new Exception();
            }

            process.Announce(port);
        }

        private bool TryGetActiveProcess(string fusionId, out IFusionProcess process)
        {
            process = default(IFusionProcess);

            return processes?.TryGetValue(fusionId, out process) == true;
        }

        private IEnumerable<IFusionProcess> GetActiveProcesses(IEnumerable<string> fusionIds)
        {
            var current = default(IFusionProcess);
            var exhausedFusionIds = fusionIds.Exhaust();

            foreach (string fusionId in exhausedFusionIds)
            {
                if (TryGetActiveProcess(fusionId, out current))
                {
                    yield return current;
                }
            }
        }

        private void StartupMultiple(params IFusionProcess[] processes)
        {
            foreach (var process in processes)
            {
                Startup(process);
            }
        }

        private void Startup(IFusionProcess process)
        {
            process.Startup();

            foreach (var interceptor in interceptors)
            {
                interceptor.OnStartupCalled(process);
            }

            process.OnInterceptorsInformed();
        }

        private IEnumerable<IFusionProcess> SpawnMultiple(params string[] fusionIds)
        {
            foreach (var fusionId in fusionIds)
            {
                yield return Spawn(fusionId);
            }
        }

        private IFusionProcess Spawn(string fusionId)
        {
            var process = fusionProcessFactory.CreateNew(fusionId);

            process.Spawn();

            processes.Add(fusionId, process);

            return process;
        }

        private async Task WaitForAnnouncements(params IFusionProcess[] processes)
        {
            do
            {
                var hasDeadProcesses = processes
                    .Any(_ => _.State == FusionProcessState.Dead);

                if (hasDeadProcesses)
                {
                    throw new  Exception();
                }

                await Task.Delay(waitForAnnouncementInterval);
            }
            while (!processes.All(_ => _.State == FusionProcessState.Announced));
        }

        private void HealthCheckMultiple(params IFusionProcess[] processes)
        {
            foreach(var process in processes)
            {
                HealthCheck(process);
            }
        }

        private void HealthCheck(IFusionProcess process)
        {
            // todo: create!
        }

        private void TerminateAll()
        {
            lock (syncLock)
            {
                var activeProcesses = processes?.Values?.ToArray() ?? new IFusionProcess[0];

                TerminateMultiple(activeProcesses);
            }
        }

        private void TerminateMultiple(params IFusionProcess[] processes)
        {
            Drain(processes);

            foreach (var process in processes)
            {
                Terminate(process);
            }
        }

        private void Terminate(IFusionProcess process)
        {
            try
            {
                process.Terminate();

                (process as IDisposable).Dispose();
            }
            finally
            {
                processes?.Remove(process.FusionId);
            }
        }

        private void Drain(params IFusionProcess[] processes)
        {
            if (!processes.Any())
            {
                return;
            }

            foreach (var drainer in drainers)
            {
                drainer.Drain(processes);
            }
        }

        private void ResumeAll()
        {
            foreach (var drainer in drainers)
            {
                drainer.Resume();
            }
        }

        /// <summary>
        /// Releases all resources used by the <see cref="ScheduleService"/> instance.
        /// </summary>
        public void Dispose()
        {
            try
            {
                TerminateAll();
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
