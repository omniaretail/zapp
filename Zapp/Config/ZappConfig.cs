using Newtonsoft.Json;

namespace Zapp.Config
{
    /// <summary>
    /// Represent a class which contains all the possible configuration.
    /// </summary>
    public class ZappConfig
    {
        /// <summary>
        /// Represents the config section for the rest-api.
        /// </summary>
        [JsonProperty("rest")]
        public RestConfig Rest { get; set; } = new RestConfig();

        /// <summary>
        /// Represents the config section for the pack-service.
        /// </summary>
        [JsonProperty("pack")]
        public PackConfig Pack { get; set; } = new PackConfig();

        /// <summary>
        /// Represents the config section for the fusion-service.
        /// </summary>
        [JsonProperty("fuse")]
        public FuseConfig Fuse { get; set; } = new FuseConfig();

        /// <summary>
        /// Represents the config section for the sync-service.
        /// </summary>
        [JsonProperty("sync")]
        public SyncConfig Sync { get; set; } = new SyncConfig();
    }
}
