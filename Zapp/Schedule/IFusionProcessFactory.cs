namespace Zapp.Schedule
{
    /// <summary>
    /// Represents an interface used for instatiating <see cref="IFusionProcess"/>.
    /// </summary>
    public interface IFusionProcessFactory
    {
        /// <summary>
        /// Creates a new <see cref="IFusionProcess"/>.
        /// </summary>
        /// <param name="fusionId">Identity of the fusion.</param>
        IFusionProcess CreateNew(string fusionId);
    }
}
