using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Zapp.Pack;
using Zapp.Utils;

namespace Zapp.Fuse
{
    /// <summary>
    /// Represents an implementation of <see cref="IPackageEntry"/> for fusion meta info.
    /// </summary>
    public class FusionMetaEntry : IPackageEntry
    {
        private const string entryName = "fusion-meta.json";

        private IDictionary<string, string> info;

        /// <summary>
        /// Represents the name of the entry.
        /// </summary>
        /// <inheritdoc />
        public string Name
        {
            get { return entryName; }
            set { throw new NotSupportedException("Not allowed to rename this entry."); }
        }

        /// <summary>
        /// Initializes a new <see cref="FusionMetaEntry"/>.
        /// </summary>
        public FusionMetaEntry()
        {
            info = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            SetInfo("entry.file", FusionProcessEntry.DefaultEntryName);
        }

        /// <summary>
        /// Sets information to the entry.
        /// </summary>
        /// <param name="key">Key of the information.</param>
        /// <param name="value">Value of the information.</param>
        /// <exception cref="ArgumentException">Throw when <paramref name="key"/> is not set.</exception>
        public void SetInfo(string key, string value)
        {
            Guard.ParamNotNullOrEmpty(key, nameof(key));

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
