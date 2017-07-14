using System.IO;

namespace Zapp.Perspectives
{
    /// <summary>
    /// Represents a factory for <see cref="IDirectoryInfo"/>.
    /// </summary>
    public interface IDirectoryInfoFactory
    {
        /// <summary>
        /// <see cref="DirectoryInfo"/>
        /// </summary>
        IDirectoryInfo CreateNew(string path);
    }
}
