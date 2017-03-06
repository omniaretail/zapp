using System.IO;
using Zapp.Pack;
using Zapp.Process;

namespace Zapp.Fuse
{
    /// <summary>
    /// Represents an implementation of <see cref="IPackageEntry"/> for fusion process it's config.
    /// </summary>
    public class FusionProcessConfigEntry : IPackageEntry
    {
        private const string resourceName = "Zapp.Process.App.config";

        /// <summary>
        /// Represents the default name of this entry.
        /// </summary>
        public const string DefaultEntryName = "Zapp.Process.exe.config";

        /// <summary>
        /// Represents the name of the entry.
        /// </summary>
        /// <inheritdoc />
        public string Name { get; set; } = DefaultEntryName;

        /// <summary>
        /// Opens the entry with a streamed content.
        /// </summary>
        /// <inheritdoc />
        public Stream Open() => typeof(ZappProcessModule).Assembly
            .GetManifestResourceStream(resourceName);
    }
}
