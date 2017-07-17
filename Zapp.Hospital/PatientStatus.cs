using EnsureThat;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Zapp.Hospital
{
    /// <summary>
    /// Represents a status for the <see cref="IPatient.GetStatus()"/> method/.
    /// </summary>
    public class PatientStatus
    {
        /// <summary>
        /// Represents the identity of the patient.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Represents the type of the status.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public PatientStatusType Type { get; private set; }

        /// <summary>
        /// Represents the reason of the type.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Reason { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="PatientStatus"/>.
        /// </summary>
        /// <param name="id">The identity of the <see cref="IPatient"/>.</param>
        /// <param name="type">The type of the status.</param>
        /// <param name="reason">
        /// The reason of the <paramref name="type"/>. 
        /// Which can only be empty/null when the <paramref name="type"/> 
        /// is set to <see cref="PatientStatusType.Green"/>.
        /// </param>
        [JsonConstructor]
        public PatientStatus(string id, PatientStatusType type, string reason = null)
        {
            EnsureArg.IsNotNullOrEmpty(id, nameof(id));

            if (type != PatientStatusType.Green)
            {
                EnsureArg.IsNotNullOrEmpty(reason, nameof(reason));
            }

            Id = id;
            Type = type;
            Reason = reason;
        }
    }
}
