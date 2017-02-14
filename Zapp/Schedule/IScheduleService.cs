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
        /// <param name="isExtractionRequired">Indicates if extraction is required.</param>
        void Schedule(IReadOnlyCollection<string> fusionIds, bool isExtractionRequired = true);

        /// <summary>
        /// Schedules all the configured fusions.
        /// </summary>
        /// <param name="isExtractionRequired">Indicates if extraction is required.</param>
        void ScheduleAll(bool isExtractionRequired = true);

        /// <summary>
        /// Announces a running app on which rest port it's bound.
        /// </summary>
        /// <param name="fusionId">Identity of the fusion.</param>
        /// <param name="port">Port of the fusions' rest service.</param>
        bool TryAnnounce(string fusionId, int port);
    }
}
