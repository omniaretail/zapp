using EnsureThat;
using System.IO;
using System.Xml.Linq;
using Zapp.Config;

namespace Zapp.Transform
{
    /// <summary>
    /// Represents an implementation of <see cref="ITransformConfig"/> for xml-configurations.
    /// </summary>
    public class XmlTransformConfig : ITransformConfig
    {
        private readonly IConfigStore configStore;

        /// <summary>
        /// Initializes a new <see cref="XmlTransformConfig"/>.
        /// </summary>
        /// <param name="configStore">Store used for getting configuration.</param>
        public XmlTransformConfig(IConfigStore configStore)
        {
            this.configStore = configStore;
        }

        /// <summary>
        /// Transforms the given input config stream.
        /// </summary>
        /// <param name="input">Stream of the config to transform</param>
        /// <param name="output">Stream of the transformed config.</param>
        /// <inheritdoc />
        public void Transform(Stream input, Stream output)
        {
            EnsureArg.IsNotNull(input, nameof(input));
            EnsureArg.IsNotNull(output, nameof(output));

            var config = configStore.Value?.Fuse ?? new FuseConfig();
            var document = XDocument.Load(input);
            var runtimeElement = document.Root.Element("runtime");

            runtimeElement.AddFirst(CreateElement("gcAllowVeryLargeObjects", config.IsGcVeryLargeObjectsAllowed));
            runtimeElement.AddFirst(CreateElement("gcConcurrent", config.IsGcConcurrentEnabled));
            runtimeElement.AddFirst(CreateElement("gcServer", config.IsGcServerEnabled));
            runtimeElement.AddFirst(CreateElement("loadFromRemoteSources", config.IsLoadFromRemoteSourcesEnabled));

            document.Save(output);
        }

        private XElement CreateElement(string name, bool? isEnabled)
        {
            var element = new XElement(name);
            element.Add(new XAttribute("enabled", isEnabled));
            return element;
        }
    }
}
