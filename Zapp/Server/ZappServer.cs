using log4net;
using System;
using Zapp.Rest;

namespace Zapp.Server
{
    /// <summary>
    /// Represents a class which controls all the services.
    /// </summary>
    public class ZappServer : IZappServer
    {
        private readonly ILog logService;
        private readonly IRestService apiService;

        /// <summary>
        /// Initializes a new <see cref="ZappServer"/> with the specified dependencies.
        /// </summary>
        /// <param name="logService">The instance of <see cref="ILog"/> used for logging.</param>
        /// <param name="apiService">The instance of <see cref="IRestService"/> used for web actions.</param>
        public ZappServer(
            ILog logService,
            IRestService apiService)
        {
            this.logService = logService;
            this.apiService = apiService;
        }

        /// <summary>
        /// Starts the current instance of <see cref="ZappServer"/> and it's dependencies.
        /// </summary>
        public void Start()
        {
            try
            {
                apiService.Start();
            }
            catch (Exception ex)
            {
                logService.Fatal("zapp-server failed to start", ex);

                throw;
            }
        }
    }
}
