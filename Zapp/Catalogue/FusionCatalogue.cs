using AntPathMatching;
using EnsureThat;
using StringFormat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Zapp.Config;

namespace Zapp.Catalogue
{
    /// <summary>
    /// Represents a file catalogue for the fusions.
    /// </summary>
    /// <inheritDoc />
    public class FusionCatalogue : IFusionCatalogue
    {
        private static readonly DateTime utcEpochStartTime = new DateTime(1970, 1, 1);

        private readonly IConfigStore configStore;
        private readonly IAntFactory antFactory;

        private string rootDirectory;

        /// <summary>
        /// Initializes a new <see cref="FusionCatalogue"/> with it's dependencies.
        /// </summary>
        /// <param name="configStore">Store used to retrieve configuration values.</param>
        /// <param name="antFactory">Factory to create <see cref="IAnt"/> instances.</param>
        public FusionCatalogue(
            IConfigStore configStore,
            IAntFactory antFactory)
        {
            this.configStore = configStore;
            this.antFactory = antFactory;

            rootDirectory = FormatRootDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        /// <summary>
        /// Creates a new location for the requested fusion.
        /// </summary>
        /// <param name="fusionId">Id of the requested fusion.</param>
        /// <inheritDoc />
        public string CreateLocation(string fusionId)
        {
            EnsureArg.IsNotNullOrEmpty(fusionId, nameof(fusionId));

            var name = FormatFusionDirectory(fusionId, GetEpoch().ToString());

            return Path.Combine(rootDirectory, name);
        }

        /// <summary>
        /// Gets the active location of the requested fusion.
        /// </summary>
        /// <param name="fusionId">Id of the requested fusion.</param>
        /// <inheritDoc />
        public string GetActiveLocation(string fusionId)
        {
            EnsureArg.IsNotNullOrEmpty(fusionId, nameof(fusionId));

            return GetAllLocations(fusionId)
                .OrderByDescending(_ => _)
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets all the locations of the requested fusion.
        /// </summary>
        /// <param name="fusionId">Id of the requested fusion.</param>
        /// <inheritDoc />
        public IEnumerable<string> GetAllLocations(string fusionId)
        {
            EnsureArg.IsNotNullOrEmpty(fusionId, nameof(fusionId));

            var pattern = FormatFusionDirectory(fusionId, "*");
            var ant = antFactory.CreateNew(pattern);
            var rootInfo = new DirectoryInfo(rootDirectory);

            return rootInfo
                .GetDirectories()
                .Where(_ => ant.IsMatch(_.Name))
                .Select(_ => _.FullName);
        }

        private string FormatFusionDirectory(string fusionId, string epoch)
        {
            var args = new
            {
                fusionId = fusionId,
                epoch = epoch
            };

            var baseFormat = configStore.Value?.Fuse?.FusionDirectoryPattern;

            return TokenStringFormat.Format(baseFormat, args);
        }

        private string FormatRootDirectory(string zappDir)
        {
            var args = new
            {
                zappDir = zappDir
            };

            var baseFormat = configStore.Value?.Fuse?.RootDirectory;

            return TokenStringFormat.Format(baseFormat, args);
        }

        private long GetEpoch() => (long)(DateTime.UtcNow - utcEpochStartTime).TotalSeconds;
    }
}
