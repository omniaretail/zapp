using AntPathMatching;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Zapp.Config;
using Zapp.Core.Clauses;
using Zapp.Core.NuGet;
using Zapp.Pack;
using Zapp.Process;
using Zapp.Sync;

namespace Zapp.Fuse
{
    /// <summary>
    /// Represents an implementation of <see cref="IFusionService"/>.
    /// </summary>
    public class FusionService : IFusionService
    {
        private readonly ILog logService;

        private readonly IConfigStore configStore;

        private readonly IPackService packService;
        private readonly ISyncService syncService;

        private readonly IAnt entryFilter;

        private readonly IFusionFactory fusionFactory;
        private readonly IFusionExtracter fusionExtractor;

        private readonly IReadOnlyCollection<IFusionFilter> fusionFilters;

        private readonly INuGetPackageResolver nuGetPackageResolver;
        private readonly IFrameworkPackageEntryFactory frameworkPackageEntryFactory;

        private IReadOnlyCollection<FileInfo> defaultEntries;

        /// <summary>
        /// Initializes a new <see cref="FusionService"/>.
        /// </summary>
        /// <param name="logService">Service used for logging.</param>
        /// <param name="configStore">Store used for loading configuration.</param>
        /// <param name="packService">Service used for loading packages.</param>
        /// <param name="syncService">Service used for synchronization of package versions.</param>
        /// <param name="antFactory">Factory used for creating <see cref="IAnt"/> instances.</param>
        /// <param name="fusionFactory">Factory used for creating <see cref="IFusion"/> instances.</param>
        /// <param name="fusionExtractor">Extracttor used for extracting streams of fusions.</param>
        /// <param name="fusionFilters">Filters used for decorating fusion entries.</param>
        /// <param name="nuGetPackageResolver">Resolver used to resolve NuGet packages.</param>
        /// <param name="frameworkPackageEntryFactory">Factory used for creati9ng <see cref="IFrameworkPackageEntry"/> instances.</param>
        public FusionService(
            ILog logService,
            IConfigStore configStore,
            IPackService packService,
            ISyncService syncService,
            IAntFactory antFactory,
            IFusionFactory fusionFactory,
            IFusionExtracter fusionExtractor,
            IEnumerable<IFusionFilter> fusionFilters,
            INuGetPackageResolver nuGetPackageResolver,
            IFrameworkPackageEntryFactory frameworkPackageEntryFactory)
        {
            this.logService = logService;

            this.configStore = configStore;

            this.packService = packService;
            this.syncService = syncService;

            this.fusionFactory = fusionFactory;
            this.fusionExtractor = fusionExtractor;

            this.fusionFilters = fusionFilters.ToList();

            this.nuGetPackageResolver = nuGetPackageResolver;
            this.frameworkPackageEntryFactory = frameworkPackageEntryFactory;

            entryFilter = antFactory.CreateNew(configStore?.Value?.Fuse?.EntryPattern);
            defaultEntries = GetDefaultEntries();
        }

        /// <summary>
        /// Starts to fuse all the packages.
        /// </summary>
        /// <inheritdoc />
        public bool TryExtract()
        {
            var fusionIds = configStore.Value?.Fuse?.Fusions?
                .Select(f => f.Id)?
                .ToList() ?? new List<string>();

            bool globalExtractionResult = TryExtractFusionBatch(fusionIds);

            if (!globalExtractionResult)
            {
                logService.Error($"Failed to extract all fusions.");
            }

            return globalExtractionResult;
        }

        /// <summary>
        /// Tries to create a new fusion extraction.
        /// </summary>
        /// <param name="fusionId">Identity of the fusion.</param>
        /// <exception cref="ArgumentException">Throw when <paramref name="fusionId"/> is not set.</exception>
        public bool TryExtractFusion(string fusionId)
        {
            Guard.ParamNotNullOrEmpty(fusionId, nameof(fusionId));

            var fusionConfig = GetFusionConfig(fusionId);
            var packageVersions = GetPackageVersions(fusionConfig.Id);

            if (packageVersions.Any(v => v.IsUnknown) ||
                packageVersions.Any(v => !packService.IsPackageVersionDeployed(v)))
            {
                logService.Warn($"Some of the packages for fusion {fusionId} are unknown or not deployed yet.");
                return false;
            }

            using (var packageStream = new MemoryStream())
            {
                var fusion = fusionFactory.CreateNew(packageStream);
                var packages = default(List<IPackage>);

                try
                {
                    packages = packageVersions
                        .Select(v => packService.LoadPackage(v))
                        .ToList();

                    var entries = PrioritizeEntries(GenerateDefaultEntries(), packages);

                    foreach (var entry in entries)
                    {
                        AddEntryToFusion(fusion, entry, fusionConfig);

                        logService.Debug($"Entry {entry.Name} added to fusion: {fusionId}");
                    }
                }
                finally
                {
                    (fusion as IDisposable)?.Dispose();

                    if (packages != null)
                    {
                        foreach (var package in packages)
                        {
                            (package as IDisposable)?.Dispose();
                        }
                    }
                }

                using (var packageStreamReadable = new MemoryStream(packageStream.ToArray()))
                {
                    fusionExtractor.Extract(fusionConfig, packageStreamReadable);
                }
            }

            return true;
        }

        /// <summary>
        /// Tries to create new fusion extractions.
        /// </summary>
        /// <param name="fusionIds">Identities of the fusion.</param>
        /// <inheritdoc />
        public bool TryExtractFusionBatch(IReadOnlyCollection<string> fusionIds) =>
            fusionIds.All(f => TryExtractFusion(f)) == true;

        /// <summary>
        /// Searches for affected fusion packages.
        /// </summary>
        /// <param name="packageId">Identity of the package.</param>
        /// <inheritdoc />
        public IReadOnlyCollection<string> GetAffectedFusions(string packageId)
        {
            Guard.ParamNotNullOrEmpty(packageId, nameof(packageId));

            return configStore.Value?.Fuse?.Fusions?
                .Where(f => f.PackageIds.Contains(packageId, StringComparer.OrdinalIgnoreCase))?
                .Select(f => f.Id)?
                .ToList() ?? new List<string>();
        }

        /// <summary>
        /// Gets the package versions from the sync-service for a specific fusion.
        /// </summary>
        /// <param name="fusionId">Identity of the fusion.</param>
        /// <exception cref="ArgumentException">Throw when <paramref name="fusionId"/> is not set.</exception>
        /// <inheritdoc />
        public IReadOnlyCollection<PackageVersion> GetPackageVersions(string fusionId)
        {
            Guard.ParamNotNullOrEmpty(fusionId, nameof(fusionId));

            return GetFusionConfig(fusionId)?.PackageIds?
                .Select(p => new PackageVersion(p, syncService.Sync(p)))?
                .ToList() ?? new List<PackageVersion>();
        }

        private FusePackConfig GetFusionConfig(string fusionId)
        {
            var result = configStore.Value?.Fuse?.Fusions?
                .SingleOrDefault(f => string.Equals(f.Id, fusionId, StringComparison.OrdinalIgnoreCase));

            if (result == null)
            {
                throw new KeyNotFoundException($"Fusion: {fusionId} not found.");
            }
            else
            {
                return result;
            }
        }

        private void AddEntryToFusion(
            IFusion fusion,
            IPackageEntry entry,
            FusePackConfig config)
        {
            foreach (var filter in fusionFilters)
            {
                filter.BeforeAddEntry(config, entry);
            }

            fusion.AddEntry(entry);
        }

        private IReadOnlyCollection<IPackageEntry> PrioritizeEntries(
            IReadOnlyCollection<IPackageEntry> standardEntries,
            IReadOnlyCollection<IPackage> packages)
        {
            return standardEntries.Concat(packages
                .SelectMany(p => p.GetEntries())
                .Where(e => entryFilter.IsMatch(e.Name)))
                .GroupBy(e => e.Name, StringComparer.OrdinalIgnoreCase)
                .Select(e => e.FirstOrDefault())
                .ToList();
        }

        private IReadOnlyCollection<IPackageEntry> GenerateDefaultEntries()
        {
            return defaultEntries
                .Select(f => new LazyPackageEntry(f.Name, new LazyStream(() => f.OpenRead())))
                .Concat(frameworkPackageEntryFactory
                    .CreateNew()
                    .Cast<IPackageEntry>())
                .ToList();
        }

        private IReadOnlyCollection<FileInfo> GetDefaultEntries()
        {
            var assembly = typeof(ZappProcessModule).Assembly;

            var references = nuGetPackageResolver
                .GetPackageIds(assembly)
                .Select(id => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{id}.dll"))
                .Where(File.Exists)
                .SelectMany(e => GetAssemblyReferences(e).Concat(new[] { e }))
                .Concat(GetAssemblyReferences(assembly))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Where(File.Exists)
                .Select(e => new FileInfo(e).Name)
                .ToList();

            return Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
                .Select(f => new FileInfo(f))
                .Where(i => references.Contains(i.Name, StringComparer.OrdinalIgnoreCase))
                .ToList();
        }

        private string GetReferencePath(string fileName) => Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, fileName);

        private IReadOnlyCollection<string> GetAssemblyReferences(string assemblyPath) =>
            GetAssemblyReferences(Assembly.ReflectionOnlyLoadFrom(assemblyPath));

        private IReadOnlyCollection<string> GetAssemblyReferences(Assembly assembly)
        {
            return assembly
                .GetReferencedAssemblies()
                .Select(p => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{p.Name}.dll"))
                .ToList();
        }
    }
}
