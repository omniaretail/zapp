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
    }
}
