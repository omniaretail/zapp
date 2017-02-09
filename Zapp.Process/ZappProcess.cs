using System;
using System.Threading;
using Zapp.Process.Client;
using Zapp.Process.Controller;
using Zapp.Process.Libraries;
using Zapp.Process.Rest;

namespace Zapp.Process
{
    /// <summary>
    /// Represents a entry point for ninject to resolve.
    /// </summary>
    public class ZappProcess : IZappProcess, IDisposable
    {
        private ManualResetEvent resetEvent;

        private readonly IZappClient zappClient;
        private readonly IRestService restService;
        private readonly IPortProvider portProvider;
        private readonly ILibraryService libraryService;
        private readonly IProcessController processController;

        /// <summary>
        /// Initializes a new <see cref="ZappProcess"/>.
        /// </summary>
        /// <param name="zappClient">Client used to communicate with the zapp-service.</param>
        /// <param name="restService">Service used to host the process' rest-api.</param>
        /// <param name="portProvider">Provider to request bindable ports from.</param>
        /// <param name="libraryService">Service used to load libraries.</param>
        /// <param name="processController">Controller used to mimic the parents process behavior.</param>
        public ZappProcess(
            IZappClient zappClient,
            IRestService restService,
            IPortProvider portProvider,
            ILibraryService libraryService,
            IProcessController processController)
        {
            this.zappClient = zappClient;
            this.restService = restService;
            this.portProvider = portProvider;
            this.libraryService = libraryService;
            this.processController = processController;

            resetEvent = new ManualResetEvent(false);
        }

        /// <summary>
        /// Starts all the services for the process.
        /// </summary>
        /// <inheritdoc />
        public void Start()
        {
            libraryService.LoadAll();
            processController.ParentProcessExited += (s, e) => Stop();

            var bindablePort = portProvider
                .FindBindablePort();

            restService.Listen(bindablePort);
            zappClient.Announce(bindablePort);

            resetEvent.WaitOne();
        }

        /// <summary>
        /// Starts all the services for the process.
        /// </summary>
        /// <inheritdoc />
        public void Stop() => resetEvent?.Set();

        /// <summary>
        /// Releases all used resouced by the <see cref="ZappProcess"/> instance.
        /// </summary>
        public void Dispose()
        {
            resetEvent?.Dispose();
            resetEvent = null;
        }
    }
}
