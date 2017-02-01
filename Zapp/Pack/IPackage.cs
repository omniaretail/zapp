using System.Collections.Generic;
using System.IO;

namespace Zapp.Pack
{
    /// <summary>
    /// Represents an interface which defines the content of a package.
    /// </summary>
    public interface IPackage
    {
        /// <summary>
        /// Represents a collection of all containing entries.
        /// </summary>
        IReadOnlyCollection<string> Entries { get; }

        /// <summary>
        /// Reads a entry from the package.
        /// </summary>
        /// <param name="name">Name of entry to read.</param>
        Stream ReadEntry(string name);
    }
}
