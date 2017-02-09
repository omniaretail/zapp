using AntPathMatching;
using log4net;
using System;
using System.IO;
using System.Linq;
using Zapp.Config;
using Zapp.Sync;
using Zapp.Core.Clauses;

namespace Zapp.Pack
{
    /// <summary>
    /// Represents a implementation of <see cref="IPackService"/> to handle all package related actions.
    /// </summary>
    public class FilePackService : IPackService
    {
        private readonly ILog logService;

        private readonly ISyncService syncService;
        private readonly IConfigStore configStore;

        private readonly IAntFactory antFactory;
        private readonly IAntDirectoryFactory antDirectoryFactory;

        private readonly IPackageFactory packageFactory;

        private string packageRootDir;

        /// <summary>
        /// Initializes a new <see cref="FilePackService"/>.
        /// </summary>
        /// <param name="logService">Service for logging.</param>
        /// <param name="syncService">Service for synchronizing package information.</param>
        /// <param name="configStore">Store for loading <see cref="ZappConfig"/>.</param>
        /// <param name="antFactory">Factory for creating <see cref="IAnt"/> instances.</param>
        /// <param name="antDirectoryFactory">Factory for creating <see cref="IAntDirectory"/> instances.</param>
        /// <param name="packageFactory">Factory for creating <see cref="IPackage"/> instances.</param>
        public FilePackService(
            ILog logService,
            ISyncService syncService,
            IConfigStore configStore,
            IAntFactory antFactory,
            IAntDirectoryFactory antDirectoryFactory,
            IPackageFactory packageFactory)
        {
            this.logService = logService;

            this.syncService = syncService;
            this.configStore = configStore;

            this.antFactory = antFactory;
            this.antDirectoryFactory = antDirectoryFactory;

            this.packageFactory = packageFactory;

            packageRootDir = configStore?.Value?.Pack?
                .GetActualRootDirectory();
        }

        /// <summary>
        /// Verifies if the requested package exists.
        /// </summary>
        /// <param name="version">Version of the package.</param>
        public bool IsPackageVersionDeployed(PackageVersion version)
        {
            Guard.ParamNotNull(version, nameof(version));

            return !string.IsNullOrEmpty(LocatePackage(version));
        }

        /// <summary>
        /// Loads a specific package.
        /// </summary>
        /// <param name="version">Version of the package.</param>
        /// <exception cref="ArgumentNullException">Throw when <paramref name="version"/> is not set.</exception>
        /// <inheritdoc />
        public IPackage LoadPackage(PackageVersion version)
        {
            Guard.ParamNotNull(version, nameof(version));

            var packageLocation = LocatePackage(version);

            if (string.IsNullOrEmpty(packageLocation))
            {
                throw new PackageException("Package not found.", version);
            }

            return packageFactory.CreateNew(version, File.OpenRead(packageLocation));
        }

        /// <summary>
        /// Deploys the new package.
        /// </summary>
        /// <param name="version">Version of the package.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="version"/> is not set.</exception>
        /// <inheritdoc />
        public PackDeployResult Deploy(PackageVersion version)
        {
            Guard.ParamNotNull(version, nameof(version));

            var package = LocatePackage(version);

            if (string.IsNullOrEmpty(package))
            {
                return PackDeployResult.PackageNotFound;
            }

            bool isSynced = syncService.Announce(version);

            if (!isSynced)
            {
                return PackDeployResult.SyncFailed;
            }

            return PackDeployResult.Success;
        }

        private string LocatePackage(PackageVersion version)
        {
            var pattern = configStore.Value?.Pack?
                .GetActualPackagePattern(version);

            if (string.IsNullOrEmpty(pattern)) return null;

            var matcher = antFactory.CreateNew(pattern);
            var directorySearcher = antDirectoryFactory.CreateNew(matcher);

            var results = directorySearcher
                .SearchRecursively(packageRootDir, true)?
                .ToList();

            return (results?.Count != 1)
                ? null
                : results.SingleOrDefault();
        }
    }
}
