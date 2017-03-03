using Newtonsoft.Json;
using System.Collections.Generic;

namespace Zapp.Config
{
    /// <summary>
    /// Represents a configuration for a fusion package.
    /// </summary>
    public class FusePackConfig
    {
        /// <summary>
        /// Represents the identity of the package.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Represents the order of scheduling of the package.
        /// </summary>
        [JsonProperty("order")]
        public int Order { get; set; }

        /// <summary>
        /// Represents a collection with the containing packages.
        /// </summary>
        [JsonProperty("packageIds")]
        public List<string> PackageIds { get; set; } = new List<string>();
    }
}
