using System.Collections.Generic;

namespace Zapp.Schedule
{
    /// <summary>
    /// Represents an interface 
    /// </summary>
    public interface IFusionProcessDrainer
    {
        /// <summary>
        /// Drains the requested processes.
        /// </summary>
        /// <param name="processes">Processes that needs to be drained.</param>
        void Drain(IReadOnlyCollection<IFusionProcess> processes);
    }
}
