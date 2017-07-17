namespace Zapp.Hospital
{
    /// <summary>
    /// Represents an interface to provide health details about something in a zapp fusion.
    /// </summary>
    public interface IPatient
    {
        /// <summary>
        /// The Id of the patient.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the status of the current patient.
        /// </summary>
        PatientStatus GetStatus();
    }
}
