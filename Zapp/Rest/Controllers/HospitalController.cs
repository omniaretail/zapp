using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Zapp.Hospital;
using Zapp.Schedule;

namespace Zapp.Rest.Controllers
{
    /// <summary>
    /// Represents a controller responsible for all Zapp.Hospital info.
    /// </summary>
    public class HospitalController : ApiController
    {
        private readonly IHospitalService hospitalService;

        /// <summary>
        /// Initializes a new <see cref="HospitalController"/> with it's depenedencies.
        /// </summary>
        /// <param name="hospitalService">Service used for quering aggregating hospital causes.</param>
        public HospitalController(
            IHospitalService hospitalService)
        {
            this.hospitalService = hospitalService;
        }

        /// <summary>
        /// Fetches a set of <see cref="PatientStatus"/> for specific set of <see cref="IPatient"/> instances.
        /// </summary>
        /// <param name="fusionPattern">Pattern that is used to query <see cref="IFusionProcess"/> instances.</param>
        /// <param name="patientPattern">Pattern that is used to query <see cref="IPatient"/> instances.</param>
        /// <param name="token">Token used to handle cancelled requests.</param>
        [HttpGet, Route("api/hospital/status")]
        public async Task<HospitalStatus> Status(
            CancellationToken token,
            string fusionPattern = "*",
            string patientPattern = "*")
        {
            return await hospitalService
                .GetStatusAsync(fusionPattern, patientPattern, token);
        }
    }
}
