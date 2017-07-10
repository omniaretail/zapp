namespace Zapp.Fuse
{
    /// <summary>
    /// Represents a maid for the fusion stored as directories on disk.
    /// </summary>
    public interface IFusionMaid
    {
        /// <summary>
        /// Requests the maid to clean the old fusion's directories.
        /// </summary>
        /// <param name="fusionId">The id of the fusion where the old directories needs to becleaned of.</param>
        void CleanAll(string fusionId);
    }
}
