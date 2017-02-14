namespace Zapp.Process.Client
{
    /// <summary>
    /// Represents an interface maily used to communication with the zapp-service.
    /// </summary>
    public interface IZappClient
    {
        /// <summary>
        /// Tries to announce the port to the zapp-service.
        /// </summary>
        /// <param name="port">Port of the process' rest-api.</param>
        void Announce(int port);
    }
}
