using System.Collections.Generic;
using Zapp.Config;

namespace Zapp.Fuse
{
    /// <summary>
    /// Represents an interface mainly used for fusing packages.
    /// </summary>
    public interface IFusionService
    {
        /// <summary>
        /// Starts to fuse all the packages.
        /// </summary>
        void Start();

        /// <summary>
        /// Tries to fuse the latest possible version of the fusion.
        /// </summary>
        /// <param name="config">Configuration for the fusion</param>
        bool TryFuseLatest(FusePackConfig config);

        /// <summary>
        /// Searches for affected fusion packages.
        /// </summary>
        /// <param name="packageId">Identity of the package.</param>
        IReadOnlyCollection<string> GetAffectedFusions(string packageId);
    }
}
