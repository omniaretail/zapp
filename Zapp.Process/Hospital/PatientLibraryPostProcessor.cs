using Ninject;
using Ninject.Extensions.Conventions;
using System.Reflection;
using Zapp.Hospital;
using Zapp.Process.Libraries;

namespace Zapp.Process.Hospital
{
    /// <summary>
    /// Represents a <see cref="ILibraryPostProcessor"/> which binds all <see cref="IPatient"/> instances.
    /// </summary>
    public class PatientLibraryPostProcessor : ILibraryPostProcessor
    {
        private readonly IKernel kernel;

        /// <summary>
        /// Initializes a new <see cref="PatientLibraryPostProcessor"/> with it's dependencies.
        /// </summary>
        /// <param name="kernel">The ninject kernel that currently active.</param>
        public PatientLibraryPostProcessor(IKernel kernel)
        {
            this.kernel = kernel;
        }

        /// <summary>
        /// Processes the loaded assembly.
        /// </summary>
        /// <param name="assembly">The assembly that has been loaded.</param>
        /// <inheritDoc />
        public void Process(Assembly assembly)
        {
            kernel.Bind(_ => _
                .From(assembly)
                .SelectAllClasses()
                .InheritedFrom<IPatient>()
                .BindSingleInterface()
            );
        }
    }
}
