using System;
using System.Net.Http;

namespace Zapp.Core.Extensions
{
    /// <summary>
    /// Represents a policy used to determine if a http request was success or not.
    /// </summary>
    public interface IHttpFailurePolicy
    {
        /// <summary>
        /// Invoked when an error occured during the http request.
        /// </summary>
        /// <param name="ex">The exception that occured during the http request.</param>
        void OnError(Exception ex);

        /// <summary>
        /// Invoked when a response was loaded during the http request.
        /// </summary>
        /// <param name="response">The response that was loaded during the http request.</param>
        void OnResponse(HttpResponseMessage response);
    }
}
