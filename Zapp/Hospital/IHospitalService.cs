using System.Threading;
using System.Threading.Tasks;
using Zapp.Schedule;

namespace Zapp.Hospital
{
    /// <summary>
    /// Represents an interface used to query all zapp hospital metrics.
    /// </summary>
    public interface IHospitalService
    {
        /// <summary>
        /// Gets all the statusses of the given query.
        /// </summary>
        /// <param name="fusionPattern">Pattern that is used to query <see cref="IFusionProcess"/> instances.</param>
        /// <param name="patientPattern">Pattern that is used to query <see cref="IPatient"/> instances.</param>
        /// <param name="token">Token to cancel the request.</param>
        Task<HospitalStatus> GetStatusAsync(
            string fusionPattern, 
            string patientPattern, 
            CancellationToken token);
    }
}