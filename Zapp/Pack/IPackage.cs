using System.Collections.Generic;

namespace Zapp.Pack
{
    /// <summary>
    /// Represents an interface which defines the content of a package.
    /// </summary>
    public interface IPackage
    {
        /// <summary>
        /// Represents if the package is executable.
        /// </summary>
        bool IsExecutable { get; }

        /// <summary>
        /// Represents all the entries (files) in the package.
        /// </summary>
        IReadOnlyCollection<string> Entries { get; }
    }
}
