using System;
using System.Net;
using System.Net.Http;

namespace Zapp.Core.Extensions
{
    /// <summary>
    /// Represents a policy used to verify only acknowledged http request(s).
    /// </summary>
    public class AckHttpFailurePolicy : IHttpFailurePolicy
    {
        /// <summary>
        /// Invoked when an error occured during the http request.
        /// </summary>
        /// <param name="ex">The exception that occured during the http request.</param>
        /// <inheritDoc />
        public void OnError(Exception ex)
        {
            throw new AckHttpException(AckHttpException.RequestFailure, ex);
        }

        /// <summary>
        /// Invoked when a response was loaded during the http request.
        /// </summary>
        /// <param name="response">The response that was loaded during the http request.</param>
        /// <inheritDoc />
        public void OnResponse(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return;
            }

            var content = response.Content
                .ReadAsStringAsync()
                .GetAwaiter()
                .GetResult();

            throw new AckHttpException(AckHttpException.StatusCodeNotOk, response.StatusCode, content);
        }
    }
}
