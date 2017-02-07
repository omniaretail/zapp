using Zapp.Pack;

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
        /// Synchronizes the version of the requested package.
        /// </summary>
        /// <param name="packageId">Identity of the package.</param>
        string Sync(string packageId);

        /// <summary>
        /// Announces the version of the requested package to the server.
        /// </summary>
        /// <param name="version">Version of the package.</param>
        bool Announce(PackageVersion version);
    }
}