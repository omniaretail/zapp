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
        /// Represents the package's mode.
        /// </summary>
        PackageMode Mode { get; }

        /// <summary>
        /// Represents if the package is executable.
        /// </summary>
        bool IsExecutable { get; }

        /// <summary>
        /// Represents all the entries (files) in the package.
        /// </summary>
        IReadOnlyCollection<string> Entries { get; }

        /// <summary>
        /// Reads a entry from the package.
        /// </summary>
        /// <param name="name">Name of entry to read.</param>
        Stream GetEntry(string name);

        /// <summary>
        /// Writes a entry from the package.
        /// </summary>
        /// <param name="name">Name of entry to write.</param>
        /// <param name="stream">Stream of entry to write.</param>
        void AddEntry(string name, Stream stream);
    }
}
