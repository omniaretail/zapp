using System;
using System.IO;
using Zapp.Pack;

namespace Zapp.Fuse
{
    /// <summary>
    /// Represents an implementation of <see cref="IPackageEntry"/> for fusion process.
    /// </summary>
    public class FusionProcessEntry : IPackageEntry
    {
        private const string entryName = "Zapp.Process.exe";

        /// <summary>
        /// Represents the name of the entry.
        /// </summary>
        /// <inheritdoc />
        public string Name { get; set; } = entryName;

        /// <summary>
        /// Opens the entry with a streamed content.
        /// </summary>
        /// <inheritdoc />
        public Stream Open() => File.OpenRead(GetProcessFilePath());

        private string GetProcessFilePath() => Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            entryName
        );
    }
}
