using log4net;
using StackExchange.Redis;
using System;
using Zapp.Config;

namespace Zapp.Sync
{
    /// <summary>
    /// Represents a class which is responsible for synchronizing specific parts of zapp.
    /// </summary>
    public class RedisSyncService : ISyncService, IDisposable
    {
        private readonly ILog logService;
        private readonly IConfigStore configStore;

        private IDatabase database;
        private ConnectionMultiplexer multiplexer;

        /// <summary>
        /// Initializes a new <see cref="RedisSyncService"/>.
        /// </summary>
        /// <param name="logService">Service used for logging.</param>
        /// <param name="configStore">Configuration storage instance.</param>
        public RedisSyncService(
            ILog logService,
            IConfigStore configStore)
        {
            this.logService = logService;
            this.configStore = configStore;
        }

        /// <summary>
        /// Connects the service with the sync-server.
        /// </summary>
        public void Connect()
        {
            // todo: make this test-able.
            SyncConfig config = configStore.Value.Sync;

            multiplexer = ConnectionMultiplexer.Connect(config.ConnectionString);
            database = multiplexer.GetDatabase();

            logService.Info($"connected to: {config.ConnectionString}");
        }

        /// <summary>
        /// Synchronizes the version of the requested package from the server.
        /// </summary>
        /// <param name="packageId">Identity of the package.</param>
        /// <returns>Stored deploy version of the requested package.</returns>
        public string GetPackageDeployVersion(string packageId) => database.StringGet($"package:{packageId}");

        /// <summary>
        /// Synchronizes the version of the requested package to the server.
        /// </summary>
        /// <param name="packageId">Identity of the package.</param>
        /// <param name="deployVersion">Deploy version of the package.</param>
        /// <returns>If the operation was completed or not.</returns>
        public bool SetPackageDeployVersion(string packageId, string deployVersion) => database.StringSet($"package:{packageId}", deployVersion);

        /// <summary>
        /// Releases all resourced used by the <see cref="RedisSyncService"/> instance.
        /// </summary>
        public void Dispose()
        {
            multiplexer?.Dispose();
            multiplexer = null;

            database = null;
        }
    }
}
