using EnsureThat;
using log4net;
using System;
using System.IO;
using System.IO.Compression;
using Zapp.Catalogue;
using Zapp.Config;

namespace Zapp.Fuse
{
    /// <summary>
    /// Represents an implementation of <see cref="IFusionExtracter"/> which extracts fusions to disk.
    /// </summary>
    public class FusionExtractor : IFusionExtracter
    {
        private readonly ILog logService;
        private readonly IConfigStore configStore;

        private readonly IFusionMaid fusionMaid;
        private readonly IFusionCatalogue fusionCatalogue;

        /// <summary>
        /// Initializes a new <see cref="FusionExtractor"/>.
        /// </summary>
        /// <param name="logService">Service used for logging.</param>
        /// <param name="configStore">Store used for configuration loading.</param>
        /// <param name="fusionMaid">Virtual maid that cleans the old fusions.</param>
        /// <param name="fusionCatalogue">Catalogue used to resolve locations of fusions.</param>
        public FusionExtractor(
            ILog logService,
            IConfigStore configStore,
            IFusionMaid fusionMaid,
            IFusionCatalogue fusionCatalogue)
        {
            this.logService = logService;
            this.configStore = configStore;

            this.fusionMaid = fusionMaid;
            this.fusionCatalogue = fusionCatalogue;
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
            EnsureArg.IsNotNull(config, nameof(config));
            EnsureArg.IsNotNull(contentStream, nameof(contentStream));

            fusionMaid.CleanAll(config.Id);

            var fusionLocation = fusionCatalogue
                .CreateLocation(config.Id);

            Directory.CreateDirectory(fusionLocation);

            using (var archive = new ZipArchive(contentStream))
            {
                archive.ExtractToDirectory(fusionLocation);

                logService.Info($"Fusion {config.Id} extracted.");
            }
        }
    }
}
