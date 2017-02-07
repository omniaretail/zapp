using System.Net;
using System.Web.Http;
using System.Web.Http.Results;
using Zapp.Pack;
using Zapp.Sync;

namespace Zapp.Rest.Controllers
{
    /// <summary>
    /// Represents a controller for controlling packages.
    /// </summary>
    public class ClerkController : ApiController
    {
        private readonly ISyncService syncService;

        /// <summary>
        /// Initializes a new <see cref="ClerkController"/>.
        /// </summary>
        /// <param name="syncService">Service used for syncing package versions.</param>
        public ClerkController(
            ISyncService syncService)
        {
            this.syncService = syncService;
        }

        /// <summary>
        /// Http method for announcing new deployment versions for packages.
        /// </summary>
        /// <param name="packageId">Identity of the package.</param>
        /// <param name="deployVersion">Deploy version of the package.</param>
        [HttpGet, HttpPost, Route("api/clerk/announce/{packageId}/{deployVersion}")]
        public StatusCodeResult Announce(string packageId, string deployVersion)
        {
            var version = new PackageVersion(packageId, deployVersion);

            var statusCode = HttpStatusCode.OK;

            if (!syncService.Announce(version))
            {
                statusCode = HttpStatusCode.InternalServerError;
            }

            return StatusCode(statusCode);
        }
    }
}
