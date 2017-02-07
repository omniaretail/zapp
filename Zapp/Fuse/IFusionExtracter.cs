using System.IO;
using Zapp.Config;

namespace Zapp.Fuse
{
    /// <summary>
    /// Represents an interface for extracting fusions.
    /// </summary>
    public interface IFusionExtracter
    {
        /// <summary>
        /// Extracts the stream 
        /// </summary>
        /// <param name="fusionConfig">Configuration of the fusion.</param>
        /// <param name="contentStream">Stream of the fusion.</param>
        void Extract(FusePackConfig fusionConfig, Stream contentStream);
    }
}
