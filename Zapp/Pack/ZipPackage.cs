using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Zapp.Pack
{
    /// <summary>
    /// Represents an implementation of <see cref="IPackage"/> for zip compressed files.
    /// </summary>
    public class ZipPackage : IPackage, IDisposable
    {
        private ZipArchive archive;

        /// <summary>
        /// Represents all the entries (files) in the package.
        /// </summary>
        public IReadOnlyCollection<string> Entries =>
            archive?.Entries.Select(e => e.Name).ToList();

        /// <summary>
        /// Initializes a new <see cref="ZipPackage"/>.
        /// </summary>
        /// <param name="stream">Stream of the package to load.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="stream"/> is not set.</exception>
        /// <exception cref="PackageLoadFailureException">Thrown when the package failed to load.</exception>
        public ZipPackage(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            try
            {
                archive = new ZipArchive(stream, ZipArchiveMode.Read);
            }
            catch (Exception ex)
            {
                throw new PackageLoadFailureException("Package failed to load.", ex);
            }
        }

        /// <summary>
        /// Reads a entry from the package.
        /// </summary>
        /// <param name="name">Name of entry to read.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is empty.</exception>
        /// <exception cref="KeyNotFoundException">Throw when entry is not found.</exception>
        public Stream ReadEntry(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Must be non-empty.", nameof(name));

            var entry = archive?.GetEntry(name);

            if (entry == null)
            {
                throw new KeyNotFoundException($"Entry: {name} not found.");
            }

            return entry.Open();
        }

        /// <summary>
        /// Releases all resourced used by the <see cref="ZipPackage"/> instance.
        /// </summary>
        public void Dispose()
        {
            archive?.Dispose();
            archive = null;
        }
    }
}
