namespace Zapp.Process.Rest
{
    /// <summary>
    /// Represents an interface used to provide available ports.
    /// </summary>
    public interface IPortProvider
    {
        /// <summary>
        /// Scans for an available port to bind on.
        /// </summary>
        int FindBindablePort();
    }
}
