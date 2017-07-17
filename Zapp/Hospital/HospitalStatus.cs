using EnsureThat;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Zapp.Hospital
{
    /// <summary>
    /// Represents a hospital status for a set of fusions.
    /// </summary>
    public class HospitalStatus : PatientStatus
    {
        /// <inheritDoc />
        [JsonIgnore]
        public new string Id { get; }

        /// <summary>
        /// Represents all the fusions their statusses.
        /// </summary>
        public IEnumerable<FusionStatus> Fusions { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="HospitalStatus"/>.
        /// </summary>
        /// <param name="fusions">Statusses of the requested fusions.</param>
        /// <param name="type">The evaluated final status-type.</param>
        /// <param name="reason">The reason of the <paramref name="type"/>.</param>
        [JsonConstructor]
        public HospitalStatus(IEnumerable<FusionStatus> fusions, PatientStatusType type, string reason = null)
            : base(nameof(HospitalStatus), type, reason)
        {
            EnsureArg.IsNotNull(fusions, nameof(fusions));

            Fusions = fusions;
        }
    }
}
