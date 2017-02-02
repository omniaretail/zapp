namespace Zapp.Server
{
    /// <summary>
    /// Represents a interface which is responsible for all the actions.
    /// </summary>
    public interface IZappServer
    {
        /// <summary>
        /// Starts the instance of <see cref="IZappServer"/> and it's dependencies.
        /// </summary>
        void Start();
    }
}