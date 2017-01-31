using System;
using System.Collections.Generic;

namespace Zapp.Pack
{
    /// <summary>
    /// Represents a implementation of <see cref="IPackageService"/> for zip-compressed packages.
    /// </summary>
    public class ZipPackageService : IPackageService
    {
        /// <summary>
        /// Fuses all the given packages into a new package.
        /// </summary>
        /// <param name="packages">Packages which needs to be fused.</param>
        public IPackage Fuse(IReadOnlyCollection<IPackage> packages)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads the package from disk and returns the definition.
        /// </summary>
        /// <param name="path">Path to the package to load.</param>
        public IPackage Load(string path)
        {
            throw new NotImplementedException();
        }
    }
}
