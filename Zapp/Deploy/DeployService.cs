using System;
using Zapp.Fuse;
using Zapp.Pack;
using Zapp.Schedule;
using Zapp.Sync;
using Zapp.Core.Clauses;

namespace Zapp.Deploy
{
    /// <summary>
    /// Represents an implementation of <see cref="IDeployService"/> for distributing deployments.
    /// </summary>
    public class DeployService : IDeployService
    {
        private readonly ISyncService syncService;
        private readonly IPackService packService;
        private readonly IFusionService fusionService;
        private readonly IScheduleService scheduleService;

        /// <summary>
        /// Initializes a new <see cref="DeployService"/>.
        /// </summary>
        /// <param name="syncService">Service used for synchronizing package versions.</param>
        /// <param name="packService">Service used for handling packages.</param>
        /// <param name="fusionService">Service used for fusing packages.</param>
        /// <param name="scheduleService">Service used for orchestrating fusion packages.</param>
        public DeployService(
            ISyncService syncService,
            IPackService packService,
            IFusionService fusionService,
            IScheduleService scheduleService)
        {
            this.syncService = syncService;
            this.packService = packService;
            this.fusionService = fusionService;
            this.scheduleService = scheduleService;
        }

        /// <summary>
        /// Announces a new version of a package.
        /// </summary>
        /// <param name="version">Version of the package.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="version"/> is not set.</exception>
        public AnnounceResult Announce(PackageVersion version)
        {
            Guard.ParamNotNull(version, nameof(version));

            if (!syncService.Announce(version))
            {
                return AnnounceResult.InternalError;
            }

            if (!packService.IsPackageVersionDeployed(version))
            {
                return AnnounceResult.NotFound;
            }

            var affectedFusions = fusionService
                .GetAffectedFusions(version.PackageId);

            scheduleService.Schedule(affectedFusions);

            return AnnounceResult.Ok;
        }
    }
}
