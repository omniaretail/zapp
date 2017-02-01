using System.IO;

namespace Zapp.Pack
{
    /// <summary>
    /// Represents a implementation of <see cref="IPackageFactory"/>.
    /// </summary>
    public class ZipPackageFactory : IPackageFactory
    {

#pragma warning disable 1591

        public IPackage CreateNew(Stream stream) => new ZipPackage(stream);

#pragma warning restore 1591

    }
}
