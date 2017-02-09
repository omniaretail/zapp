using Newtonsoft.Json;
using System;
using Zapp.Pack;
using Zapp.Core.Clauses;

namespace Zapp.Config
{
    /// <summary>
    /// Represents the configuration for finding the packages.
    /// </summary>
    public class PackConfig
    {
        /// <summary>
        /// Represents the root folder for the packs.
        /// </summary>
        [JsonProperty("rootDir")]
        public string RootDirectory { get; set; } = "{zappDir}/pack/";

        /// <summary>
        /// Represents the pattern for finding packages.
        /// </summary>
        [JsonProperty("packagePattern")]
        public string PackagePattern { get; set; } = "containers-{deployVersion}-dev/{packageId}*.{nupkg,zip}";

        /// <summary>
        /// Resolves the actual root directory.
        /// </summary>
        public string GetActualRootDirectory() => RootDirectory
            .Replace("{zappDir}", AppDomain.CurrentDomain.BaseDirectory);

        /// <summary>
        /// Resolves the actual fusion directory.
        /// </summary>
        /// <param name="version">Version of the package.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="version"/> is not set.</exception>
        public string GetActualPackagePattern(PackageVersion version)
        {
            Guard.ParamNotNull(version, nameof(version));

            return PackagePattern
                .Replace("{packageId}", version.PackageId)
                .Replace("{deployVersion}", version.DeployVersion);
        }
    }
}
