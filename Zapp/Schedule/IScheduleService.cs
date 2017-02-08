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
    }
}
