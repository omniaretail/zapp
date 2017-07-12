using FluentValidation;
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
        /// <summary>
        /// Represents the name of the config file.
        /// </summary>
        public const string ConfigFileName = "zapp-config.json";

        private readonly IFile file;
        private readonly ILog logService;
        private readonly IValidator<ZappConfig> configValidator;

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
        /// <param name="configValidator">Validator used to check some config constraints.</param>
        public JsonConfigStore(
            IFile file,
            ILog logService,
            IValidator<ZappConfig> configValidator)
        {
            this.file = file;
            this.logService = logService;
            this.configValidator = configValidator;

            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName);

            lazy = new Lazy<ZappConfig>(() => Resolve());
        }

        private ZappConfig Resolve()
        {
            if (!file.Exists(filePath))
            {
                return Prefab();
            }

            var content = file.ReadAllText(filePath);
            var configFromDisk = JsonConvert.DeserializeObject<ZappConfig>(content);

            configValidator.ValidateAndThrow(configFromDisk);

            return configFromDisk;
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
