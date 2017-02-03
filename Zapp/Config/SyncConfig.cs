using Newtonsoft.Json;

namespace Zapp.Config
{
    /// <summary>
    /// Represents a class with configuration for the sync-service.
    /// </summary>
    public class SyncConfig
    {
        /// <summary>
        /// Represents the address of the sync-server.
        /// </summary>
        [JsonProperty("connectionString")]
        public string ConnectionString { get; set; } = "localhost,defaultDatabase=5";
    }
}
