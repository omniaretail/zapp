using System.Collections.Generic;
using System.IO;

namespace Zapp.Pack
{
    /// <summary>
    /// Represents an interface which is responsible for all package-related actions.
    /// </summary>
    public interface IPackageService
    {
        /// <summary>
        /// Loads a package from the given stream.
        /// </summary>
        /// <param name="stream">Stream of the package.</param>
        IPackage Load(Stream stream);

        /// <summary>
        /// Fuses all the given packages into a new package.
        /// </summary>
        /// <param name="packages">Packages which needs to be fused.</param>
        IPackage Fuse(IReadOnlyCollection<IPackage> packages);
    }
}
