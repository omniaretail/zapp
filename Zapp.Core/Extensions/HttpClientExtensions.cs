using EnsureThat;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Zapp.Core.Extensions
{
    /// <summary>
    /// Represents a collection of extensions for <see cref="HttpClient"/>.
    /// </summary>
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Runs a Http-Get with a <see cref="IHttpFailurePolicy"/> which determines if the request was success or not.
        /// </summary>
        /// <param name="client">The client which is used to run the request with.</param>
        /// <param name="requestUri">The url of the request.</param>
        /// <param name="failurePolicy">The policy used to determine if the request was success or not.</param>
        /// <param name="token">The token used to cancel the request.</param>
        public static async Task GetWithFailurePolicyAsync(this HttpClient client, string requestUri, IHttpFailurePolicy failurePolicy, CancellationToken token)
        {
            EnsureArg.IsNotNull(client, nameof(client));
            EnsureArg.IsNotNullOrEmpty(requestUri, nameof(requestUri));
            EnsureArg.IsNotNull(failurePolicy, nameof(failurePolicy));

            try
            {
                using (var response = await client.GetAsync(requestUri, token))
                {
                    failurePolicy.OnResponse(response);
                }
            }
            catch (Exception ex)
            {
                failurePolicy.OnError(ex);
            }
        }

        /// <summary>
        /// Runs a Http-Get and deserializes the json response.
        /// </summary>
        /// <param name="client">The client which is used to run the request with.</param>
        /// <param name="requestUri">The url of the request.</param>
        /// <param name="token">The token used to cancel the request.</param>
        public static async Task<T> GetJsonAsync<T>(this HttpClient client, string requestUri, CancellationToken token)
        {
            EnsureArg.IsNotNull(client, nameof(client));
            EnsureArg.IsNotNullOrEmpty(requestUri, nameof(requestUri));

            var response = await client.GetAsync(requestUri, token);

            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var textReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(textReader))
            {
                var serializer = new JsonSerializer();

                return serializer.Deserialize<T>(jsonReader);
            }
        }

        /// <summary>
        /// Creates a new <see cref="HttpClient"/> to localhost with the given port.
        /// </summary>
        /// <param name="client">Client to bind to.</param>
        /// <param name="port">Port of the service.</param>
        public static HttpClient AsLocalhost(this HttpClient client, int? port)
        {
            if (client.BaseAddress == null)
            {
                client.BaseAddress = new Uri($"http://localhost:{port ?? 80}");
            }

            return client;
        }
    }
}