using System.Collections.Generic;

namespace Zapp.Pack
{
    /// <summary>
    /// Represents an interface which is responsible for all package-related actions.
    /// </summary>
    public interface IPackageService
    {
        /// <summary>
        /// Loads the package from disk and returns the definition.
        /// </summary>
        /// <param name="path">Path to the package to load.</param>
        IPackage Load(string path);

        /// <summary>
        /// Fuses all the given packages into a new package.
        /// </summary>
        /// <param name="packages">Packages which needs to be fused.</param>
        IPackage Fuse(IReadOnlyCollection<IPackage> packages);
    }
}
