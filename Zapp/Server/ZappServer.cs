using log4net;
using System;
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

        /// <summary>
        /// Initializes a new <see cref="ZappServer"/> with the specified dependencies.
        /// </summary>
        /// <param name="logService">The instance of <see cref="ILog"/> used for logging.</param>
        /// <param name="apiService">The instance of <see cref="IRestService"/> used for web actions.</param>
        /// <param name="syncService">The instance of <see cref="ISyncService"/> used for server synchronization.</param>
        public ZappServer(
            ILog logService,
            IRestService apiService,
            ISyncService syncService)
        {
            this.logService = logService;
            this.apiService = apiService;
            this.syncService = syncService;
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
