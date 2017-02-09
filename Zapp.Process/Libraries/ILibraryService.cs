namespace Zapp.Process.Libraries
{
    /// <summary>
    /// Represents an interface mainly used for loading packages.
    /// </summary>
    public interface ILibraryService
    {
        /// <summary>
        /// Loads all the assemblies located in the working-dir.
        /// </summary>
        void LoadAll();

        /// <summary>
        /// Runs the defined startup assembly.
        /// </summary>
        void RunStartup();

        /// <summary>
        /// Runs the defined teardown method.
        /// </summary>
        void RunTeardown();
    }
}
