using System.Reflection;

namespace Zapp.Process.Libraries
{
    /// <summary>
    /// Represents an interface for processing library post loading.
    /// </summary>
    public interface ILibraryPostProcessor
    {
        /// <summary>
        /// Processes the loaded assembly.
        /// </summary>
        /// <param name="assembly">The assembly that has been loaded.</param>
        void Process(Assembly assembly);
    }
}
