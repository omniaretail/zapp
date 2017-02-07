using System.Collections.Generic;

namespace Zapp.Pack
{
    /// <summary>
    /// Represents an interface for managing packages.
    /// </summary>
    public interface IPackService
    {
        /// <summary>
        /// Loads the package.
        /// </summary>
        /// <param name="version">Version of the package.</param>
        IPackage LoadPackage(PackageVersion version);

        /// <summary>
        /// Searches for affected fusion packages.
        /// </summary>
        /// <param name="packageId">Identity of the package.</param>
        IReadOnlyCollection<string> GetAffectedFusions(string packageId);

        /// <summary>
        /// Deploys the new package.
        /// </summary>
        /// <param name="version">Version of the package.</param>
        PackDeployResult Deploy(PackageVersion version);
    }
}