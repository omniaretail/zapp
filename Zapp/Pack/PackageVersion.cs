using EnsureThat;
using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace Zapp.Pack
{
    /// <summary>
    /// Represents a class with a collaboration of package and it's deploy version.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class PackageVersion : IEquatable<PackageVersion>
    {
        private const string unknownVersion = "?";

        /// <summary>
        /// Represents the identity of the package.
        /// </summary>
        [JsonProperty("packageId")]
        public string PackageId { get; private set; }

        /// <summary>
        /// Represents the deploy version of the package.
        /// </summary>
        [JsonProperty("deployVersion")]
        public string DeployVersion { get; private set; }

        /// <summary>
        /// Represents if the version is unknown.
        /// </summary>
        [JsonIgnore]
        public bool IsUnknown => DeployVersion == unknownVersion;

        /// <summary>
        /// Initializes a new <see cref="PackageVersion"/>.
        /// </summary>
        /// <param name="packageId">Identity of the package.</param>
        /// <param name="deployVersion">Deploy version of the package.</param>
        /// <exception cref="ArgumentException">Throw when either <paramref name="packageId"/>  is not set.</exception>
        [JsonConstructor]
        public PackageVersion(string packageId, string deployVersion = unknownVersion)
        {
            EnsureArg.IsNotNullOrEmpty(packageId, nameof(packageId));

            PackageId = packageId;
            DeployVersion = deployVersion ?? unknownVersion;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <inheritdoc />
        public bool Equals(PackageVersion other)
        {
            if (other == null) return false;

            return string.Equals(PackageId, other.PackageId, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(DeployVersion, other.DeployVersion, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj"> The object to compare with the current object.</param>
        /// <inheritdoc />
        public override bool Equals(object obj) => ((obj as PackageVersion) == this);

        /// <summary>
        /// Equality operator overload.
        /// </summary>
        public static bool operator ==(PackageVersion a, PackageVersion b)
        {
            if (ReferenceEquals(null, a)) return ReferenceEquals(null, b);

            return a.Equals(b);
        }

        /// <summary>
        /// Equality operator overload.
        /// </summary>
        public static bool operator !=(PackageVersion a, PackageVersion b) => !(a == b);

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return PackageId.GetHashCode() ^ DeployVersion.GetHashCode();
            }
        }

        private string DebuggerDisplay => $"Version: {PackageId} - {DeployVersion}";
    }
}
