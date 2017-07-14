using System.Collections.Generic;
using Zapp.Pack;

namespace Zapp.Deploy
{
    /// <summary>
    /// Represents a factory for the <see cref="IDeployAnnouncement"/>.
    /// </summary>
    public interface IDeployAnnouncementFactory
    {
        /// <summary>
        /// Creates a new <see cref="IDeployAnnouncement"/> with the requested <paramref name="packageVersions"/>.
        /// </summary>
        /// <param name="fusionIds">Ids of the fusions that needs to be deployed. (empty if <paramref name="packageVersions"/> is specified).</param>
        /// <param name="packageVersions">Versions of the packages that needs to be deployed. (empty if <paramref name="fusionIds"/> is specified).</param>
        IDeployAnnouncement CreateNew(
            IEnumerable<string> fusionIds, 
            IEnumerable<PackageVersion> packageVersions
        );
    }
}
