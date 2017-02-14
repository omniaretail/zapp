namespace Zapp.Process.Controller
{
    /// <summary>
    /// Represents an interface used to controll the lifecycle of the zapp-process.
    /// </summary>
    public interface IProcessController
    {
        /// <summary>
        /// Gets a variable which is defined for this process.
        /// </summary>
        /// <typeparam name="T">Type of the variable.</typeparam>
        /// <param name="key">Key of the variable.</param>
        T GetVariable<T>(string key);

        /// <summary>
        /// Waits for completion of the process.
        /// </summary>
        void WaitForCompletion();

        /// <summary>
        /// Cancels the current <see cref="WaitForCompletion"/>.
        /// </summary>
        void Cancel();
    }
}
