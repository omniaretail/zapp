using Zapp.Config;
using Zapp.Pack;

namespace Zapp.Fuse
{
    /// <summary>
    /// Represents an interface for filters what apply when a fusion is happening.
    /// </summary>
    public interface IFusionFilter
    {
        /// <summary>
        /// Invoked before a new entry is added to a fusion.
        /// </summary>
        /// <param name="config">Config that describes the fusion package.</param>
        /// <param name="entry">Entry that will be added to the fusion.</param>
        void BeforeAddEntry(FusePackConfig config, IPackageEntry entry);
    }
}
