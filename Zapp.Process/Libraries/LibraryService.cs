using Ninject;
using Ninject.Web.Common;
using Ninject.Web.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Zapp.Process.Libraries
{
    /// <summary>
    /// Represents an implementation of <see cref="ILibraryService"/>.
    /// </summary>
    public class LibraryService : ILibraryService
    {
        private readonly IKernel kernel;
        //private readonly IZappProcess zappProcess;

        /// <summary>
        /// Initializes a new <see cref="LibraryService"/>.
        /// </summary>
        /// <param name="kernel">Ninject kernel instance.</param>
        /// <param name="zappProcess">The process instance used to stop the app.</param>
        public LibraryService(
            IKernel kernel)
        {
            this.kernel = kernel;
            //this.zappProcess = zappProcess;
        }

        /// <summary>
        /// Loads all the assemblies located in the working-dir.
        /// </summary>
        /// <inheritdoc />
        public void LoadAll()
        {
            var libraries = Directory.GetFiles(
                AppDomain.CurrentDomain.BaseDirectory, "*.dll");

            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;

            foreach (string library in libraries)
            {
                Assembly.LoadFile(library);
            }

            kernel.Load(
                new WebCommonNinjectModule(),
                new WebApiModule()
            );
        }

        /// <summary>
        /// Runs the defined startup assembly.
        /// </summary>
        /// <inheritdoc />
        public void RunStartup() { }

        /// <summary>
        /// Runs the defined teardown method.
        /// </summary>
        /// <inheritdoc />
        public void RunTeardown()
        {
            // do it !

            Environment.Exit(0);
        }

        private static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            var assemblyName = new AssemblyName(args.Name);

            return AppDomain.CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(a => string.Equals(a.GetName().Name, assemblyName.Name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
