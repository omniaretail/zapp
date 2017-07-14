using System.IO;

namespace Zapp.Perspectives
{
    /// <summary>
    /// Perspective for <see cref="DirectoryInfo"/>.
    /// </summary>
    public interface IDirectoryInfo
    {
        /// <summary>
        /// <see cref="DirectoryInfo.GetDirectories()"/>
        /// </summary>
        DirectoryInfo[] GetDirectories();

        /// <summary>
        /// <see cref="DirectoryInfo.Exists"/>
        /// </summary>
        bool Exists { get; }

        /// <summary>
        /// <see cref="DirectoryInfo.Create()"/>
        /// </summary>
        void Create();
    }
}
