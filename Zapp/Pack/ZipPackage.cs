using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Zapp.Utils;

namespace Zapp.Pack
{
    /// <summary>
    /// Represents an implementation of <see cref="IPackage"/> for compressed packages.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class ZipPackage : IPackage, IDisposable
    {
        private ZipArchive archive;

        private readonly IPackageEntryFactory packageEntryFactory;

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
        /// <param name="packageEntryFactory">Factory for creating <see cref="IPackageEntry"/> instances.</param>
        /// <exception cref="ArgumentNullException">Throw when either <paramref name="version"/> or <paramref name="contentStream"/> is not set.</exception>
        /// <exception cref="PackageException">Throw when the <paramref name="contentStream"/> is not a compatible stream.</exception>
        public ZipPackage(
            PackageVersion version,
            Stream contentStream,
            IPackageEntryFactory packageEntryFactory)
        {
            Guard.ParamNotNull(version, nameof(version));
            Guard.ParamNotNull(contentStream, nameof(contentStream));

            Version = version;

            try
            {

                archive = new ZipArchive(contentStream, ZipArchiveMode.Read);
            }
            catch (Exception ex)
            {
                throw new PackageException("Package not compatible.", version, ex);
            }

            this.packageEntryFactory = packageEntryFactory;
        }

        /// <summary>
        /// Get the entries of the package.
        /// </summary>
        /// <inheritdoc />
        public IReadOnlyCollection<IPackageEntry> GetEntries()
        {
            return archive.Entries
                .Select(e => packageEntryFactory
                    .CreateNew(e.FullName, new LazyStream(e.Open)))
                .ToList();
        }

        private string DebuggerDisplay => $"Package: {Version.PackageId} - {Version.DeployVersion}";

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
