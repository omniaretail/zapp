using System;
using System.Net;
using System.Net.Http;
using Zapp.Core.Clauses;

namespace Zapp.Core.Http
{
    /// <summary>
    /// Represents a collection of extensions for <see cref="HttpClient"/>.
    /// </summary>
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Gets the statuscode of a requested get-url.
        /// </summary>
        /// <param name="client">Client used to request on.</param>
        /// <param name="requestUri">Uri of the request.</param>
        public static bool ExpectOk(this HttpClient client, string requestUri)
        {
            Guard.ParamNotNull(client, nameof(client));
            Guard.ParamNotNullOrEmpty(requestUri, nameof(requestUri));

            try
            {
                using (var response = client.GetAsync(requestUri).Result)
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a new <see cref="HttpClient"/> to localhost with the given port.
        /// </summary>
        /// <param name="client">Client to bind to.</param>
        /// <param name="port">Port of the service.</param>
        public static HttpClient AsLocalhost(this HttpClient client, int? port)
        {
            if (client.BaseAddress != null)
            {
                client.BaseAddress = new Uri($"http://localhost:{port ?? 80}");
            }

            return client;
        }
    }
}