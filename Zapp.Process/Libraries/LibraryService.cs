using Ninject;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Zapp.Core;
using Zapp.Core.NuGet;
using Zapp.Process.Controller;
using Zapp.Process.Meta;

namespace Zapp.Process.Libraries
{
    /// <summary>
    /// Represents an implementation of <see cref="ILibraryService"/>.
    /// </summary>
    public class LibraryService : ILibraryService
    {
        private static string[] allowedExtensions = new[] { ".dll", ".exe" };

        private readonly IKernel kernel;
        private readonly IMetaService metaService;
        private readonly IProcessController processController;
        private readonly INuGetPackageResolver nuGetPackageResolver;

        /// <summary>
        /// Initializes a new <see cref="LibraryService"/>.
        /// </summary>
        /// <param name="kernel">Ninject kernel instance.</param>
        /// <param name="metaService">Service used for receiving meta info.</param>
        /// <param name="processController">Controller for process' lifetime.</param>
        /// <param name="nuGetPackageResolver">Resolver used to resolve NuGet packages.</param>
        public LibraryService(
            IKernel kernel,
            IMetaService metaService,
            IProcessController processController,
            INuGetPackageResolver nuGetPackageResolver)
        {
            this.kernel = kernel;
            this.metaService = metaService;
            this.processController = processController;
            this.nuGetPackageResolver = nuGetPackageResolver;
        }

        /// <summary>
        /// Loads all the assemblies located in the working-dir.
        /// </summary>
        /// <inheritdoc />
        public void LoadAll()
        {
            var directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

            var nuGetPackages = nuGetPackageResolver
                .GetPackageIds(typeof(LibraryService).Assembly);

            var missingLibraries = directory.GetFiles()
                .Where(f => allowedExtensions.Contains(f.Extension, StringComparer.OrdinalIgnoreCase))
                .Where(f => !nuGetPackages.Contains(GetFileName(f), StringComparer.OrdinalIgnoreCase))
                .Where(f => !IsFileLoaded(f))
                .ToList();

            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;

            foreach (var library in missingLibraries)
            {
                Assembly.LoadFile(library.FullName);
            }
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

            var candidates = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => string.Equals(a.GetName().Name, assemblyName.Name, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return candidates
                .OrderByDescending(c => c.GetName().Version)
                .FirstOrDefault();
        }

        private bool IsFileLoaded(FileInfo info)
        {
            var loadedAssemblyNames = AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(e => e.GetName().Name)
                .ToList();

            var fileName = GetFileName(info);

            return loadedAssemblyNames.Contains(fileName, StringComparer.OrdinalIgnoreCase);
        }

        private string GetFileName(FileInfo info) => Path.GetFileNameWithoutExtension(info.Name);
    }
}
