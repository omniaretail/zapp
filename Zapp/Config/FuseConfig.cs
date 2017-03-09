using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Zapp.Core.Clauses;

namespace Zapp.Config
{
    /// <summary>
    /// Represents a class used for configurating the fusions.
    /// </summary>
    public class FuseConfig
    {
        /// <summary>
        /// Represents the root folder for the fusion packs.
        /// </summary>
        [JsonProperty("rootDir")]
        public string RootDirectory { get; set; } = "{zappDir}/fuse/";

        /// <summary>
        /// Represents the pattern for packing entries.
        /// </summary>
        [JsonProperty("entryPattern")]
        public string EntryPattern { get; set; } = "*.{dll,exe}";

        /// <summary>
        /// Represents the pattern for extracting fusions.
        /// </summary>
        [JsonProperty("fusionDirPattern")]
        public string FusionDirectoryPattern { get; set; } = "{fusionId}/";

        /// <summary>
        /// Represents a collection of configuration for fusions.
        /// </summary>
        [JsonProperty("fusions")]
        public List<FusePackConfig> Fusions { get; set; } = new List<FusePackConfig>();

        /// <summary>
        /// Represents a security configuration for loading unknown assemblies.
        /// </summary>
        [JsonProperty("isLoadFromRemoteSourcesEnabled")]
        public bool IsLoadFromRemoteSourcesEnabled { get; set; } = false;

        /// <summary>
        /// Represents a configuration to enable per-process garbage collection threads.
        /// </summary>
        [JsonProperty("isGcServerEnabled")]
        public bool IsGcServerEnabled { get; set; } = false;

        /// <summary>
        /// Represents a configuration to enable concurrent garbage collection.
        /// </summary>
        [JsonProperty("isGcConcurrentEnabled")]
        public bool IsGcConcurrentEnabled { get; set; } = true;

        /// <summary>
        /// Represents a configuration to enable garbage collection for very large objects.
        /// </summary>
        [JsonProperty("isGcVeryLargeObjectsAllowed")]
        public bool IsGcVeryLargeObjectsAllowed { get; set; } = false;

        /// <summary>
        /// Resolves the actual root directory.
        /// </summary>
        public string GetActualRootDirectory() => RootDirectory
            .Replace("{zappDir}", AppDomain.CurrentDomain.BaseDirectory);

        /// <summary>
        /// Resolves the actual fusion directory.
        /// </summary>
        /// <param name="fusionId">Identity of the fusion.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fusionId"/> is not set.</exception>
        public string GetActualFusionDirectory(string fusionId)
        {
            Guard.ParamNotNullOrEmpty(fusionId, nameof(fusionId));

            return Path.Combine(
                GetActualRootDirectory(),
                FusionDirectoryPattern.Replace("{fusionId}", fusionId)
            );
        }
    }
}
