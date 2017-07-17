using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zapp.Deploy;

namespace Zapp.Schedule
{
    /// <summary>
    /// Represents an interface mainly used to schedule fusions.
    /// </summary>
    public interface IScheduleService
    {
        /// <summary>
        /// Represents if the service is deploying currently.
        /// </summary>
        bool IsDeploying();

        /// <summary>
        /// Represents the current running processes.
        /// </summary>
        IEnumerable<IFusionProcess> Processes { get; }

        /// <summary>
        /// Schedules all the configured fusions.
        /// </summary>
        /// <param name="token">The token of cancellation.</param>
        Task ScheduleAllAsync(CancellationToken token);

        /// <summary>
        /// Schedules an <see cref="IDeployAnnouncement"/>.
        /// </summary>
        /// <param name="announcement">The announcement that needs to be scheduled.</param>
        /// <param name="token">The token of cancellation.</param>
        Task ScheduleAsync(IDeployAnnouncement announcement, CancellationToken token);

        /// <summary>
        /// Announces a running app on which rest port it's bound.
        /// </summary>
        /// <param name="fusionId">Identity of the fusion.</param>
        /// <param name="port">Port of the fusions' rest service.</param>
        void Announce(string fusionId, int port);
    }
}
