using System.IO;

namespace Zapp.Pack
{
    /// <summary>
    /// Represents and interface for entries in a <see cref="IPackage"/>.
    /// </summary>
    public interface IPackageEntry
    {
        /// <summary>
        /// Represents the name of the entry.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Opens the entry with a streamed content.
        /// </summary>
        Stream Open();
    }
}
