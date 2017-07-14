using Newtonsoft.Json;
using System.Collections.Generic;

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
        public string FusionDirectoryPattern { get; set; } = "{fusionId}_{epoch}/";

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
        /// Represents a setting that indicates if zapp should ignore an initial schedule failure.
        /// </summary>
        [JsonProperty("ignoreInitialScheduleFailure")]
        public bool IgnoreInitialScheduleFailure = true;
    }
}
