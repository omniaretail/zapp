namespace Zapp.Pack
{
    /// <summary>
    /// Represents an interface for managing packages.
    /// </summary>
    public interface IPackService
    {
        /// <summary>
        /// Deploys the new package.
        /// </summary>
        /// <param name="packageId">Identity of the package.</param>
        /// <param name="deployVersion">Deploy version of the package.</param>
        PackDeployResult Deploy(string packageId, string deployVersion);
    }
}