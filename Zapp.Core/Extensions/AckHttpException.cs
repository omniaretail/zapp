using System;
using System.Net;
using System.Runtime.Serialization;

namespace Zapp.Core.Extensions
{
    /// <summary>
    /// Represents an acknowledge failure when a http request failed for the <see cref="AckHttpFailurePolicy"/>.
    /// </summary>
    [Serializable]
    public class AckHttpException : Exception, ISerializable
    {
        /// <summary>
        /// Represents a message for a critical http error.
        /// </summary>
        public const string RequestFailure = "The http request failed to deliver.";

        /// <summary>
        /// Represents a message for a not-ok status-code.
        /// </summary>
        public const string StatusCodeNotOk = "The http request failed due a not-ok status-code.";

        /// <summary>
        /// Represents the status-code of the response.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Represents the response body of the response.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        /// <inheritdoc />
        public override string Message
        {
            get
            {
                var message = base.Message;

                message += Environment.NewLine;
                message += $"StatusCode: {StatusCode}";

                message += Environment.NewLine;
                message += $"Content: {Content}";

                return message;
            }
        }

        /// <summary>
        /// Initializes a new <see cref="AckHttpException"/>.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="statusCode">The status-code of the response.</param>
        /// <param name="content">The content of the response.</param>
        /// <param name="innerException">Inner <see cref="Exception"/> of the exception.</param>
        public AckHttpException(string message, HttpStatusCode statusCode, string content, Exception innerException = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            Content = content ?? string.Empty;
        }

        /// <summary>
        /// Initializes a new <see cref="AckHttpException"/>.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Inner <see cref="Exception"/> of the exception.</param>
        public AckHttpException(string message, Exception innerException = null)
            : this(message, HttpStatusCode.InternalServerError, string.Empty, innerException) { }

        /// <summary>
        /// Initializes a new <see cref="AckHttpException"/> for the <see cref="ISerializable"/> interface.
        /// </summary>
        /// <inheritDoc />
        public AckHttpException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            StatusCode = (HttpStatusCode)info.GetValue(nameof(StatusCode), typeof(HttpStatusCode));
            Content = info.GetString(nameof(Content));
        }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(StatusCode), StatusCode);
            info.AddValue(nameof(Content), Content);
        }
    }
}
