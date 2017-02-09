namespace Zapp.Process.Rest
{
    /// <summary>
    /// Represents an interface used for bootstrapping the rest-api.
    /// </summary>
    public interface IRestService
    {
        /// <summary>
        /// Starts the rest-service on the provided port.
        /// </summary>
        /// <param name="port">Port that the service should bind onto.</param>
        void Listen(int port);       
    }
}
