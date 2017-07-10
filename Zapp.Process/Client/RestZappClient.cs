using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Zapp.Core;
using Zapp.Core.Extensions;
using Zapp.Process.Controller;

namespace Zapp.Process.Client
{
    /// <summary>
    /// Represents an implemenation of <see cref="IZappClient"/> using rest-calls.
    /// </summary>
    public sealed class RestZappClient : IZappClient, IDisposable
    {
        private HttpClient client;

        private readonly IProcessController processController;
        private readonly IHttpFailurePolicy httpFailurePolicy;

        /// <summary>
        /// Initializes a new <see cref="RestZappClient"/>.
        /// </summary>
        /// <param name="processController">Controller used for process' lifetime.</param>
        /// <param name="httpFailurePolicy">Failure policy used to determine http request(s) success.</param>
        public RestZappClient(
            IProcessController processController,
            IHttpFailurePolicy httpFailurePolicy)
        {
            this.processController = processController;
            this.httpFailurePolicy = httpFailurePolicy;

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
            AnnounceAsync(port, CancellationToken.None)
                .GetAwaiter()
                .GetResult();
        }

        private async Task AnnounceAsync(int port, CancellationToken token)
        {
            var fusionId = processController.GetVariable<string>("fusion.id");
            var requestUri = $"api/radar/announce/{fusionId}/{port}";

            await client.GetWithFailurePolicyAsync(requestUri, httpFailurePolicy, token);
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
