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
        /// Represents if the patient is healthy.
        /// </summary>
        bool IsHealthy();
    }
}
