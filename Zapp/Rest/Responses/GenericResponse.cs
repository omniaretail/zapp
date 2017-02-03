namespace Zapp.Rest.Responses
{
    /// <summary>
    /// Represents a default response for the rest-service.
    /// </summary>
    public class GenericResponse
    {
        /// <summary>
        /// Represents if the operation was successful or not.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Represents a reason if the operation was not successful.
        /// </summary>
        public string Reason { get; set; }
    }
}
