using System.IO;

namespace Zapp.Pack
{
    /// <summary>
    /// Represents a interface which defines a fused package.
    /// </summary>
    public interface IFusionPackage
    {
        /// <summary>
        /// Opens a new writeable stream for the requested entry.
        /// </summary>
        /// <param name="name">Name of the entry.</param>
        Stream WriteEntry(string name);
    }
}
