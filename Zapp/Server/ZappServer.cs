using log4net;
using System;
using Zapp.Fuse;
using Zapp.Rest;
using Zapp.Schedule;
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
        private readonly IFusionService fusionService;
        private readonly IScheduleService scheduleService;

        /// <summary>
        /// Initializes a new <see cref="ZappServer"/>.
        /// </summary>
        /// <param name="logService">Service used for logging.</param>
        /// <param name="apiService">Service used for outside communcation.</param>
        /// <param name="syncService">Service used for package-version synchronization.</param>
        /// <param name="fusionService">Service used for extraction fusions.</param>
        /// <param name="scheduleService">Service used for scheduling fusions.</param>
        public ZappServer(
            ILog logService,
            IRestService apiService,
            ISyncService syncService,
            IFusionService fusionService,
            IScheduleService scheduleService)
        {
            this.logService = logService;

            this.apiService = apiService;
            this.syncService = syncService;

            this.fusionService = fusionService;
            this.scheduleService = scheduleService;
        }

        /// <summary>
        /// Starts the current instance of <see cref="ZappServer"/> and it's dependencies.
        /// </summary>
        /// <inheritdoc />
        public void Start()
        {
            try
            {
                syncService.Connect();
                apiService.Listen();

                fusionService.ExtractAll();
                scheduleService.ScheduleAll();
            }
            catch (Exception ex)
            {
                logService.Fatal("Failed to start the server instance", ex);

                throw;
            }
        }
    }
}
