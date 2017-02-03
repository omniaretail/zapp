using AntPathMatching;
using log4net;
using System;
using System.Linq;
using Zapp.Config;

namespace Zapp.Pack
{
    /// <summary>
    /// Represents a class which handles all the package operations.
    /// </summary>
    public class PackService
    {
        private readonly ILog logService;
        private readonly IConfigStore configStore;

        private string packageRootDir;

        /// <summary>
        /// Initializes a new <see cref="PackService"/>.
        /// </summary>
        /// <param name="logService">Service used for logging.</param>
        /// <param name="configStore">Configuration storage instance.</param>
        public PackService(
            ILog logService,
            IConfigStore configStore)
        {
            this.logService = logService;
            this.configStore = configStore;

            packageRootDir = GetPackageRootDirectory();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Load()
        {
            logService.Info(FindPackage("ClusterAware.Package.Shared", "0.0.21"));
            logService.Info(FindPackage("ADPSCAS", "0.0.12"));
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
