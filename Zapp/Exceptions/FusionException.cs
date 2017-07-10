using System;
using System.Runtime.Serialization;

namespace Zapp.Exceptions
{
    /// <summary>
    /// Represents one non existent fusion configuration that was requested during extraction.
    /// </summary>
    [Serializable]
    public class FusionException : Exception, ISerializable
    {
        /// <summary>
        /// Represents a message for a non found fusion.
        /// </summary>
        public static readonly string NotFound = "Fusion configuration not found.";

        /// <summary>
        /// Represents a message for a failed extraction.
        /// </summary>
        public static readonly string ExtractionFailure = "Fusion failed to extract.";

        /// <summary>
        /// Represents the id of the not found fusion.
        /// </summary>
        public string FusionId { get; set; }

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
                message += $"FusionId: {FusionId}";

                return message;
            }
        }

        /// <summary>
        /// Initializes a new <see cref="FusionException"/>.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="fusionId">Id of the fusion.</param>
        /// <param name="innerException">Inner <see cref="Exception"/> of the exception.</param>
        public FusionException(string message, string fusionId, Exception innerException = null)
            : base(message, innerException)
        {
            FusionId = fusionId ?? "?";
        }

        /// <summary>
        /// Initializes a new <see cref="FusionException"/> for the <see cref="ISerializable"/> interface.
        /// </summary>
        /// <inheritDoc />
        public FusionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            FusionId = info.GetString(nameof(FusionId));
        }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(FusionId), FusionId);
        }
    }
}
