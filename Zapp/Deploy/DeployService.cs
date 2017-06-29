using System;
using System.Collections.Generic;
using System.Linq;
using Zapp.Core.Clauses;
using Zapp.Exceptions;
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
        /// <inheritdoc />
        public void Announce(PackageVersion version) => Announce(new[] { version });

        /// <summary>
        /// Announces a new collection of package versions.
        /// </summary>
        /// <param name="versions">Collection of package versions.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="versions"/> is not set.</exception>
        /// <inheritdoc />
        public void Announce(IEnumerable<PackageVersion> versions)
        {
            Guard.ParamNotNull(versions, nameof(versions));

            var exhausedVersions = versions.Exhaust();

            var nonExistingPackages = exhausedVersions
                .Where(_ => !packService.IsPackageVersionDeployed(_))
                .ToArray();

            if (nonExistingPackages.Any())
            {
                var errors = nonExistingPackages
                    .Select(_ => new PackageException(PackageException.NotFound, _));

                throw new AggregateException(errors);
            }

            var affections = versions
                .SelectMany(_ => fusionService.GetAffectedFusions(_.PackageId))
                .Distinct(StringComparer.OrdinalIgnoreCase);

            scheduleService.ScheduleMultiple(affections);
        }
    }
}
