using System;
using System.Collections.Generic;
using System.IO;

namespace Zapp.Pack
{
    /// <summary>
    /// Represents a implementation of <see cref="IPackageService"/> for zip-compressed packages.
    /// </summary>
    public class PackageService : IPackageService
    {
        private readonly IPackageFactory packageFactory;

        /// <summary>
        /// Initializes a new <see cref="PackageService"/>.
        /// </summary>
        /// <param name="packageFactory">Factory to create packages with.</param>
        public PackageService(IPackageFactory packageFactory)
        {
            this.packageFactory = packageFactory;
        }

        /// <summary>
        /// Fuses all the given packages into a new package.
        /// </summary>
        /// <param name="packages">Packages which needs to be fused.</param>
        public IPackage Fuse(IReadOnlyCollection<IPackage> packages)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Loads a package from the given stream.
        /// </summary>
        /// <param name="stream">Stream of the package.</param>
        public IPackage Load(Stream stream) => packageFactory.CreateNew(stream);
    }
}
