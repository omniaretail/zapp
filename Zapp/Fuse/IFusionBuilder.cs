using System.Collections.Generic;
using Zapp.Config;
using Zapp.Pack;

namespace Zapp.Fuse
{
    /// <summary>
    /// Represents an interface that builds <see cref="IFusion"/> with it's required packages.
    /// </summary>
    public interface IFusionBuilder
    {
        /// <summary>
        /// Builds a new fusion package into <paramref name="fusion"/> with all the required package versions.
        /// </summary>
        /// <param name="fusionConfig">Configuration of the fusion that needs to be builded.</param>
        /// <param name="fusion">Fusion that needs to be builded.</param>
        /// <param name="packageVersions">Versions of the packages that needs to be included in the fusion.</param>
        void Build(FusePackConfig fusionConfig, IFusion fusion, IEnumerable<PackageVersion> packageVersions);
    }
}
