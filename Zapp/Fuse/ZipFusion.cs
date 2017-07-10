using EnsureThat;
using System;
using System.IO;
using System.IO.Compression;
using Zapp.Pack;

namespace Zapp.Fuse
{
    /// <summary>
    /// Represents an inmplementation of <see cref="IFusion"/> as a zip-file.
    /// </summary>
    public sealed class ZipFusion : IFusion, IDisposable
    {
        private ZipArchive archive;

        /// <summary>
        /// Initializes a new <see cref="ZipFusion"/>.
        /// </summary>
        /// <param name="contentStream">Stream where the fusion is written to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="contentStream"/> is not set.</exception>
        public ZipFusion(Stream contentStream)
        {
            EnsureArg.IsNotNull(contentStream, nameof(contentStream));

            archive = new ZipArchive(contentStream, ZipArchiveMode.Create);
        }

        /// <summary>
        /// Adds an entry to the fusion package.
        /// </summary>
        /// <param name="entry">Entry to add.</param>
        /// <exception cref="ArgumentNullException">Throw when <paramref name="entry"/> is not set.</exception>
        /// <inheritdoc />
        public void AddEntry(IPackageEntry entry)
        {
            EnsureArg.IsNotNull(entry, nameof(entry));

            var newEntry = archive.CreateEntry(entry.Name);

            using (var entryStream = entry.Open())
            using (var newEntryStream = newEntry.Open())
            {
                entryStream.CopyTo(newEntryStream);
            }
        }

        /// <summary>
        /// Releases all used resources by the <see cref="ZipFusion"/> instance.
        /// </summary>
        public void Dispose()
        {
            archive?.Dispose();
            archive = null;
        }
    }
}
