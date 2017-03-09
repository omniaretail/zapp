using System.Collections.Generic;

namespace Zapp.Fuse
{
    /// <summary>
    /// Represents an interface for creating instances of <see cref="IFrameworkPackageEntry"/>.
    /// </summary>
    public interface IFrameworkPackageEntryFactory
    {
        /// <summary>
        /// Creates a new collection of <see cref="IFrameworkPackageEntry" />.
        /// </summary>
        IEnumerable<IFrameworkPackageEntry> CreateNew();
    }
}
