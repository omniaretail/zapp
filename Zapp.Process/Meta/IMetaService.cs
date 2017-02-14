namespace Zapp.Process.Meta
{
    /// <summary>
    /// Represents an interface used for receiving the process' meta info.
    /// </summary>
    public interface IMetaService
    {
        /// <summary>
        /// Loads the meta information.
        /// </summary>
        void Load();

        /// <summary>
        /// Gets the value of a meta key.
        /// </summary>
        /// <param name="key">Key of the meta info.</param>
        string GetValue(string key);
    }
}
