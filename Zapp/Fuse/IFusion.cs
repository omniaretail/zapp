using Zapp.Pack;

namespace Zapp.Fuse
{
    /// <summary>
    /// Represents an interface for fusion packages.
    /// </summary>
    public interface IFusion
    {
        /// <summary>
        /// Adds an entry to the fusion package.
        /// </summary>
        /// <param name="entry">Entry to add.</param>
        void AddEntry(IPackageEntry entry);
    }
}
