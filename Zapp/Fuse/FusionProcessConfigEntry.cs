using System.IO;
using Zapp.Pack;
using Zapp.Process;
using Zapp.Transform;

namespace Zapp.Fuse
{
    /// <summary>
    /// Represents an implementation of <see cref="IPackageEntry"/> for fusion process it's config.
    /// </summary>
    public class FusionProcessConfigEntry : IFrameworkPackageEntry
    {
        private const string resourceName = "Zapp.Process.App.config";

        private readonly ITransformConfig transformConfig;

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
        /// Initializes a new <see cref="FusionProcessConfigEntry"/>.
        /// </summary>
        /// <param name="transformConfig">Transformer used to transform the config.</param>
        public FusionProcessConfigEntry(
            ITransformConfig transformConfig)
        {
            this.transformConfig = transformConfig;
        }

        /// <summary>
        /// Opens the entry with a streamed content.
        /// </summary>
        /// <inheritdoc />
        public Stream Open()
        {
            var output = new MemoryStream();
            var assembly = typeof(ZappProcessModule).Assembly;

            using (var input = assembly.GetManifestResourceStream(resourceName))
            {
                transformConfig.Transform(input, output);
            }

            output.Seek(0, SeekOrigin.Begin);

            return output;
        }
    }
}
