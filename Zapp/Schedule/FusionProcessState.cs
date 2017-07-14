namespace Zapp.Schedule
{
    /// <summary>
    /// Represents a enum that indicates the state of a <see cref="IFusionProcess"/>.
    /// </summary>
    public enum FusionProcessState
    {
        /// <summary>
        /// Indicates that nothing has ever happend on this process.
        /// </summary>
        None = 0,

        /// <summary>
        /// Indicates that the process has been spawned.
        /// </summary>
        Spawned = 1,

        /// <summary>
        /// Indicates that the process has been announced.
        /// </summary>
        Announced = 2,

        /// <summary>
        /// Indicates that the entry point has been started and the interceptors has been run.
        /// </summary>
        Started = 3,

        /// <summary>
        /// Indicates that the process has been exited, but not recovered yet.
        /// </summary>
        Exited = 4,

        /// <summary>
        /// Indicates that the process has been exited, but could not be recovered.
        /// </summary>
        Dead = 5
    }
}
