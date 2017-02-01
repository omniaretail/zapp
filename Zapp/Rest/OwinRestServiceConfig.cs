namespace Zapp.Rest
{
    /// <summary>
    /// Represents a configuration class for <see cref="OwinRestService"/>.
    /// </summary>
    public class OwinRestServiceConfig
    {
        /// <summary>
        /// Represents the port the service must listen on.
        /// </summary>
        public int Port { get; set; } = 6464;
    }
}
