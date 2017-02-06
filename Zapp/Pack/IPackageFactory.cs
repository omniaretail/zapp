using System.IO;

namespace Zapp.Pack
{
    /// <summary>
    /// Represents an interface to create instances of <see cref="IPackage"/>.
    /// </summary>
    public interface IPackageFactory
    {
        /// <summary>
        /// Creates a new <see cref="IPackage"/>.
        /// </summary>
        /// <param name="version">Version of the package.</param>
        /// <param name="contentStream">Stream of the package content.</param>
        IPackage CreateNew(PackageVersion version, Stream contentStream);
    }
}
