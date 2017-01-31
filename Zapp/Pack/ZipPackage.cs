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
        private Stream stream;
        private ZipArchive archive;

        /// <summary>
        /// Represents the package's mode.
        /// </summary>
        public PackageMode Mode { get; private set; }

        /// <summary>
        /// Represents if the package is executable.
        /// </summary>
        public bool IsExecutable { get; set; }

        /// <summary>
        /// Represents all the entries (files) in the package.
        /// </summary>
        public IReadOnlyCollection<string> Entries => archive?.Entries
            .Select(e => e.Name)
            .ToList() ?? new List<string>();

        /// <summary>
        /// Initializes a new <see cref="ZipPackage"/>.
        /// </summary>
        /// <param name="fileLocation">Path to the package to load.</param>
        /// <param name="mode">Mode to use this package.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="fileLocation"/> is empty ordoes not exists.</exception>
        public ZipPackage(string fileLocation, PackageMode mode = PackageMode.Read)
        {
            if (string.IsNullOrEmpty(fileLocation)) throw new ArgumentException("Must be non-empty.", nameof(fileLocation));

            Mode = mode;

            switch (mode)
            {
                case PackageMode.Read:
                    stream = File.OpenRead(fileLocation);
                    archive = new ZipArchive(stream, ZipArchiveMode.Read);
                    break;
                case PackageMode.Write:
                    stream = File.OpenWrite(fileLocation);
                    archive = new ZipArchive(stream, ZipArchiveMode.Create);
                    break;
            }
        }

        /// <summary>
        /// Reads a entry from the package.
        /// </summary>
        /// <param name="name">Name of entry to read.</param>
        public Stream GetEntry(string name) => archive.GetEntry(name)?.Open();


        /// <summary>
        /// Writes a entry from the package.
        /// </summary>
        /// <param name="name">Name of entry to write.</param>
        /// <param name="stream">Stream of entry to write.</param>
        public void AddEntry(string name, Stream stream)
        {
            var writeStream = archive.CreateEntry(name).Open();
            stream.CopyTo(writeStream);
        }

        /// <summary>
        /// Releases all resourced used by the <see cref="ZipPackage"/> instance.
        /// </summary>
        public void Dispose()
        {
            if (Mode == PackageMode.Read)
            {
                stream?.Dispose();
            }

            stream = null;

            archive?.Dispose();
            archive = null;
        }
    }
}
