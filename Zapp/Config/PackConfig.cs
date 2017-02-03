using Newtonsoft.Json;

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
    }
}
