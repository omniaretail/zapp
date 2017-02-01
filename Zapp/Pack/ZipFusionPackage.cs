using System.IO;

namespace Zapp.Pack
{
    /// <summary>
    /// Represents a implementation of <see cref="ZipFusionPackage"/>.
    /// </summary>
    public class ZipFusionPackage : IFusionPackage
    {
        /// <summary>
        /// Initializes a new <see cref="ZipFusionPackage"/>.
        /// </summary>
        public ZipFusionPackage()
        {

        }

        /// <summary>
        /// Opens a new writeable stream for the requested entry.
        /// </summary>
        /// <param name="name">Name of the entry.</param>
        public Stream WriteEntry(string name) => null;
    }
}
