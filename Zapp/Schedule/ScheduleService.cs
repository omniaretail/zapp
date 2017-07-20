using EnsureThat;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zapp.Config;
using Zapp.Deploy;
using Zapp.Exceptions;
using Zapp.Extensions;
using Zapp.Fuse;
using Zapp.Hospital;
using Zapp.Pack;

namespace Zapp.Schedule
{
    /// <summary>
    /// Represents a implementation of <see cref="IScheduleService"/> used for process orchestration.
    /// </summary>
    public sealed class ScheduleService : IScheduleService, IDisposable
    {
        private const int nrOfSimultaneousOperations = 1;

        private static readonly TimeSpan waitForAnnouncementTimeout = TimeSpan.FromMinutes(2);
        private static readonly TimeSpan waitForAnnouncementInterval = TimeSpan.FromSeconds(1);

        private readonly ILog logService;
        private readonly IConfigStore configStore;
        private readonly IFusionService fusionService;

        private readonly IFusionProcessFactory fusionProcessFactory;
        private readonly IDeployAnnouncementFactory announcementFactory;

        private readonly IReadOnlyCollection<IFusionProcessDrainer> drainers;
        private readonly IReadOnlyCollection<IFusionProcessInterceptor> interceptors;

        private IDictionary<string, IFusionProcess> processes;

        private SemaphoreSlim syncLock;
        private bool isFullCompleted;

        /// <summary>
        /// Represents the current running processes.
        /// </summary>
        /// <inheritdoc />
        public IEnumerable<IFusionProcess> Processes => processes?.Values;

        /// <summary>
        /// Initializes a new <see cref="ScheduleService"/>.
        /// </summary>
        /// <param name="logService">Service used for logging.</param>
        /// <param name="configStore">Store used for loading configuration.</param>
        /// <param name="fusionService">Service used for resolving fusion extractions.</param>
        /// <param name="fusionProcessFactory">Factory used for creating <see cref="IFusionProcess"/> instances.</param>
        /// <param name="announcementFactory">Factory that creates <see cref="IDeployAnnouncement"/> instances.</param>
        /// <param name="drainers">Collection of drainers that needs to determine if a <see cref="IFusionProcess"/> is ready for teardown.</param>
        /// <param name="interceptors">Collection of interceptors thats needs to be fired on certain events.</param>
        public ScheduleService(
            ILog logService,
            IConfigStore configStore,
            IFusionService fusionService,
            IFusionProcessFactory fusionProcessFactory,
            IDeployAnnouncementFactory announcementFactory,
            IEnumerable<IFusionProcessDrainer> drainers,
            IEnumerable<IFusionProcessInterceptor> interceptors)
        {
            this.logService = logService;
            this.configStore = configStore;
            this.fusionService = fusionService;

            this.fusionProcessFactory = fusionProcessFactory;
            this.announcementFactory = announcementFactory;

            this.drainers = drainers.StaleReadOnly();
            this.interceptors = interceptors.StaleReadOnly();

            syncLock = new SemaphoreSlim(nrOfSimultaneousOperations);

            processes = new SortedDictionary<string, IFusionProcess>(
                StringComparer.OrdinalIgnoreCase);
        }
        
        /// <summary>
        /// Represents if the service is deploying currently.
        /// </summary>
        /// <inheritDoc />
        public bool IsDeploying() => syncLock?.CurrentCount < nrOfSimultaneousOperations;

        /// <summary>
        /// Schedules all the configured fusions.
        /// </summary>
        /// <param name="token">The token of cancellation.</param>
        /// <inheritdoc />
        public async Task ScheduleAllAsync(CancellationToken token)
        {
            var fusionIds = configStore.Value?.Fuse?.Fusions?
                .Select(_ => _.Id) ?? new string[0];

            var announcement = announcementFactory
                .CreateNew(fusionIds, new PackageVersion[0]);

            try
            {
                await ScheduleAsync(announcement, token);
            }
            catch (Exception ex) when (
                !isFullCompleted &&
                configStore?.Value?.Fuse?.IgnoreInitialScheduleFailure == true
            )
            {
                logService.Warn("Initial schedule failed.", ex);
            }
        }

        /// <summary>
        /// Schedules an <see cref="IDeployAnnouncement"/>.
        /// </summary>
        /// <param name="announcement">The announcement that needs to be scheduled.</param>
        /// <param name="token">The token of cancellation.</param>
        /// <inheritdoc />
        public async Task ScheduleAsync(IDeployAnnouncement announcement, CancellationToken token)
        {
            EnsureArg.IsNotNull(announcement, nameof(announcement));

            var fusionIds = announcement
                .GetFusionIds()
                .OrderBy(_ => GetOrderId(_))
                .Stale();

            if (!isFullCompleted &&
                announcement.IsDelta())
            {
                throw new ScheduleException(ScheduleException.FullAnnouncementRequired, "*");
            }

            await syncLock.WaitAsync();

            try
            {
                var activeProcesses = GetActiveProcesses(fusionIds);

                await TerminateMultipleAsync(activeProcesses, token); // drains and stops the services

                logService.Info("Terminated all active fusion(s).");

                fusionService.Extract(announcement, token); // extracts the new fusions

                logService.Info("Extracted all new fusion(s).");

                var newProcesses = SpawnMultiple(fusionIds, token).Stale(); // spawns the services

                logService.Info("Spawned all new fusion(s).");

                await WaitForAnnouncements(newProcesses, token); // wait for all port callbacks

                logService.Info("Received all announcements from the new fusion(s).");

                await StartupMultipleAsync(newProcesses, token); // calls all the startups + nurse statusses

                logService.Info("Started all new fusion(s) and checked their patient(s).");

                ResumeAll(token);

                logService.Info("Resumed all drainers, deployment completed.");

                if (!announcement.IsDelta())
                {
                    isFullCompleted = true;
                }
            }
            finally
            {
                syncLock.Release();
            }
        }

        /// <summary>
        /// Announces a running app on which rest port it's bound.
        /// </summary>
        /// <param name="fusionId">Identity of the fusion.</param>
        /// <param name="port">Port of the fusions' rest service.</param>
        /// <param name="token">Token of cancellation.</param>
        /// <inheritdoc />
        public async Task AnnounceAsync(string fusionId, int port, CancellationToken token)
        {
            EnsureArg.IsNotNullOrEmpty(fusionId, nameof(fusionId));

            var process = default(IFusionProcess);

            if (!TryGetActiveProcess(fusionId, out process))
            {
                throw new ScheduleException(ScheduleException.NotFound, fusionId);
            }

            process.Announce(port);

            if (!IsDeploying())
            {
                try
                {
                    await StartupAsync(process, token);
                }
                catch
                {
                    process.OnRespawnStartupFailed();
                    throw;
                }
            }
        }

        private bool TryGetActiveProcess(string fusionId, out IFusionProcess process)
        {
            process = default(IFusionProcess);

            return processes?.TryGetValue(fusionId, out process) == true;
        }

        private IEnumerable<IFusionProcess> GetActiveProcesses(IEnumerable<string> fusionIds)
        {
            var current = default(IFusionProcess);

            foreach (string fusionId in fusionIds)
            {
                if (TryGetActiveProcess(fusionId, out current))
                {
                    yield return current;
                }
            }
        }

        private async Task StartupMultipleAsync(IEnumerable<IFusionProcess> processes, CancellationToken token)
        {
            foreach (var process in processes)
            {
                token.ThrowIfCancellationRequested();

                await StartupAsync(process, token);
            }
        }

        private async Task StartupAsync(IFusionProcess process, CancellationToken token)
        {
            await process.StartupAsync(token);

            foreach (var interceptor in interceptors)
            {
                token.ThrowIfCancellationRequested();

                interceptor.OnStartupCalled(process);

                logService.Debug($"Interceptor: '{interceptor.GetType().Name}' called for fusion: '{process.FusionId}'.");
            }

            process.OnInterceptorsInformed();

            await FetchNurseStatusAsync(process, token);
        }

        private async Task FetchNurseStatusAsync(IFusionProcess process, CancellationToken token)
        {
            IEnumerable<PatientStatus> statusses;

            try
            {
                statusses = (await process.NurseStatusAsync("*", token)).Stale();

                foreach(var status in statusses)
                {
                    logService.Debug($"Patient: '{status.Id}' received for : '{process.FusionId}' with status '{status.Type}'.");
                }
            }
            catch (Exception ex)
            {
                throw new ScheduleException(ScheduleException.NurseStatusFailure, process.FusionId, ex);
            }

            var allowNoPatients = configStore.Value.Fuse.AllowFusionsWithoutPatients;

            if (!allowNoPatients &&
                !statusses.Any())
            {
                var builder = new StringBuilder();
                builder.AppendLine(ScheduleException.NurseUnhealthyPatientsPrefix);
                builder.AppendLine();
                builder.AppendLine("No patients created for this fusion.");

                throw new ScheduleException(builder.ToString(), process.FusionId);
            }

            var unhealthyStatusses = statusses
                .Where(_ => _.Type == PatientStatusType.Red)
                .ToArray();

            if (unhealthyStatusses.Any())
            {
                var builder = new StringBuilder();
                builder.AppendLine(ScheduleException.NurseUnhealthyPatientsPrefix);
                
                foreach(var status in unhealthyStatusses)
                {
                    builder.AppendLine();
                    builder.AppendLine($"Id: {status.Id}");
                    builder.AppendLine($"Status: {status.Type.ToString()}");
                    builder.AppendLine($"Reason: {status.Reason}");
                }

                throw new ScheduleException(builder.ToString(), process.FusionId);
            }
        }

        private IEnumerable<IFusionProcess> SpawnMultiple(IEnumerable<string> fusionIds, CancellationToken token)
        {
            foreach (var fusionId in fusionIds)
            {
                token.ThrowIfCancellationRequested();

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

        private async Task WaitForAnnouncements(IEnumerable<IFusionProcess> processes, CancellationToken token)
        {
            var startedUtc = DateTime.UtcNow;
            var pendingProcesses = default(IEnumerable<IFusionProcess>);

            processes = processes.Stale();

            do
            {
                token.ThrowIfCancellationRequested();

                pendingProcesses = processes
                    .Where(_ => _.State != FusionProcessState.Announced)
                    .Stale();

                var deadProcesses = processes
                    .Where(_ => _.State == FusionProcessState.Dead)
                    .Stale();

                var hasTimeout = (DateTime.UtcNow - startedUtc) >= waitForAnnouncementTimeout;

                if (deadProcesses.Any())
                {
                    var errors = deadProcesses
                        .Select(_ => new ScheduleException(GetDeathErrorMessage(_), _.FusionId));

                    throw new AggregateException(errors);
                }

                if (hasTimeout &&
                    pendingProcesses.Any())
                {
                    var errors = pendingProcesses
                        .Select(_ => new ScheduleException(ScheduleException.TimedOut, _.FusionId));

                    throw new AggregateException(errors);
                }

                await Task.Delay(waitForAnnouncementInterval);
            }
            while (pendingProcesses.Any());
        }

        private async Task TerminateAllAsync(CancellationToken token)
        {
            await syncLock.WaitAsync();

            try
            {
                await TerminateMultipleAsync(processes.Values, token);
            }
            finally
            {
                syncLock.Release();
            }
        }

        private async Task TerminateMultipleAsync(IEnumerable<IFusionProcess> processes, CancellationToken token)
        {
            processes = processes.Stale();

            try
            {
                await DrainAsync(processes, token);
            }
            catch (OperationCanceledException)
            {
                ResumeAll(token);

                throw;
            }

            foreach (var process in processes)
            {
                token.ThrowIfCancellationRequested();

                await TerminateAsync(process, token);
            }
        }

        private async Task TerminateAsync(IFusionProcess process, CancellationToken token)
        {
            try
            {
                await process.TerminateAsync(token);

                (process as IDisposable).Dispose();
            }
            finally
            {
                processes?.Remove(process.FusionId);
            }
        }

        private async Task DrainAsync(IEnumerable<IFusionProcess> processes, CancellationToken token)
        {
            processes = processes.Stale();

            if (!processes.Any())
            {
                return;
            }

            foreach (var drainer in drainers)
            {
                token.ThrowIfCancellationRequested();

                await drainer.DrainAsync(processes, token);

                logService.Debug($"Drainer: '{drainer.GetType().Name}' executed.");
            }
        }

        private void ResumeAll(CancellationToken token)
        {
            foreach (var drainer in drainers)
            {
                token.ThrowIfCancellationRequested();

                drainer.Resume();
            }
        }

        private int GetOrderId(string fusionId)
        {
            return configStore.Value?.Fuse?.Fusions?
                .SingleOrDefault(_ => string.Equals(_.Id, fusionId, StringComparison.OrdinalIgnoreCase))?.Order ?? int.MaxValue;
        }

        private string GetDeathErrorMessage(IFusionProcess process)
        {
            var builder = new StringBuilder();
            builder.AppendLine(ScheduleException.Dead);
            builder.AppendLine(process.GetErrorOutput());
            return builder.ToString();
        }

        /// <summary>
        /// Releases all resources used by the <see cref="ScheduleService"/> instance.
        /// </summary>
        public void Dispose()
        {
            try
            {
                TerminateAllAsync(CancellationToken.None)
                    .GetAwaiter()
                    .GetResult();
            }
            catch (Exception ex)
            {
                logService?.Fatal("Failed to terminate all active processes.", ex);
            }

            syncLock?.Dispose();
            syncLock = null;

            processes?.Clear();
            processes = null;
        }
    }
}
