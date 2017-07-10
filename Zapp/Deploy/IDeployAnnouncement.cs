using System.Collections.Generic;
using Zapp.Pack;

namespace Zapp.Deploy
{
    /// <summary>
    /// Represents an interface that resolves all the values needed for a deployment.
    /// </summary>
    public interface IDeployAnnouncement
    {
        /// <summary>
        /// Indicates if this deployment is a delta.
        /// </summary>
        bool IsDelta();

        /// <summary>
        /// Gets the fusions that are touched by the deployment.
        /// </summary>
        IEnumerable<string> GetFusionIds();

        /// <summary>
        /// Gets all the package versions of the requested fusion.
        /// </summary>
        /// <param name="fusionId">Id of the fusion where the versions are requested of.</param>
        IEnumerable<PackageVersion> GetPackageVersions(string fusionId);

        /// <summary>
        /// Gets all the new package versions that were requested in this announcement.
        /// </summary>
        IEnumerable<PackageVersion> GetNewPackageVersions();
    }
}
