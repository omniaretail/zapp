using System;
using System.Runtime.Serialization;

namespace Zapp.Exceptions
{
    /// <summary>
    /// Represents an error that occured when a scheduling of a fusion fails.
    /// </summary>
    [Serializable]
    public class ScheduleException : Exception, ISerializable
    {
        /// <summary>
        /// Represents a message for a non found fusion it's process.
        /// </summary>
        public static readonly string NotFound = "Fusion process not found.";

        /// <summary>
        /// Represents a message for a failed to spawn fusion it's process;
        /// </summary>
        public static readonly string SpawnFailure = "Fusion process failed to spawn.";

        /// <summary>
        /// Represents a message for a timed out fusion it's announcement.
        /// </summary>
        public static readonly string TimedOut = "Fusion process it's announcement has timed out.";

        /// <summary>
        /// Represents a message for a dead fusion it's spawn.
        /// </summary>
        public static readonly string Dead = "Fusion process failed to start, caused death.";

        /// <summary>
        /// Represents a message for when a full announcement is required.
        /// </summary>
        public static readonly string FullAnnouncementRequired = "Zapp requires a full announcement to function before any delta.";

        /// <summary>
        /// Represents the id of the fusion where the error occurred on.
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
        /// Initializes a new <see cref="ScheduleException"/>.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="fusionId">Id of the fusion.</param>
        /// <param name="innerException">Inner <see cref="Exception"/> of the exception.</param>
        public ScheduleException(string message, string fusionId, Exception innerException = null)
            : base(message, innerException)
        {
            FusionId = fusionId ?? "?";
        }

        /// <summary>
        /// Initializes a new <see cref="ScheduleException"/> for the <see cref="ISerializable"/> interface.
        /// </summary>
        /// <inheritDoc />
        public ScheduleException(SerializationInfo info, StreamingContext context)
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
