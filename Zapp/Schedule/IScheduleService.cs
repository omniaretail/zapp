using System.Collections.Generic;

namespace Zapp.Schedule
{
    /// <summary>
    /// Represents an interface mainly used to schedule fusions.
    /// </summary>
    public interface IScheduleService
    {
        /// <summary>
        /// Schedules a collection of fusions
        /// </summary>
        /// <param name="fusionIds">Identities of the fusions.</param>
        void Schedule(IReadOnlyCollection<string> fusionIds);

        /// <summary>
        /// Schedules all the configured fusions.
        /// </summary>
        void ScheduleAll();

        /// <summary>
        /// Announces a running app on which rest port it's bound.
        /// </summary>
        /// <param name="fusionId">Identity of the fusion.</param>
        /// <param name="port">Port of the fusions' rest service.</param>
        bool TryAnnounce(string fusionId, int port);
    }
}
