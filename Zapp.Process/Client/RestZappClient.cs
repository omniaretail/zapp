using System;
using System.Net.Http;
using Zapp.Core;
using Zapp.Core.Http;
using Zapp.Process.Controller;

namespace Zapp.Process.Client
{
    /// <summary>
    /// Represents an implemenation of <see cref="IZappClient"/> using rest-calls.
    /// </summary>
    public class RestZappClient : IZappClient, IDisposable
    {
        private HttpClient client;

        private readonly IProcessController processController;

        /// <summary>
        /// Initializes a new <see cref="RestZappClient"/>.
        /// </summary>
        /// <param name="processController">Controller used for process' lifetime.</param>
        public RestZappClient(
            IProcessController processController)
        {
            this.processController = processController;

            var parentPort = Convert.ToInt32(processController.GetVariable<string>(
                ZappVariables.ParentPortEnvKey));

            client = new HttpClient().AsLocalhost(parentPort);
        }

        /// <summary>
        /// Announces the port to the zapp-service.
        /// </summary>
        /// <param name="port">Port of the process' rest-api.</param>
        /// <inheritdoc />
        public void Announce(int port)
        {
            var fusionId = processController.GetVariable<string>("fusion.id");

            client.ExpectOk($"api/radar/announce/{fusionId}/{port}");
        }

        /// <summary>
        /// Releases all resources used by the <see cref="RestZappClient"/> instance.
        /// </summary>
        public void Dispose()
        {
            client?.Dispose();
            client = null;
        }
    }
}
