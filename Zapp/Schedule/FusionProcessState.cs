namespace Zapp.Schedule
{
    /// <summary>
    /// Represents a enum that indicates the state of a <see cref="IFusionProcess"/>.
    /// </summary>
    public enum FusionProcessState
    {
        /// <summary>
        /// Indicates that the process is instantiated.
        /// </summary>
        Instantiated = 0,

        /// <summary>
        /// Indicates that the process is spawned.
        /// </summary>
        Spawned = 1,

        /// <summary>
        /// Indicates that the process is running.
        /// </summary>
        Running = 2,

        /// <summary>
        /// Indicates that the process is stoping.
        /// </summary>
        Stopping = 3,
    }
}
