namespace Zapp.Schedule
{
    /// <summary>
    /// Represents an interface mainly used for communication with the fusion process.
    /// </summary>
    public interface IFusionProcess
    {
        /// <summary>
        /// Represents the state of the process.
        /// </summary>
        FusionProcessState State { get; }

        /// <summary>
        /// Tries to spawn an instance of the process.
        /// </summary>
        bool TrySpawn();

        /// <summary>
        /// Tries to request the process to start.
        /// </summary>
        /// <param name="port">Port where the process is bound onto.</param>
        bool TryRequestStart(int port);

        /// <summary>
        /// Tries to request the process to stop.
        /// </summary>
        bool TryRequestStop();
    }
}
