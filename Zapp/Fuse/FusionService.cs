using AntPathMatching;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Zapp.Config;
using Zapp.Pack;
using Zapp.Sync;
using Zapp.Utils;

namespace Zapp.Fuse
{
    /// <summary>
    /// Represents an implementation of <see cref="IFusionService"/>.
    /// </summary>
    public class FusionService : IFusionService
    {
        private readonly IConfigStore configStore;

        private readonly IPackService packService;
        private readonly ISyncService syncService;

        private readonly IAnt entryFilter;

        private readonly IFusionFactory fusionFactory;
        private readonly IFusionExtracter fusionExtractor;

        private readonly IReadOnlyCollection<IFusionFilter> fusionFilters;

        /// <summary>
        /// Initializes a new <see cref="FusionService"/>.
        /// </summary>
        /// <param name="configStore">Store used for loading configuration.</param>
        /// <param name="packService">Service used for loading packages.</param>
        /// <param name="syncService">Service used for synchronization of package versions.</param>
        /// <param name="antFactory">Factory used for creating <see cref="IAnt"/> instances.</param>
        /// <param name="fusionFactory">Factory used for creating <see cref="IFusion"/> instances.</param>
        /// <param name="fusionExtractor">Extracttor used for extracting streams of fusions.</param>
        /// <param name="fusionFilters">Filters used for decorating fusion entries.</param>
        public FusionService(
            IConfigStore configStore,
            IPackService packService,
            ISyncService syncService,
            IAntFactory antFactory,
            IFusionFactory fusionFactory,
            IFusionExtracter fusionExtractor,
            IEnumerable<IFusionFilter> fusionFilters)
        {
            this.configStore = configStore;

            this.packService = packService;
            this.syncService = syncService;

            this.fusionFactory = fusionFactory;
            this.fusionExtractor = fusionExtractor;

            this.fusionFilters = fusionFilters.ToList();

            entryFilter = antFactory.CreateNew(configStore?.Value?.Fuse?.EntryPattern);
        }

        /// <summary>
        /// Starts to fuse all the packages.
        /// </summary>
        /// <inheritdoc />
        public void Start()
        {
            var fusions = configStore.Value.Fuse.Fusions;

            foreach (var fusion in fusions)
            {
                if (!TryFuseLatest(fusion))
                {
                    throw new InvalidOperationException();
                }
            }
        }

        /// <summary>
        /// Tries to fuse the latest possible version of the fusion.
        /// </summary>
        /// <param name="config">Configuration for the fusion</param>
        /// <exception cref="ArgumentNullException">Throw when <paramref name="config"/> is not set.</exception>
        public bool TryFuseLatest(FusePackConfig config)
        {
            Guard.ParamNotNull(config, nameof(config));

            var syncDeployVersions = config.PackageIds
                .ToDictionary(k => k, k => syncService.Sync(k));

            if (syncDeployVersions.Values.Any(string.IsNullOrEmpty))
            {
                return false;
            }

            var packageVersions = syncDeployVersions
                .Select(kvp => new PackageVersion(kvp.Key, kvp.Value))
                .ToList();

            if (packageVersions.Any(v => !packService.IsPackageVersionDeployed(v)))
            {
                return false;
            }

            using (var stream = new MemoryStream())
            {
                var fusion = fusionFactory.CreateNew(stream);

                try
                {
                    var packages = packageVersions
                        .Select(v => packService.LoadPackage(v))
                        .ToList();

                    foreach (var package in packages)
                    {
                        foreach (var entry in package.GetEntries())
                        {
                            if (entryFilter.IsMatch(entry.Name))
                            {
                                AddEntryToFusion(fusion, entry, config);
                            }
                        }
                    }

                    var metaEntry = new FusionMetaEntry();
                    metaEntry.SetInfo("entry.file", "Zapp.Process.exe");

                    AddEntryToFusion(fusion, metaEntry, config);
                    AddEntryToFusion(fusion, new FusionProcessEntry(), config);
                }
                finally
                {
                    (fusion as IDisposable)?.Dispose();
                }

                // todo: check if this solution is possible: http://stackoverflow.com/a/21991099
                var evaluated = stream.ToArray();

                using (var readable = new MemoryStream(evaluated))
                {
                    fusionExtractor.Extract(config, readable);
                }
            }

            return true;
        }

        /// <summary>
        /// Searches for affected fusion packages.
        /// </summary>
        /// <param name="packageId">Identity of the package.</param>
        /// <inheritdoc />
        public IReadOnlyCollection<string> GetAffectedFusions(string packageId)
        {
            Guard.ParamNotNullOrEmpty(packageId, nameof(packageId));

            var fusions = configStore.Value.Fuse.Fusions;

            return fusions
                .Where(f => f.PackageIds.Contains(packageId, StringComparer.OrdinalIgnoreCase))
                .Select(f => f.Id)
                .ToList();
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
    }
}
