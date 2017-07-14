using System;
using System.Runtime.Serialization;
using Zapp.Pack;

namespace Zapp.Exceptions
{
    /// <summary>
    /// Represents an implementation of <see cref="Exception"/> for sync error(s).
    /// </summary>
    [Serializable]
    public class SyncException : Exception, ISerializable
    {
        /// <summary>
        /// Represents a message for a announcing failure.
        /// </summary>
        public static readonly string AnnounceFailure = "Version failed to announce to the sync-server";

        /// <summary>
        /// Represents the version of the package.
        /// </summary>
        public PackageVersion Version { get; private set; }

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
                message += $"Version.PackageId: {Version.PackageId}";

                message += Environment.NewLine;
                message += $"Version.DeployVersion: {Version.DeployVersion}";

                return message;
            }
        }

        /// <summary>
        /// Initializes a new <see cref="SyncException"/>.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="version">Version of the package.</param>
        /// <param name="innerException">Inner <see cref="Exception"/> of the exception.</param>
        public SyncException(string message, PackageVersion version, Exception innerException = null)
            : base(message, innerException)
        {
            Version = version ?? new PackageVersion("?", "?");
        }

        /// <summary>
        /// Initializes a new <see cref="SyncException"/> for the <see cref="ISerializable"/> interface.
        /// </summary>
        /// <inheritDoc />
        public SyncException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Version = info.GetValue(nameof(Version), typeof(PackageVersion)) as PackageVersion;
        }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(Version), Version);
        }
    }
}
