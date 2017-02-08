namespace Zapp.Deploy
{
    /// <summary>
    /// Represents a enum used for responding to announcements.
    /// </summary>
    public enum AnnounceResult
    {
        /// <summary>
        /// Indicates that the announces was successful.
        /// </summary>
        Ok = 0,

        /// <summary>
        /// Indicates that the specific version is not found.
        /// </summary>
        NotFound = 1,

        /// <summary>
        /// Indicates that there was an error while distributing.
        /// </summary>
        InternalError = 2
    }
}
