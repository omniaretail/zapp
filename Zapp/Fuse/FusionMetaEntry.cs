using EnsureThat;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Zapp.Core;

namespace Zapp.Fuse
{
    /// <summary>
    /// Represents an implementation of <see cref="IFrameworkPackageEntry"/> for fusion meta info.
    /// </summary>
    public class FusionMetaEntry : IFrameworkPackageEntry
    {
        /// <summary>
        /// Represents the key of the executable info.
        /// </summary>
        public const string ExecutableInfoKey = "executable.file.name";

        private IDictionary<string, string> info;

        /// <summary>
        /// Represents the name of the entry.
        /// </summary>
        /// <inheritdoc />
        public string Name
        {
            get { return ZappVariables.FusionMetaEntyName; }
            set { throw new NotSupportedException("Not allowed to rename this entry."); }
        }

        /// <summary>
        /// Initializes a new <see cref="FusionMetaEntry"/>.
        /// </summary>
        public FusionMetaEntry()
        {
            info = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            SetInfo(ExecutableInfoKey, FusionProcessEntry.DefaultEntryName);
        }

        /// <summary>
        /// Sets information to the entry.
        /// </summary>
        /// <param name="key">Key of the information.</param>
        /// <param name="value">Value of the information.</param>
        /// <exception cref="ArgumentException">Throw when <paramref name="key"/> is not set.</exception>
        public void SetInfo(string key, string value)
        {
            EnsureArg.IsNotNullOrEmpty(key, nameof(key));

            info[key] = value;
        }

        /// <summary>
        /// Opens the entry with a streamed content.
        /// </summary>
        /// <inheritdoc />
        public Stream Open()
        {
            return new MemoryStream(Encoding.UTF8
                .GetBytes(JsonConvert.SerializeObject(info, Formatting.Indented)));
        }
    }
}
