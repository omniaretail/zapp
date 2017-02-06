using Newtonsoft.Json;
using System.Collections.Generic;

namespace Zapp.Config
{
    /// <summary>
    /// Represents the configuration for finding the packages.
    /// </summary>
    public class PackConfig
    {
        /// <summary>
        /// Represents the root folder for the packs.
        /// </summary>
        [JsonProperty("rootDir")]
        public string RootDirectory { get; set; } = "{zappDir}/pack/";

        /// <summary>
        /// Represents the pattern for finding packages.
        /// </summary>
        [JsonProperty("packagePattern")]
        public string PackagePattern { get; set; } = "containers-{deployVersion}-dev/{packageId}*.{nupkg,zip}";

        /// <summary>
        /// Represents a collection of configuration for fusions.
        /// </summary>
        [JsonProperty("fusions")]
        public List<PackFusionConfig> Fusions { get; set; } = new List<PackFusionConfig>();
    }
}
