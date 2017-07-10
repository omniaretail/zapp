using EnsureThat;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zapp.Extensions;
using Zapp.Fuse;
using Zapp.Pack;
using Zapp.Schedule;
using Zapp.Sync;

namespace Zapp.Deploy
{
    /// <summary>
    /// Represents an implementation of <see cref="IDeployService"/> for distributing deployments.
    /// </summary>
    public class DeployService : IDeployService
    {
        private readonly ISyncService syncService;
        private readonly IFusionService fusionService;
        private readonly IScheduleService scheduleService;
        private readonly IDeployAnnouncementFactory announcementFactory;
        private readonly IPackageVersionValidator packageVersionValidator;

        /// <summary>
        /// Initializes a new <see cref="DeployService"/>.
        /// </summary>
        /// <param name="syncService">Service used for synchronizing package versions.</param>
        /// <param name="fusionService">Service used for fusing packages.</param>
        /// <param name="scheduleService">Service used for orchestrating fusion packages.</param>
        /// <param name="announcementFactory">Factory that creates <see cref="IDeployAnnouncement"/> instances.</param>
        /// <param name="packageVersionValidator">The validator that is used to validate announced package version(s).</param>
        public DeployService(
            ISyncService syncService,
            IFusionService fusionService,
            IScheduleService scheduleService,
            IDeployAnnouncementFactory announcementFactory,
            IPackageVersionValidator packageVersionValidator)
        {
            this.syncService = syncService;
            this.fusionService = fusionService;
            this.scheduleService = scheduleService;
            this.announcementFactory = announcementFactory;
            this.packageVersionValidator = packageVersionValidator;
        }

        /// <summary>
        /// Announces a new collection of package versions.
        /// </summary>
        /// <param name="versions">Collection of package versions.</param>
        /// <param name="token">Token of cancellation.</param>
        /// <inheritdoc />
        public async Task AnnounceAsync(IEnumerable<PackageVersion> versions, CancellationToken token)
        {
            EnsureArg.IsNotNull(versions, nameof(versions));

            versions = versions.Stale();

            packageVersionValidator.ConfirmAvailability(versions);

            var announcement = announcementFactory.CreateNew(versions);

            await scheduleService.ScheduleAsync(announcement, token);
            await syncService.AnnounceAsync(announcement, token);
        }
    }
}
