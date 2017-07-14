using System.Collections.Generic;

namespace Zapp.Pack
{
    /// <summary>
    /// Represents a interface for handling package entries.
    /// </summary>
    public interface IPackage
    {
        /// <summary>
        /// Represents the version of the package.
        /// </summary>
        PackageVersion Version { get; }

        /// <summary>
        /// Get the entries of the package.
        /// </summary>
        IEnumerable<IPackageEntry> GetEntries();
    }
}
