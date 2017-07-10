using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zapp.Pack;

namespace Zapp.Deploy
{
    /// <summary>
    /// Represents an interface mainly used for distributing deployments.
    /// </summary>
    public interface IDeployService
    {
        /// <summary>
        /// Announces a new collection of package versions.
        /// </summary>
        /// <param name="versions">Collection of package versions.</param>
        /// <param name="token">Token of cancellation.</param>
        Task AnnounceAsync(IEnumerable<PackageVersion> versions, CancellationToken token);
    }
}
