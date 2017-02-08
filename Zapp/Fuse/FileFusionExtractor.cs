using log4net;
using System;
using System.IO;
using System.IO.Compression;
using Zapp.Config;
using Zapp.Utils;

namespace Zapp.Fuse
{
    /// <summary>
    /// Represents an implementation of <see cref="IFusionExtracter"/> which extracts fusions to disk.
    /// </summary>
    public class FileFusionExtractor : IFusionExtracter
    {
        private readonly ILog logService;
        private readonly IConfigStore configStore;

        private readonly string fusionRootDirectory;

        /// <summary>
        /// Initializes a new <see cref="FileFusionExtractor"/>.
        /// </summary>
        /// <param name="logService">Service used for logging.</param>
        /// <param name="configStore">Store used for configuration loading.</param>
        public FileFusionExtractor(
            ILog logService,
            IConfigStore configStore)
        {
            this.logService = logService;
            this.configStore = configStore;

            fusionRootDirectory = this.configStore.Value?.Fuse?
                .GetActualRootDirectory();
        }

        /// <summary>
        /// Extracts the stream 
        /// </summary>
        /// <param name="config">Configuration of the fusion.</param>
        /// <param name="contentStream">Stream of the fusion.</param>
        /// <exception cref="ArgumentNullException">Throw when either <paramref name="config"/> or <paramref name="contentStream"/> is not set.</exception>
        /// <inheritdoc />
        public void Extract(FusePackConfig config, Stream contentStream)
        {
            Guard.ParamNotNull(config, nameof(config));
            Guard.ParamNotNull(contentStream, nameof(contentStream));

            var fusionDirectory = configStore.Value?.Fuse?
                .GetActualFusionDirectory(config.Id);

            if (Directory.Exists(fusionDirectory))
            {
                Directory.Delete(fusionDirectory, true);
            }

            Directory.CreateDirectory(fusionDirectory);

            using (var archive = new ZipArchive(contentStream))
            {
                archive.ExtractToDirectory(fusionDirectory);

                logService.Info($"fusion {config.Id} extracted.");
            }
        }
    }
}
