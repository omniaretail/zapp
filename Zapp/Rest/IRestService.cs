namespace Zapp.Rest
{
    /// <summary>
    /// Represents a interface which is responsible for all the web-related actions.
    /// </summary>
    public interface IRestService
    {
        /// <summary>
        /// Starts listening on the configured endpoint.
        /// </summary>
        void Listen();
    }
}