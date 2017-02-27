using System.Collections.Generic;
using Zapp.Pack;

namespace Zapp.Deploy
{
    /// <summary>
    /// Represents an interface mainly used for distributing deployments.
    /// </summary>
    public interface IDeployService
    {
        /// <summary>
        /// Announces a new version of a package.
        /// </summary>
        /// <param name="version">Version of the package.</param>
        AnnounceResult Announce(PackageVersion version);

        /// <summary>
        /// Announces a new collection of package versions.
        /// </summary>
        /// <param name="versions">Collection of package versions.</param>
        AnnounceResult Announce(IReadOnlyCollection<PackageVersion> versions);
    }
}
