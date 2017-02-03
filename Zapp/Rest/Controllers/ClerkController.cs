using System.Web.Http;
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
        /// <param name="syncService">Service used for sync'ing deployments.</param>
        public ClerkController(ISyncService syncService)
        {
            this.syncService = syncService;
        }

        /// <summary>
        /// Http method for announcing new deployment versions for packages.
        /// </summary>
        /// <param name="packageId">Identity of the package.</param>
        /// <param name="deployVersion">Deploy version of the package.</param>
        [HttpGet, HttpPost, Route("api/clerk/deploy/{packageId}/{deployVersion}")]
        public bool Deploy(string packageId, string deployVersion)
        {
            return syncService.SetPackageDeployVersion(packageId, deployVersion);
        }
    }
}
