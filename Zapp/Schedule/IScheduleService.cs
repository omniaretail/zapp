using System.Collections.Generic;

namespace Zapp.Schedule
{
    /// <summary>
    /// Represents an interface mainly used to schedule fusions.
    /// </summary>
    public interface IScheduleService
    {
        /// <summary>
        /// Represents the current running processes.
        /// </summary>
        IReadOnlyCollection<IFusionProcess> Processes { get; }

        /// <summary>
        /// Schedules all the configured fusions.
        /// </summary>
        void ScheduleAll();

        /// <summary>
        /// Schedules a collection of fusions
        /// </summary>
        /// <param name="fusionIds">Identities of the fusions.</param>
        void ScheduleMultiple(IEnumerable<string> fusionIds);

        /// <summary>
        /// Announces a running app on which rest port it's bound.
        /// </summary>
        /// <param name="fusionId">Identity of the fusion.</param>
        /// <param name="port">Port of the fusions' rest service.</param>
        void Announce(string fusionId, int port);
    }
}
