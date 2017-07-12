using EnsureThat;
using log4net;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zapp.Config;
using Zapp.Deploy;
using Zapp.Exceptions;
using Zapp.Extensions;
using Zapp.Pack;

namespace Zapp.Sync
{
    /// <summary>
    /// Represents a class which is responsible for synchronizing specific parts of zapp.
    /// </summary>
    public sealed class RedisSyncService : ISyncService, IDisposable
    {
        private readonly ILog logService;
        private readonly IConfigStore configStore;
        private readonly IConnectionMultiplexerFactory multiplexerFactory;

        private IConnectionMultiplexer multiplexer;

        /// <summary>
        /// Initializes a new <see cref="RedisSyncService"/>.
        /// </summary>
        /// <param name="logService">Service used for logging.</param>
        /// <param name="configStore">Configuration storage instance.</param>
        /// <param name="multiplexerFactory">Facory that is able to create instances of <see cref="IConnectionMultiplexer"/>.</param>
        public RedisSyncService(
            ILog logService,
            IConfigStore configStore,
            IConnectionMultiplexerFactory multiplexerFactory)
        {
            this.logService = logService;
            this.configStore = configStore;
            this.multiplexerFactory = multiplexerFactory;
        }

        /// <summary>
        /// Connects the service with the sync-server.
        /// </summary>
        public void Connect()
        {
            var config = configStore.Value.Sync;

            multiplexer = multiplexerFactory.CreateNew(config.ConnectionString);

            logService.Info($"Connected to redis on: '{config.ConnectionString}'.");
        }

        /// <summary>
        /// Gets if the database is empty.
        /// </summary>
        /// <inheritDoc />
        public async Task<bool> IsEmptyAsync()
        {
            var randomKey = await multiplexer
                .GetDatabase()
                .KeyRandomAsync();

            return randomKey == default(RedisKey);
        }

        /// <summary>
        /// Get the latest version stored in the server of the requested package.
        /// </summary>
        /// <param name="packageId">Identity of the package.</param>
        /// <inheritDoc />
        public async Task<string> GetVersionAsync(string packageId)
        {
            EnsureArg.IsNotNullOrEmpty(packageId, nameof(packageId));

            return await multiplexer
                .GetDatabase()
                .StringGetAsync($"package:{packageId}");
        }

        /// <summary>
        /// Announces the versions to the server.
        /// </summary>
        /// <param name="announcement">Announcement that needs to be synchronized.</param>
        /// <param name="token">Token used to cancel the announcement.</param>
        /// <inheritDoc />
        public async Task AnnounceAsync(IDeployAnnouncement announcement, CancellationToken token)
        {
            EnsureArg.IsNotNull(announcement, nameof(announcement));

            var newPackageVersions = announcement
                .GetNewPackageVersions()
                .Stale();

            if (!newPackageVersions.Any())
            {
                return;
            }

            foreach (var version in newPackageVersions)
            {
                token.ThrowIfCancellationRequested();

                await AnnounceAsync(version);
            }
        }

        private async Task AnnounceAsync(PackageVersion version)
        {
            EnsureArg.IsNotNull(version, nameof(version));

            var isStored = await multiplexer
                .GetDatabase()
                .StringSetAsync($"package:{version.PackageId}", version.DeployVersion);

            if (!isStored)
            {
                throw new SyncException(SyncException.AnnounceFailure, version);
            }
        }

        /// <summary>
        /// Releases all resourced used by the <see cref="RedisSyncService"/> instance.
        /// </summary>
        public void Dispose()
        {
            multiplexer?.Dispose();
            multiplexer = null;
        }
    }
}
