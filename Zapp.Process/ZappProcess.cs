using Zapp.Process.Client;
using Zapp.Process.Libraries;
using Zapp.Process.Meta;
using Zapp.Process.Rest;

namespace Zapp.Process
{
    /// <summary>
    /// Represents a entry point for ninject to resolve.
    /// </summary>
    public class ZappProcess : IZappProcess
    {
        private readonly IZappClient zappClient;
        private readonly IRestService restService;
        private readonly IMetaService metaService;
        private readonly IPortProvider portProvider;
        private readonly ILibraryService libraryService;

        /// <summary>
        /// Initializes a new <see cref="ZappProcess"/>.
        /// </summary>
        /// <param name="zappClient">Client used to communicate with the zapp-service.</param>
        /// <param name="metaService">Service used to receive meta info.</param>
        /// <param name="restService">Service used to host the process' rest-api.</param>
        /// <param name="portProvider">Provider to request bindable ports from.</param>
        /// <param name="libraryService">Service used to load libraries.</param>
        public ZappProcess(
            IZappClient zappClient,
            IRestService restService,
            IMetaService metaService,
            IPortProvider portProvider,
            ILibraryService libraryService)
        {
            this.zappClient = zappClient;
            this.restService = restService;
            this.metaService = metaService;
            this.portProvider = portProvider;
            this.libraryService = libraryService;
        }

        /// <summary>
        /// Starts all the services for the process.
        /// </summary>
        /// <inheritdoc />
        public void Start()
        {
            metaService.Load();

            var bindablePort = portProvider
                .FindBindablePort();

            restService.Listen(bindablePort);

            libraryService.LoadAll();

            zappClient.Announce(bindablePort);
        }
    }
}
