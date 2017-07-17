using System.Collections.Generic;
using System.Web.Http;
using Zapp.Hospital;
using Zapp.Process.Hospital;

namespace Zapp.Process.Rest.Controllers
{
    /// <summary>
    /// Represents a controller responsible for all Zapp.Hospital info.
    /// </summary>
    public class NurseController : ApiController
    {
        private readonly IPatientService patientService;

        /// <summary>
        /// Initializes a new <see cref="NurseController"/> with it's depenedencies.
        /// </summary>
        /// <param name="patientService">Service used for quering <see cref="IPatient"/> instances.</param>
        public NurseController(
            IPatientService patientService)
        {
            this.patientService = patientService;
        }

        /// <summary>
        /// Fetches a set of <see cref="PatientStatus"/> for specific set of <see cref="IPatient"/> instances.
        /// </summary>
        /// <param name="patientPattern">Pattern that is used to query <see cref="IPatient"/> instances.</param>
        [HttpGet, Route("api/nurse/status")]
        public IEnumerable<PatientStatus> Status(string patientPattern = "*") 
            => patientService.GetStatusses(patientPattern);
    }
}
