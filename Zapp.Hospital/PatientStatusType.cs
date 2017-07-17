namespace Zapp.Hospital
{
    /// <summary>
    /// Represents the type of status a patient can have.
    /// </summary>
    public enum PatientStatusType
    {
        /// <summary>
        /// Indicates that the patient is unhealthy.
        /// </summary>
        Red = 0,

        /// <summary>
        /// Indicates that the patient is not healthy nor unhealthy.
        /// </summary>
        Yellow = 1,

        /// <summary>
        /// Indicates that the patient is healthy.
        /// </summary>
        Green = 2
    }
}
