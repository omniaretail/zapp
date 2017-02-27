using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Web.Http.Results;
using Zapp.Deploy;
using Zapp.Pack;

namespace Zapp.Rest.Controllers
{
    /// <summary>
    /// Represents a class which contains clerk-related actions.
    /// </summary>
    public class ClerkController : ApiController
    {
        private readonly IDeployService deployService;

        /// <summary>
        /// Initializes a new <see cref="ClerkController"/>.
        /// </summary>
        /// <param name="deployService">Service used for announcing deployments.</param>
        public ClerkController(
            IDeployService deployService)
        {
            this.deployService = deployService;
        }

        /// <summary>
        /// Announces the new deployed version of a package.
        /// </summary>
        /// <param name="packageId">Identity of the package.</param>
        /// <param name="deployVersion">Deploy version of the package.</param>
        [HttpGet, Route("api/clerk/announce/{packageId}/{deployVersion}")]
        public StatusCodeResult Announce(string packageId, string deployVersion)
        {
            return Announce(new[]
            {
                new PackageVersion(packageId, deployVersion)
            });
        }

        /// <summary>
        /// Announces a new collection of package versions.
        /// </summary>
        /// <param name="versions">Collection of package versions.</param>
        [HttpPost, Route("api/clerk/announce/")]
        public StatusCodeResult Announce([FromBody]IReadOnlyCollection<PackageVersion> versions)
        {
            var announceResult = deployService.Announce(versions);

            switch (announceResult)
            {
                case AnnounceResult.Ok:
                    return StatusCode(HttpStatusCode.OK);
                case AnnounceResult.NotFound:
                    return StatusCode(HttpStatusCode.NotFound);
                default:
                case AnnounceResult.InternalError:
                    return StatusCode(HttpStatusCode.InternalServerError);
            }
        }
    }
}
