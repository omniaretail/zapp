using AntPathMatching;
using log4net;
using System;
using System.Linq;
using Zapp.Config;
using Zapp.Sync;

namespace Zapp.Pack
{
    /// <summary>
    /// Represents a class which handles all the package operations.
    /// </summary>
    public class PackService : IPackService
    {
        private readonly ILog logService;
        private readonly ISyncService syncService;
        private readonly IConfigStore configStore;

        private string packageRootDir;

        /// <summary>
        /// Initializes a new <see cref="PackService"/>.
        /// </summary>
        /// <param name="logService">Service used for logging.</param>
        /// <param name="syncService">Service used for synchronization of package deploy versions.</param>
        /// <param name="configStore">Configuration storage instance.</param>
        public PackService(
            ILog logService,
            ISyncService syncService,
            IConfigStore configStore)
        {
            this.logService = logService;
            this.syncService = syncService;
            this.configStore = configStore;

            packageRootDir = GetPackageRootDirectory();
        }

        /// <summary>
        /// Deploys the new package.
        /// </summary>
        /// <param name="packageId">Identity of the package.</param>
        /// <param name="deployVersion">Deploy version of the package.</param>
        public PackDeployResult Deploy(string packageId, string deployVersion)
        {
            var package = FindPackage(packageId, deployVersion);

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

        private string GetPackageRootDirectory()
        {
            PackConfig config = configStore.Value.Pack;

            return config.RootDirectory
                .Replace("{zappDir}", AppDomain.CurrentDomain.BaseDirectory);
        }

        private string GetPackagePattern(string packageId, string deployVersion)
        {
            PackConfig config = configStore.Value.Pack;

            return config.PackagePattern
                .Replace("{deployVersion}", deployVersion)
                .Replace("{packageId}", packageId);
        }

        private string FindPackage(string packageId, string deployVersion)
        {
            var pattern = GetPackagePattern(packageId, deployVersion);

            var ant = new Ant(pattern);
            var andDir = new AntDirectory(ant);

            var files = andDir.SearchRecursively(packageRootDir);

            return files.FirstOrDefault();
        }
    }
}
