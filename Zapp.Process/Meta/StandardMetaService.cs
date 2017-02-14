using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Zapp.Core;

namespace Zapp.Process.Meta
{
    /// <summary>
    /// Represents an implementation of <see cref="IMetaService"/>.
    /// </summary>
    public class StandardMetaService : IMetaService
    {
        private string filePath;
        private IDictionary<string, string> info;

        /// <summary>
        /// Initializes a new <see cref="StandardMetaService"/>.
        /// </summary>
        public StandardMetaService()
        {
            filePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                ZappVariables.FusionMetaEntyName
            );
        }

        /// <summary>
        /// Loads the meta information.
        /// </summary>
        /// <inheritdoc />
        public void Load()
        {
            info = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                File.ReadAllText(filePath));
        }

        /// <summary>
        /// Gets the value of a meta key.
        /// </summary>
        /// <param name="key">Key of the meta info.</param>
        /// <inheritdoc />
        public string GetValue(string key) => info[key];
    }
}
