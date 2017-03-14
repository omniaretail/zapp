using System.Collections.Generic;
using Zapp.Config;
using Zapp.Pack;

namespace Zapp.Fuse
{
    /// <summary>
    /// Represents an interface for adding <see cref="IPackageEntry"/> to a fusion.
    /// </summary>
    public interface IFusionInterceptor
    {
        /// <summary>
        /// Gets a collection of <see cref="IPackageEntry"/> which needs to be added.
        /// </summary>
        /// <param name="fusionConfig">Config of the constructing fusion.</param>
        IReadOnlyCollection<IPackageEntry> GetEntries(FusePackConfig fusionConfig);
    }
}
