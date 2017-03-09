using System;
using System.IO;
using Zapp.Pack;

namespace Zapp.Fuse
{
    /// <summary>
    /// Represents an implementation of <see cref="IPackageEntry"/> for fusion process.
    /// </summary>
    public class FusionProcessEntry : IFrameworkPackageEntry
    {
        /// <summary>
        /// Represents the default name of this entry.
        /// </summary>
        public const string DefaultEntryName = "Zapp.Process.exe";

        /// <summary>
        /// Represents the name of the entry.
        /// </summary>
        /// <inheritdoc />
        public string Name { get; set; } = DefaultEntryName;

        /// <summary>
        /// Opens the entry with a streamed content.
        /// </summary>
        /// <inheritdoc />
        public Stream Open() => File.OpenRead(GetProcessFilePath());

        private string GetProcessFilePath() => Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            DefaultEntryName
        );
    }
}
