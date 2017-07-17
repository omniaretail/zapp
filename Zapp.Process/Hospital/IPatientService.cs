using System.Collections.Generic;
using Zapp.Hospital;

namespace Zapp.Process.Hospital
{
    /// <summary>
    /// Represents an interface for a service that handles status requests for <see cref="IPatient"/> instances.
    /// </summary>
    public interface IPatientService
    {
        /// <summary>
        /// Gets all the statusses of tha <see cref="IPatient"/> instance that matches the pattern.
        /// </summary>
        /// <param name="patientPattern">Pattern to determine the instances of <see cref="IPatient"/>.</param>
        IEnumerable<PatientStatus> GetStatusses(string patientPattern = "*");
    }
}