using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Zapp.Hospital;

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
        /// Represents the state of the fusion.
        /// </summary>
        FusionProcessState State { get; }

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
        /// Announces the port of the process.
        /// </summary>
        /// <param name="port">Port that was received from the process.</param>
        void Announce(int port);

        /// <summary>
        /// Spawns the actual process for this fusion.
        /// </summary>
        void Spawn();

        /// <summary>
        /// Runs the startup event on the process.
        /// </summary>
        /// <param name="token">Token used to cancel the http request.</param>
        Task StartupAsync(CancellationToken token);

        /// <summary>
        /// Runs the terminate event on the process.
        /// </summary>
        /// <param name="token">Token used to cancel the http request.</param>
        Task TerminateAsync(CancellationToken token);

        /// <summary>
        /// Requests the NurseController for the patient statusses.
        /// </summary>
        /// <param name="patientPattern">Pattern used to query patients.</param>
        /// <param name="token">Token used to cancel the http request.</param>
        Task<IEnumerable<PatientStatus>> NurseStatusAsync(string patientPattern, CancellationToken token);

        /// <summary>
        /// Gets the error output for a dead or exited process
        /// </summary>
        /// <inheritDoc />
        string GetErrorOutput();

        /// <summary>
        /// Called when the interceptors are informed.
        /// </summary>
        void OnInterceptorsInformed();

        /// <summary>
        /// Called when a respawn has been failed.
        /// </summary>
        void OnRespawnStartupFailed();
    }
}
