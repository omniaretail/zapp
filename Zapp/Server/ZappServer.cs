using log4net;
using System;
using Zapp.Pack;
using Zapp.Rest;
using Zapp.Sync;

namespace Zapp.Server
{
    /// <summary>
    /// Represents a class which controls all the services.
    /// </summary>
    public class ZappServer : IZappServer
    {
        private readonly ILog logService;

        private readonly IRestService apiService;
        private readonly ISyncService syncService;

        private readonly IPackService packService;

        /// <summary>
        /// Initializes a new <see cref="ZappServer"/>.
        /// </summary>
        /// <param name="logService">Service used for logging.</param>
        /// <param name="apiService">Service used for outside communcation.</param>
        /// <param name="syncService">Service used for package-version synchronization.</param>
        /// <param name="packService">Service used for packages.</param>
        public ZappServer(
            ILog logService,
            IRestService apiService,
            ISyncService syncService,
            IPackService packService)
        {
            this.logService = logService;

            this.apiService = apiService;
            this.syncService = syncService;

            this.packService = packService;
        }

        /// <summary>
        /// Starts the current instance of <see cref="ZappServer"/> and it's dependencies.
        /// </summary>
        public void Start()
        {
            try
            {
                syncService.Connect();
                apiService.Listen();
            }
            catch (Exception ex)
            {
                logService.Fatal("failed to start the server instance", ex);

                throw;
            }
        }
    }
}
