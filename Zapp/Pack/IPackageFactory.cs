using System.IO;

namespace Zapp.Pack
{
    /// <summary>
    /// Represents a interface for creating new instance of <see cref="IPackage"/>.
    /// </summary>
    public interface IPackageFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="IPackage"/>.
        /// </summary>
        /// <param name="stream">Stream of the package.</param>
        IPackage CreateNew(Stream stream);
    }
}
