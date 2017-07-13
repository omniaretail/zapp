﻿using log4net;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Zapp.Deploy;
using Zapp.Pack;
using Zapp.Sync;

namespace Zapp.Rest.Controllers
{
    /// <summary>
    /// Represents a class which contains clerk-related actions.
    /// </summary>
    public class ClerkController : ApiController
    {
        private readonly ILog logService;

        private readonly ISyncService syncService;
        private readonly IDeployService deployService;

        /// <summary>
        /// Initializes a new <see cref="ClerkController"/>.
        /// </summary>
        /// <param name="logService">Service used for logging.</param>
        /// <param name="syncService">Service used to synchronize the packages.</param>
        /// <param name="deployService">Service used for announcing deployments.</param>
        public ClerkController(
            ILog logService,
            ISyncService syncService,
            IDeployService deployService)
        {
            this.logService = logService;

            this.syncService = syncService;
            this.deployService = deployService;
        }

        /// <summary>
        /// Announces the new deployed version of a package.
        /// </summary>
        /// <param name="packageId">Identity of the package.</param>
        /// <param name="deployVersion">Deploy version of the package.</param>
        /// <param name="token">Token that is used to keep track of cancelled requests.</param>
        [HttpGet, Route("api/clerk/announce/{packageId}/{deployVersion}")]
        public async Task<StatusCodeResult> Announce(
            string packageId,
            string deployVersion,
            CancellationToken token)
        {
            var versions = new[] { new PackageVersion(packageId, deployVersion) };

            return await Announce(versions, token);
        }

        /// <summary>
        /// Announces a new collection of package versions.
        /// </summary>
        /// <param name="versions">Collection of package versions.</param>
        /// <param name="token">Token that is used to keep track of cancelled requests.</param>        
        [HttpPost, Route("api/clerk/announce/")]
        public async Task<StatusCodeResult> Announce(
            [FromBody]IReadOnlyCollection<PackageVersion> versions,
            CancellationToken token)
        {
            try
            {
                await deployService.AnnounceAsync(versions, token);
            }
            catch (Exception ex)
            {
                logService.Fatal("Announcement failed.", ex);
                throw;
            }

            return StatusCode(HttpStatusCode.OK);
        }

        /// <summary>
        /// Publishes a new collection of package versions.
        /// </summary>
        /// <param name="versions">Collection of package versions.</param>
        /// <param name="token">Token that is used to keep track of cancelled requests.</param>        
        [HttpPost, Route("api/clerk/publish/")]
        public async Task<StatusCodeResult> Publish(
            [FromBody]IReadOnlyCollection<PackageVersion> versions,
            CancellationToken token)
        {
            try
            {
                await deployService.PublishAsync(versions, token);
            }
            catch (Exception ex)
            {
                logService.Fatal("Publication failed.", ex);
                throw;
            }

            return StatusCode(HttpStatusCode.OK);
        }
    }
}
