using EnsureThat;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Zapp.Hospital
{
    /// <summary>
    /// Represents a hospital status for a particular fusion.
    /// </summary>
    public class FusionStatus
    {
        /// <summary>
        /// Represents the identity of the fusion.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Represents all the patients that the fusion responded with.
        /// </summary>
        public IEnumerable<PatientStatus> Patients { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="FusionStatus"/>.
        /// </summary>
        /// <param name="id">Identity of the fusion.</param>
        /// <param name="patients">Patients that the fusion responded.</param>
        [JsonConstructor]
        public FusionStatus(string id, IEnumerable<PatientStatus> patients)
        {
            EnsureArg.IsNotNullOrEmpty(id, nameof(id));
            EnsureArg.IsNotNull(patients, nameof(patients));

            Id = id;
            Patients = patients;
        }
    }
}
