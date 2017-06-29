using System.Collections.Generic;
using Zapp.Pack;

namespace Zapp.Fuse
{
    /// <summary>
    /// Represents an interface mainly used for fusing packages.
    /// </summary>
    public interface IFusionService
    {
        /// <summary>
        /// Extracts all fusions.
        /// </summary>
        void ExtractAll();

        /// <summary>
        /// Extracts multiple fusions.
        /// </summary>
        /// <param name="fusionIds">Identities of the fusion.</param>
        void ExtractMultiple(IEnumerable<string> fusionIds);

        /// <summary>
        /// Searches for affected fusion packages.
        /// </summary>
        /// <param name="packageId">Identity of the package.</param>
        IReadOnlyCollection<string> GetAffectedFusions(string packageId);

        /// <summary>
        /// Gets the package versions from the sync-service for a specific fusion.
        /// </summary>
        /// <param name="fusionId">Identity of the fusion.</param>
        IReadOnlyCollection<PackageVersion> GetPackageVersions(string fusionId);
    }
}
