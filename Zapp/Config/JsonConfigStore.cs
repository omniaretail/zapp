using log4net;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Zapp.Config
{
    /// <summary>
    /// Represents a store for <see cref="ZappConfig"/>.
    /// </summary>
    public class JsonConfigStore : IConfigStore
    {
        private const string configFile = "zapp-config.json";

        private readonly ILog logService;

        private string filePath;

        /// <summary>
        /// Represents the lazy for the zapp-config.
        /// </summary>
        public Lazy<ZappConfig> Lazy { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="JsonConfigStore"/>.
        /// </summary>
        /// <param name="logService">Service used for logging.</param>
        public JsonConfigStore(ILog logService)
        {
            this.logService = logService;

            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile);

            Lazy = new Lazy<ZappConfig>(() => Resolve());
        }

        private ZappConfig Resolve()
        {
            if (!File.Exists(filePath))
            {
                return Prefab();
            }

            string content = File.ReadAllText(filePath);

            return JsonConvert.DeserializeObject<ZappConfig>(content);
        }

        private ZappConfig Prefab()
        {
            var cfg = new ZappConfig();

            logService.Warn($"{filePath} was not found, created pre-fab file.");

            string content = JsonConvert.SerializeObject(cfg, Formatting.Indented);

            File.WriteAllText(filePath, content);

            return cfg;
        }
    }
}
