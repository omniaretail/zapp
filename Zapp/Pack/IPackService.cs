namespace Zapp.Pack
{
    /// <summary>
    /// Represents an interface for managing packages.
    /// </summary>
    public interface IPackService
    {
        /// <summary>
        /// Verifies if the requested package exists.
        /// </summary>
        /// <param name="version">Version of the package.</param>
        bool IsPackageVersionDeployed(PackageVersion version);

        /// <summary>
        /// Loads the package.
        /// </summary>
        /// <param name="version">Version of the package.</param>
        IPackage LoadPackage(PackageVersion version);

        /// <summary>
        /// Deploys the new package.
        /// </summary>
        /// <param name="version">Version of the package.</param>
        PackDeployResult Deploy(PackageVersion version);
    }
}