using log4net;
using Newtonsoft.Json;
using System;
using System.IO;
using Zapp.Perspectives;

namespace Zapp.Config
{
    /// <summary>
    /// Represents a store for <see cref="ZappConfig"/>.
    /// </summary>
    public class JsonConfigStore : IConfigStore
    {
        private const string configFile = "zapp-config.json";

        private readonly ILog logService;
        private readonly IFile file;

        private string filePath;
        private Lazy<ZappConfig> lazy;

        /// <summary>
        /// Represents the value for the zapp-config.
        /// </summary>
        /// <inheritdoc />
        public ZappConfig Value => lazy.Value;

        /// <summary>
        /// Initializes a new <see cref="JsonConfigStore"/>.
        /// </summary>
        /// <param name="logService">Service used for logging.</param>
        /// <param name="file">Util used for file operations.</param>
        public JsonConfigStore(
            ILog logService,
            IFile file)
        {
            this.logService = logService;
            this.file = file;

            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile);

            lazy = new Lazy<ZappConfig>(() => Resolve());
        }

        private ZappConfig Resolve()
        {
            if (!file.Exists(filePath))
            {
                return Prefab();
            }

            string content = file.ReadAllText(filePath);

            return JsonConvert.DeserializeObject<ZappConfig>(content);
        }

        private ZappConfig Prefab()
        {
            var cfg = new ZappConfig();

            logService.Warn($"{filePath} was not found, created pre-fab file.");

            string content = JsonConvert.SerializeObject(cfg, Formatting.Indented);

            file.WriteAllText(filePath, content);

            return cfg;
        }
    }
}
