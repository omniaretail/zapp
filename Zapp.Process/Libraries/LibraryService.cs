using Ninject;
using Ninject.Web.Common;
using Ninject.Web.WebApi;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Zapp.Core;
using Zapp.Process.Controller;
using Zapp.Process.Meta;

namespace Zapp.Process.Libraries
{
    /// <summary>
    /// Represents an implementation of <see cref="ILibraryService"/>.
    /// </summary>
    public class LibraryService : ILibraryService
    {
        private readonly IKernel kernel;
        private readonly IMetaService metaService;
        private readonly IProcessController processController;

        /// <summary>
        /// Initializes a new <see cref="LibraryService"/>.
        /// </summary>
        /// <param name="kernel">Ninject kernel instance.</param>
        /// <param name="metaService">Service used for receiving meta info.</param>
        /// <param name="processController">Controller for process' lifetime.</param>
        public LibraryService(
            IKernel kernel,
            IMetaService metaService,
            IProcessController processController)
        {
            this.kernel = kernel;
            this.metaService = metaService;
            this.processController = processController;
        }

        /// <summary>
        /// Loads all the assemblies located in the working-dir.
        /// </summary>
        /// <inheritdoc />
        public void LoadAll()
        {
            var libraries = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.*")
                .Where(e => e.EndsWith(".dll") || e.EndsWith(".exe"))
                .ToList();

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
        public void RunStartup()
        {
            var assembly = default(Assembly);
            var assemblySearch = metaService
                .GetValue(ZappVariables.StartupAssemblyNameFusionInfoKey);

            if (TryFindAssembly(assemblySearch, out assembly))
            {
                assembly?.EntryPoint?.Invoke(null, new object[] { new string[0] });
            }
        }

        /// <summary>
        /// Runs the defined teardown method.
        /// </summary>
        /// <inheritdoc />
        public void RunTeardown()
        {
            var assembly = default(Assembly);
            var assemblySearch = metaService
                .GetValue(ZappVariables.TeardownAssemblyNameFusionInfoKey);

            var type = default(Type);
            var typeSearch = metaService
                .GetValue(ZappVariables.TeardownTypeNameFusionInfoKey);

            var method = default(MethodInfo);
            var methodSearch = metaService
                .GetValue(ZappVariables.TeardownMethodNameFusionInfoKey);

            if (TryFindAssembly(assemblySearch, out assembly) &&
                TryFindType(typeSearch, assembly, out type) &&
                TryFindMethod(methodSearch, type, out method))
            {
                method?.Invoke(null, new object[0]);
            }

            processController.Cancel();
        }

        private bool TryFindAssembly(string name, out Assembly assembly)
        {
            return (assembly = AppDomain.CurrentDomain
                .GetAssemblies()
                .SingleOrDefault(a => string.Equals(a.GetName().Name, name, StringComparison.OrdinalIgnoreCase))) != null;
        }

        private bool TryFindType(string name, Assembly assembly, out Type type)
        {
            return (type = assembly
                .GetTypes()
                .SingleOrDefault(t => string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase))) != null;
        }

        private bool TryFindMethod(string name, Type type, out MethodInfo method)
        {
            return (method = type
                .GetMethods()
                .SingleOrDefault(t => string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase))) != null;
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
