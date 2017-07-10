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
        /// <param name="packageVersions">Versions of the announcement.</param>
        IDeployAnnouncement CreateNew(IEnumerable<PackageVersion> packageVersions);

        /// <summary>
        /// Creates a new <see cref="IDeployAnnouncement"/> with the requested <paramref name="fusionIds"/>.
        /// </summary>
        /// <param name="fusionIds">Ids of the fusions for the announcement.</param>
        IDeployAnnouncement CreateNew(IEnumerable<string> fusionIds);
    }
}
