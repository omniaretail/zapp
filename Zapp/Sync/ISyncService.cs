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
        /// <returns>Stored version of the requested package.</returns>
        string GetPackageDeployVersion(string packageId);

        /// <summary>
        /// Synchronizes the version of the requested package to the server.
        /// </summary>
        /// <param name="packageId">Identity of the package.</param>
        /// <param name="deployVersion">Deploy version of the package.</param>
        /// <returns>If the operation was completed or not.</returns>
        bool SetPackageDeployVersion(string packageId, string deployVersion);
    }
}