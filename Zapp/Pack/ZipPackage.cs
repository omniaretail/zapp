using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Zapp.Pack
{
    /// <summary>
    /// Represents an implementation of <see cref="IPackage"/> for compressed packages.
    /// </summary>
    public class ZipPackage : IPackage, IDisposable
    {
        private ZipArchive archive;

        /// <summary>
        /// Represents the version of the package.
        /// </summary>
        /// <inheritdoc />
        public PackageVersion Version { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="ZipPackage"/>.
        /// </summary>
        /// <param name="version"> Version of the package.</param>
        /// <param name="contentStream">Stream of the package content.</param>
        /// <exception cref="ArgumentException">Throw when either <paramref name="version"/> or <paramref name="contentStream"/> is not set.</exception>
        public ZipPackage(PackageVersion version, Stream contentStream)
        {
            if (version == null) throw new ArgumentNullException(nameof(version));
            if (contentStream == null) throw new ArgumentNullException(nameof(contentStream));

            Version = version;

            archive = new ZipArchive(contentStream, ZipArchiveMode.Read);
        }

        /// <summary>
        /// Get the entries of the package.
        /// </summary>
        /// <inheritdoc />
        public IReadOnlyCollection<IPackageEntry> GetEntries()
        {
            return null;
        }

        /// <summary>
        /// Releases all used resourced by the <see cref="ZipPackage"/> instance.
        /// </summary>
        public void Dispose()
        {
            archive?.Dispose();
            archive = null;
        }
    }
}
