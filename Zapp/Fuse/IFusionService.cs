using System.Collections.Generic;
using System.Threading;
using Zapp.Deploy;
using Zapp.Pack;

namespace Zapp.Fuse
{
    /// <summary>
    /// Represents an interface mainly used for fusing packages.
    /// </summary>
    public interface IFusionService
    {
        /// <summary>
        /// Extracts all configured fusions.
        /// </summary>
        /// <param name="token">Token of cancellation.</param>
        void ExtractAll(CancellationToken token);

        /// <summary>
        /// Extracts a deploy anncouncement.
        /// </summary>
        /// <param name="announcement">The announcement that needs to be extracted.</param>
        /// <param name="token">Token of cancellation.</param>
        void Extract(IDeployAnnouncement announcement, CancellationToken token);

        /// <summary>
        /// Searches for affected fusion packages.
        /// </summary>
        /// <param name="packageId">Identity of the package.</param>
        IEnumerable<string> GetAffectedFusions(string packageId);

        /// <summary>
        /// Gets the package versions from the sync-service for a specific fusion.
        /// </summary>
        /// <param name="fusionId">Identity of the fusion.</param>
        IEnumerable<PackageVersion> GetPackageVersions(string fusionId);
    }
}
