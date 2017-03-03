using System;
using System.Diagnostics;

namespace Zapp.Schedule
{
    /// <summary>
    /// Represents an interface mainly used for communication with the fusion process.
    /// </summary>
    public interface IFusionProcess
    {
        /// <summary>
        /// Represents the identity of the fusion.
        /// </summary>
        string FusionId { get; }

        /// <summary>
        /// Represents the timestamp when the process started.
        /// </summary>
        DateTime? StartedAt { get; }

        /// <summary>
        /// Represents a custom session implemention.
        /// </summary>
        object Session { get; set; }

        /// <summary>
        /// Peformance counter for cpu.
        /// </summary>
        PerformanceCounter CpuCounter { get; }

        /// <summary>
        /// Peformance counter for memory.
        /// </summary>
        PerformanceCounter MemoryCounter { get; }

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
