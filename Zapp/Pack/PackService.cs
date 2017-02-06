using AntPathMatching;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using Zapp.Config;
using Zapp.Sync;

namespace Zapp.Pack
{
    /// <summary>
    /// Represents a implementation of <see cref="IPackService"/> to handle all package related actions.
    /// </summary>
    public class PackService : IPackService
    {
        private readonly ILog logService;

        private readonly ISyncService syncService;
        private readonly IConfigStore configStore;

        private readonly IAntFactory antFactory;
        private readonly IAntDirectoryFactory antDirectoryFactory;

        private string packageRootDir;

        /// <summary>
        /// Initializes a new <see cref="PackService"/>.
        /// </summary>
        /// <param name="logService">Service for logging.</param>
        /// <param name="syncService">Service for synchronizing package information.</param>
        /// <param name="configStore">Store for loading <see cref="ZappConfig"/>.</param>
        /// <param name="antFactory">Factory for creating <see cref="IAnt"/> instances.</param>
        /// <param name="antDirectoryFactory">Factory for creating <see cref="IAntDirectory"/> instances.</param>
        public PackService(
            ILog logService,
            ISyncService syncService,
            IConfigStore configStore,
            IAntFactory antFactory,
            IAntDirectoryFactory antDirectoryFactory)
        {
            this.logService = logService;

            this.syncService = syncService;
            this.configStore = configStore;

            this.antFactory = antFactory;
            this.antDirectoryFactory = antDirectoryFactory;

            packageRootDir = GetPackageRootDirectory();

            // todo: packageFactory
        }

        /// <summary>
        /// Loads a specific package.
        /// </summary>
        /// <param name="version">Version of the package.</param>
        /// <exception cref="ArgumentNullException">Throw when <paramref name="version"/> is not set.</exception>
        /// <exception cref="PackageException">Throw when <paramref name="version"/> is not found.</exception>
        /// <inheritdoc />
        public IPackage LoadPackage(PackageVersion version)
        {
            if (version == null) throw new ArgumentNullException(nameof(version));

            var packageLocation = LocatePackage(version);

            if (string.IsNullOrEmpty(packageLocation))
            {
                throw new PackageException("Not found.", version);
            }

            return null;
        }

        /// <summary>
        /// Searches for affected fusion packages.
        /// </summary>
        /// <param name="packageId">Identity of the package.</param>
        /// <inheritdoc />
        public IReadOnlyCollection<string> GetAffectedFusions(string packageId)
        {
            if (string.IsNullOrEmpty(packageId)) throw new ArgumentException("Must be non-empty.", nameof(packageId));

            var fusions = configStore.Value.Pack.Fusions;

            return fusions
                .Where(f => f.PackageIds.Contains(packageId, StringComparer.OrdinalIgnoreCase))
                .Select(f => f.Id)
                .ToList();
        }

        /// <summary>
        /// Deploys the new package.
        /// </summary>
        /// <param name="packageId">Identity of the package.</param>
        /// <param name="deployVersion">Deploy version of the package.</param>
        /// <inheritdoc />
        [Obsolete("moved to deployservice")]
        public PackDeployResult Deploy(string packageId, string deployVersion)
        {
            var package = LocatePackage(new PackageVersion(packageId, deployVersion));

            if (string.IsNullOrEmpty(package))
            {
                return PackDeployResult.PackageNotFound;
            }

            bool isSynced = syncService
                .SetPackageDeployVersion(packageId, deployVersion);

            if (!isSynced)
            {
                return PackDeployResult.SyncFailed;
            }

            return PackDeployResult.Success;
        }

        private string GetPackageRootDirectory() =>
            configStore.Value?.Pack?.RootDirectory?
                .Replace("zappDir", AppDomain.CurrentDomain.BaseDirectory);

        private string GetPackagePattern(string packageId, string deployVersion) =>
            configStore.Value?.Pack?.PackagePattern?
                .Replace("packageId", packageId)?
                .Replace("deployVersion", deployVersion);

        private string LocatePackage(PackageVersion version)
        {
            var pattern = GetPackagePattern(
                version.PackageId,
                version.DeployVersion
            );

            if (string.IsNullOrEmpty(pattern)) return null;

            var matcher = antFactory.CreateNew(pattern);
            var directorySearcher = antDirectoryFactory.CreateNew(matcher);

            var results = directorySearcher
                .SearchRecursively(packageRootDir, true)?
                .ToList();

            return (results?.Any() != true || results?.Count != 1)
                ? null
                : results.SingleOrDefault();
        }
    }
}
