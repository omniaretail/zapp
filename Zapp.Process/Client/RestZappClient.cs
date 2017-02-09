using System;
using System.Net.Http;
using Zapp.Core;

namespace Zapp.Process.Client
{
    /// <summary>
    /// Represents an implemenation of <see cref="IZappClient"/> using rest-calls.
    /// </summary>
    public class RestZappClient : IZappClient, IDisposable
    {
        private HttpClient client;

        /// <summary>
        /// Initializes a new <see cref="RestZappClient"/>.
        /// </summary>
        public RestZappClient()
        {
            var rawParentPort = Environment.GetEnvironmentVariable(
                ZappVariables.ParentPortEnvKey,
                EnvironmentVariableTarget.Process
            );

            client = new HttpClient();
            client.BaseAddress = new Uri($"http://localhost:{rawParentPort}");
        }

        /// <summary>
        /// Announces the port to the zapp-service.
        /// </summary>
        /// <param name="port">Port of the process' rest-api.</param>
        /// <inheritdoc />
        public void Announce(int port)
        {
            var fusionId = AppDomain.CurrentDomain.FriendlyName;

            var response = client.GetAsync($"api/radar/announce/{fusionId}/{port}");
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
