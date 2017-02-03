namespace Zapp.Pack
{
    /// <summary>
    /// Represents a enum to declare the result of a package deployment.
    /// </summary>
    public enum PackDeployResult
    {
        /// <summary>
        /// Indicates that the deployment was successful.
        /// </summary>
        Success = 0,

        /// <summary>
        /// Indicates that the package was not found.
        /// </summary>
        PackageNotFound = 1,

        /// <summary>
        /// Indicates that the version sync failed.
        /// </summary>
        SyncFailed = 2
    }
}
