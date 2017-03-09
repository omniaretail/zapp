using System.IO;

namespace Zapp.Perspectives
{
    /// <summary>
    /// Represents an interface to perspective the <see cref="File"/> class.
    /// </summary>
    public interface IFile
    {
        /// <summary>
        /// <see cref="File.Exists(string)"/>
        /// </summary>
        bool Exists(string path);

        /// <summary>
        /// <see cref="File.ReadAllText(string)"/>
        /// </summary>
        string ReadAllText(string path);

        /// <summary>
        /// <see cref="File.WriteAllText(string, string)"/>
        /// </summary>
        void WriteAllText(string path, string contents);
    }
}
