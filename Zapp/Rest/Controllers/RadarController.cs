using log4net;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly ILog logService;
        private readonly IScheduleService scheduleService;

        /// <summary>
        /// Initializes a new <see cref="RadarController"/>.
        /// </summary>
        /// <param name="logService">Service used for logging.</param>
        /// <param name="scheduleService">Service used for scheduling fusions.</param>
        public RadarController(
            ILog logService,
            IScheduleService scheduleService)
        {
            this.logService = logService;
            this.scheduleService = scheduleService;
        }

        /// <summary>
        /// Announces the fusion to the zapp server.
        /// </summary>
        /// <param name="fusionId">Identity of the fusion.</param>
        /// <param name="port">Port of the fusion.</param>
        /// <param name="token">Token of cancellation.</param>
        [HttpGet, HttpPost, Route("api/radar/announce/{fusionId}/{port}")]
        public async Task<StatusCodeResult> Announce(string fusionId, int port, CancellationToken token)
        {
            try
            {
                await scheduleService.AnnounceAsync(fusionId, port, token);
            }
            catch (Exception ex)
            {
                logService.Error("Process announcement failed.", ex);
                throw;
            }

            return StatusCode(HttpStatusCode.OK);
        }
    }
}
