using log4net;
using System;
using System.Web.Http;
using Zapp.Pack;
using Zapp.Rest.Responses;

namespace Zapp.Rest.Controllers
{
    /// <summary>
    /// Represents a controller for controlling packages.
    /// </summary>
    public class ClerkController : ApiController
    {
        private readonly ILog logService;
        private readonly IPackService packService;

        /// <summary>
        /// Initializes a new <see cref="ClerkController"/>.
        /// </summary>
        /// <param name="logService">Service used for logging.</param>
        /// <param name="packService">Service used for managing packages.</param>
        public ClerkController(
            ILog logService,
            IPackService packService)
        {
            this.logService = logService;
            this.packService = packService;
        }

        /// <summary>
        /// Http method for announcing new deployment versions for packages.
        /// </summary>
        /// <param name="packageId">Identity of the package.</param>
        /// <param name="deployVersion">Deploy version of the package.</param>
        [HttpGet, HttpPost, Route("api/clerk/deploy/{packageId}/{deployVersion}")]
        public GenericResponse Deploy(string packageId, string deployVersion)
        {
            var response = new GenericResponse();

            try
            {
                // todo: clean this up! (using status-codes)
                var version = new PackageVersion(packageId, deployVersion);
                var deployResult = packService.Deploy(version);

                switch (deployResult)
                {
                    case PackDeployResult.Success:

                        response.IsSuccess = true;

                        logService.Info($"deployed packageId: {packageId} deployVersion: {deployVersion}");

                        break;
                    default:
                    case PackDeployResult.PackageNotFound:
                    case PackDeployResult.SyncFailed:

                        response.IsSuccess = true;
                        response.Reason = deployResult.ToString();

                        logService.Error($"deployed failed: {response.Reason}");

                        break;
                }

                return response;
            }
            catch (Exception ex)
            {
                logService.Fatal("deployment crashed.", ex);
                throw;
            }
        }
    }
}
