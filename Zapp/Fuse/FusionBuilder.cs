using AntPathMatching;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Zapp.Config;
using Zapp.Core.NuGet;
using Zapp.Extensions;
using Zapp.Pack;
using Zapp.Process;

namespace Zapp.Fuse
{
    /// <summary>
    /// Represents a builder for all the entries required for a <see cref="IFusion"/>.
    /// </summary>
    public class FusionBuilder : IFusionBuilder
    {
        private readonly IAnt entryFilter;

        private readonly IConfigStore configStore;
        private readonly IPackService packService;
        private readonly INuGetPackageResolver nuGetPackageResolver;
        private readonly IFrameworkPackageEntryFactory frameworkPackageEntryFactory;

        private readonly IReadOnlyCollection<IFusionFilter> fusionFilters;
        private readonly IReadOnlyCollection<IFusionInterceptor> fusionInterceptors;

        private IReadOnlyCollection<FileInfo> dependencyFiles;

        /// <summary>
        /// Initializes a new <see cref="FusionBuilder"/> with all it's dependencies.
        /// </summary>
        /// <param name="antFactory">Factory used to create <see cref="IAnt"/> instances.</param>
        /// <param name="configStore">Store that resolved app-level configuration.</param>
        /// <param name="packService">Service that is used to get the packages from.</param>
        /// <param name="nuGetPackageResolver">Resolver that handles required NuGet packages.</param>
        /// <param name="frameworkPackageEntryFactory">Factory used for creating framework instances of <see cref="IPackageEntry"/>.</param>
        /// <param name="fusionFilters">A collection of filters used to filter instances of <see cref="IPackageEntry"/>.</param>
        /// <param name="fusionInterceptors">A collection of interceptors that intercept the creation of <see cref="IPackageEntry"/>.</param>
        public FusionBuilder(
            IAntFactory antFactory,
            IConfigStore configStore,
            IPackService packService,
            INuGetPackageResolver nuGetPackageResolver,
            IFrameworkPackageEntryFactory frameworkPackageEntryFactory,
            IEnumerable<IFusionFilter> fusionFilters,
            IEnumerable<IFusionInterceptor> fusionInterceptors)
        {
            this.configStore = configStore;
            this.packService = packService;
            this.nuGetPackageResolver = nuGetPackageResolver;
            this.frameworkPackageEntryFactory = frameworkPackageEntryFactory;

            this.fusionFilters = fusionFilters.StaleReadOnly();
            this.fusionInterceptors = fusionInterceptors.StaleReadOnly();

            entryFilter = antFactory
                .CreateNew(configStore?.Value?.Fuse?.EntryPattern);

            dependencyFiles = GetDependencyFiles().StaleReadOnly();
        }

        /// <summary>
        /// Builds a new fusion package into <paramref name="fusion"/> with all the required package versions.
        /// </summary>
        /// <param name="fusionConfig">Configuration of the fusion that needs to be builded.</param>
        /// <param name="fusion">Fusion that needs to be builded.</param>
        /// <param name="packageVersions">Versions of the packages that needs to be included in the fusion.</param>
        /// <inheritdoc />
        public void Build(FusePackConfig fusionConfig, IFusion fusion, IEnumerable<PackageVersion> packageVersions)
        {
            EnsureArg.IsNotNull(fusionConfig, nameof(fusionConfig));
            EnsureArg.IsNotNull(fusion, nameof(fusion));
            EnsureArg.IsNotNull(packageVersions, nameof(packageVersions));

            IEnumerable<IPackage> packages = null;

            try
            {
                packages = packageVersions
                    .Select(_ => packService.LoadPackage(_));

                var finalEntries = GetFinalEntries(fusionConfig, packages);

                foreach (var entry in finalEntries)
                {
                    SaveEntryOnFusion(fusion, entry, fusionConfig);
                }
            }
            finally
            {
                if (packages != null)
                {
                    foreach (var package in packages)
                    {
                        (package as IDisposable)?.Dispose();
                    }
                }
            }
        }

        private void SaveEntryOnFusion(
            IFusion fusion,
            IPackageEntry entry,
            FusePackConfig fusionConfig)
        {
            foreach (var filter in fusionFilters)
            {
                filter.BeforeAddEntry(fusionConfig, entry);
            }

            fusion.AddEntry(entry);
        }

        private IEnumerable<IPackageEntry> GetFinalEntries(FusePackConfig fusionConfig, IEnumerable<IPackage> packages)
        {
            var basicEntries = GetBasicEntries(fusionConfig);

            var packageEntries = packages
                .SelectMany(_ => _.GetEntries())
                .Where(_ => entryFilter.IsMatch(_.Name));

            var finalEntries = basicEntries
                .Concat(packageEntries);

            return finalEntries
                .GroupBy(_ => _.Name, StringComparer.OrdinalIgnoreCase)
                .Select(_ => _.FirstOrDefault());
        }

        private IEnumerable<IPackageEntry> GetBasicEntries(FusePackConfig fusionConfig)
        {
            var dependencyEntries = dependencyFiles
                .Select(_ => new LazyPackageEntry(_.Name, new LazyStream(() => _.OpenRead())));

            var interceptorEntries = fusionInterceptors
                .Select(_ => _.GetEntries(fusionConfig))
                .Where(_ => _ != null)
                .SelectMany(_ => _);

            var frameworkEntries = frameworkPackageEntryFactory
                .CreateNew()
                .Cast<IPackageEntry>();

            return dependencyEntries
                .Concat(frameworkEntries)
                .Concat(interceptorEntries);
        }

        private IEnumerable<FileInfo> GetDependencyFiles()
        {
            var mainAssembly = typeof(ZappProcessModule).Assembly;
            var mainAssemblyNuGetReferences = GetFlatNuGetReferenceFiles(mainAssembly);
            var mainAssemblyReferences = GetReferencesForAssembly(mainAssembly);

            var mainAssemblyDependencies = mainAssemblyNuGetReferences
                .Union(mainAssemblyReferences, StringComparer.OrdinalIgnoreCase);

            var mainAssemblyDependencyNames = mainAssemblyDependencies
                .Where(_ => File.Exists(_))
                .Select(_ => new FileInfo(_).Name);

            return GetDirectoryFiles(mainAssemblyDependencyNames);
        }

        private static IEnumerable<FileInfo> GetDirectoryFiles(IEnumerable<string> include)
        {
            include = include.Stale();

            return Directory
                .GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
                .Select(_ => new FileInfo(_))
                .Where(_ => include.Contains(_.Name, StringComparer.OrdinalIgnoreCase));
        }

        private IEnumerable<string> GetFlatNuGetReferenceFiles(Assembly assembly)
        {
            return nuGetPackageResolver
                .GetPackageIds(assembly)
                .Select(_ => GetFilePath(_))
                .SelectMany(_ => GetReferencesForFile(_).Concat(new[] { _ }));
        }

        private IEnumerable<string> GetReferencesForFile(string file)
        {
            if (!File.Exists(file))
            {
                return new string[0];
            }

            return GetReferencesForAssembly(Assembly.ReflectionOnlyLoadFrom(file));
        }

        private IEnumerable<string> GetReferencesForAssembly(Assembly assembly)
        {
            return assembly
                .GetReferencedAssemblies()
                .Select(_ => GetFilePath(_.Name));
        }

        private string GetFilePath(string file) =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{file}.dll");
    }
}
