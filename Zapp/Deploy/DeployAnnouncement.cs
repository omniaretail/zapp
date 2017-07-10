using EnsureThat;
using System;
using System.Collections.Generic;
using System.Linq;
using Zapp.Extensions;
using Zapp.Fuse;
using Zapp.Pack;

namespace Zapp.Deploy
{
    /// <summary>
    /// Represents a announcement of a deployment.
    /// </summary>
    public class DeployAnnouncement : IDeployAnnouncement
    {
        private readonly IFusionService fusionService;

        private readonly IReadOnlyCollection<PackageVersion> packageVersions;
        private readonly IReadOnlyCollection<string> affectedFusionIds;

        /// <summary>
        /// Initializes a new <see cref="DeployAnnouncement"/> with the new <paramref name="packageVersions"/> that needs to be deployed.
        /// </summary>
        /// <param name="fusionService">Service used to get info for package fusions.</param>
        /// <param name="packageVersions">Versions of the packages that needs to be deployed.</param>
        public DeployAnnouncement(
            IFusionService fusionService,
            IEnumerable<PackageVersion> packageVersions)
        {
            EnsureArg.IsNotNull(packageVersions, nameof(packageVersions));

            this.fusionService = fusionService;
            this.packageVersions = packageVersions.StaleReadOnly();

            affectedFusionIds = GetAffectedFusionIds(this.packageVersions).StaleReadOnly();
        }

        /// <summary>
        /// Initializes a new <see cref="DeployAnnouncement"/> with the <paramref name="fusionIds"/> that needs to be deployed.
        /// </summary>
        /// <param name="fusionService">Service used to get info for package fusions.</param>
        /// <param name="fusionIds">Ids of the fusions that needs to be deployed.</param>
        public DeployAnnouncement(
            IFusionService fusionService,
            IEnumerable<string> fusionIds)
        {
            EnsureArg.IsNotNull(fusionIds, nameof(fusionIds));

            this.fusionService = fusionService;

            packageVersions = new PackageVersion[0];
            affectedFusionIds = fusionIds.StaleReadOnly();
        }

        /// <summary>
        /// Indicates if this deployment is a delta.
        /// </summary>
        /// <inheritDoc />
        public bool IsDelta() => packageVersions?.Any() == true;

        /// <summary>
        /// Gets the fusions that are touched by the deployment.
        /// </summary>
        /// <inheritdoc />
        public IEnumerable<string> GetFusionIds() => affectedFusionIds;

        /// <summary>
        /// Gets all the package versions of the requested fusion.
        /// </summary>
        /// <param name="fusionId">Id of the fusion where the versions are requested of.</param>
        /// <inheritdoc />
        public IEnumerable<PackageVersion> GetPackageVersions(string fusionId)
        {
            EnsureArg.IsNotNullOrEmpty(fusionId, nameof(fusionId));

            return fusionService
                .GetPackageVersions(fusionId)
                .Select(_ => GetLatestVersion(_));
        }

        /// <summary>
        /// Gets all the new package versions that were requested in this announcement.
        /// </summary>
        /// <inheritdoc />
        public IEnumerable<PackageVersion> GetNewPackageVersions() => packageVersions;

        private PackageVersion GetLatestVersion(PackageVersion original)
        {
            return packageVersions
                .SingleOrDefault(_ => string.Equals(_.PackageId, original.PackageId, StringComparison.OrdinalIgnoreCase))
                ?? original;
        }

        private IEnumerable<string> GetAffectedFusionIds(IEnumerable<PackageVersion> versions)
        {
            return versions
                .SelectMany(_ => fusionService.GetAffectedFusions(_.PackageId))
                .Distinct(StringComparer.OrdinalIgnoreCase);
        }
    }
}
