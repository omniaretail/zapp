namespace Zapp.Pack
{
    /// <summary>
    /// Represents an enum which indicates what mode the <see cref="IPackage"/> uses.
    /// </summary>
    public enum PackageMode
    {
        /// <summary>
        /// Indicates that the mode is reading.
        /// </summary>
        Read = 0,

        /// <summary>
        /// Indicates that the mode is writing.
        /// </summary>
        Write = 1
    }
}
