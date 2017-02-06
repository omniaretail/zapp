namespace Zapp.Config
{
    /// <summary>
    /// Represents an interface for loading the <see cref="ZappConfig"/>.
    /// </summary>
    public interface IConfigStore
    {
        /// <summary>
        /// Represents the value for the zapp-config.
        /// </summary>
        ZappConfig Value { get; }
    }
}