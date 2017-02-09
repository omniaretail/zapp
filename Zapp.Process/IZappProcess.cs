namespace Zapp.Process
{
    /// <summary>
    /// Represents an interface used to collaborate all the services.
    /// </summary>
    public interface IZappProcess
    {
        /// <summary>
        /// Starts all the services for the process.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the application from living.
        /// </summary>
        void Stop();
    }
}