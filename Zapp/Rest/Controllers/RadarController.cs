using System.Net;
using System.Web.Http;
using System.Web.Http.Results;
using Zapp.Schedule;

namespace Zapp.Rest.Controllers
{
    /// <summary>
    /// Represents a controller which is used to announce processes.
    /// </summary>
    public class RadarController : ApiController
    {
        private readonly IScheduleService scheduleService;

        /// <summary>
        /// Initializes a new <see cref="RadarController"/>.
        /// </summary>
        /// <param name="scheduleService">Service used for scheduling fusions.</param>
        public RadarController(
            IScheduleService scheduleService)
        {
            this.scheduleService = scheduleService;
        }

        /// <summary>
        /// Announces the fusion to the zapp server.
        /// </summary>
        /// <param name="fusionId">Identity of the fusion.</param>
        /// <param name="port">Port of the fusion.</param>
        [HttpGet, HttpPost, Route("api/radar/announce/{fusionId}/{port}")]
        public StatusCodeResult Announce(string fusionId, int port)
        {
            scheduleService.Announce(fusionId, port);

            return StatusCode(HttpStatusCode.OK);
        }
    }
}
