using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
        /// <param name="token">Token of cancellation.</param>
        Task DrainAsync(IEnumerable<IFusionProcess> processes, CancellationToken token);

        /// <summary>
        /// Resumes the draining process after all processes are scheduled.
        /// </summary>
        void Resume();
    }
}
