using System.Threading;
using System.Threading.Tasks;
using Zapp.Deploy;

namespace Zapp.Sync
{
    /// <summary>
    /// Represents an interface used for synchronizing data between zapp-servers.
    /// </summary>
    public interface ISyncService
    {
        /// <summary>
        /// Connects the service with the sync-server.
        /// </summary>
        void Connect();

        /// <summary>
        /// Gets if the database is empty.
        /// </summary>
        Task<bool> IsEmptyAsync();

        /// <summary>
        /// Get the latest version stored in the server of the requested package.
        /// </summary>
        /// <param name="packageId">Identity of the package.</param>
        Task<string> GetVersionAsync(string packageId);

        /// <summary>
        /// Publishes the versions to the server.
        /// </summary>
        /// <param name="announcement">Announcement that needs to be published.</param>
        /// <param name="token">Token used to cancel the announcement.</param>
        Task PublishAsync(IDeployAnnouncement announcement, CancellationToken token);
    }
}