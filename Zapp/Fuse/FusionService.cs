using EnsureThat;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zapp.Config;
using Zapp.Deploy;
using Zapp.Exceptions;
using Zapp.Extensions;
using Zapp.Pack;
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
        private readonly ISyncService syncService;

        private readonly IFusionFactory fusionFactory;
        private readonly IFusionExtracter fusionExtractor;
        private readonly IFusionBuilder fusionBuilder;

        private readonly IDeployAnnouncementFactory announcementFactory;
        private readonly IPackageVersionValidator packageVersionValidator;

        /// <summary>
        /// Initializes a new <see cref="FusionService"/>.
        /// </summary>
        /// <param name="logService">Service used for logging.</param>
        /// <param name="configStore">Store used for loading configuration.</param>
        /// <param name="syncService">Service used for synchronization of package versions.</param>
        /// <param name="fusionFactory">Factory used for creating <see cref="IFusion"/> instances.</param>
        /// <param name="fusionExtractor">Extractor used for extracting streams of fusions.</param>
        /// <param name="fusionBuilder">Builder used to build a <see cref="IFusion"/> before it's extracted.</param>
        /// <param name="announcementFactory">Factory used for creating instances of <see cref="IDeployAnnouncement"/>.</param>
        /// <param name="packageVersionValidator">Validator used for validating <see cref="PackageVersion"/> availability.</param>
        public FusionService(
            ILog logService,
            IConfigStore configStore,
            ISyncService syncService,
            IFusionFactory fusionFactory,
            IFusionExtracter fusionExtractor,
            IFusionBuilder fusionBuilder,
            IDeployAnnouncementFactory announcementFactory,
            IPackageVersionValidator packageVersionValidator)
        {
            this.logService = logService;

            this.configStore = configStore;
            this.syncService = syncService;

            this.fusionFactory = fusionFactory;
            this.fusionExtractor = fusionExtractor;
            this.fusionBuilder = fusionBuilder;

            this.announcementFactory = announcementFactory;
            this.packageVersionValidator = packageVersionValidator;
        }

        /// <summary>
        /// Extracts all configured fusions.
        /// </summary>
        /// <param name="token">Token of cancellation.</param>
        /// <inheritdoc />
        /// <exception cref="AggregateException">Throw when one or more fusions failed to extract.</exception>
        public void ExtractAll(CancellationToken token)
        {
            var fusionIds = configStore.Value?.Fuse?.Fusions?
                .Select(_ => _.Id)?
                .Stale() ?? new string[0];

            var announcement = announcementFactory
                .CreateNew(fusionIds, new PackageVersion[0]);

            Extract(announcement, token);
        }

        /// <summary>
        /// Extracts a deploy anncouncement.
        /// </summary>
        /// <param name="announcement">The announcement that needs to be extracted.</param>
        /// <param name="token">Token of cancellation.</param>
        /// <inheritdoc />
        /// <exception cref="AggregateException">Throw when one or more fusions failed to extract.</exception>
        public void Extract(IDeployAnnouncement announcement, CancellationToken token)
        {
            EnsureArg.IsNotNull(announcement, nameof(announcement));

            var opts = new ParallelOptions
            {
                CancellationToken = token
            };

            var fusionIds = announcement.GetFusionIds();

            Parallel.ForEach(fusionIds, opts, (_, state) =>
            {
                try
                {
                    ExtractFusion(_, announcement);
                }
                catch (Exception ex)
                {
                    state.Stop();

                    throw new FusionException(FusionException.ExtractionFailure, _, ex);
                }
            });
        }

        /// <summary>
        /// Searches for affected fusion packages.
        /// </summary>
        /// <param name="packageId">Identity of the package.</param>
        /// <inheritdoc />
        public IEnumerable<string> GetAffectedFusions(string packageId)
        {
            EnsureArg.IsNotNullOrEmpty(packageId, nameof(packageId));

            return configStore.Value?.Fuse?.Fusions?
                .Where(_ => _.PackageIds.Contains(packageId, StringComparer.OrdinalIgnoreCase))?
                .Select(_ => _.Id) ?? new string[0];
        }

        /// <summary>
        /// Gets the package versions from the sync-service for a specific fusion.
        /// </summary>
        /// <param name="fusionId">Identity of the fusion.</param>
        /// <exception cref="ArgumentException">Throw when <paramref name="fusionId"/> is not set.</exception>
        /// <inheritdoc />
        public IEnumerable<PackageVersion> GetPackageVersions(string fusionId) // todo make this async somehow..
        {
            EnsureArg.IsNotNullOrEmpty(fusionId, nameof(fusionId));

            return GetFusionConfig(fusionId)?.PackageIds?
                .Select(_ => new PackageVersion(_, syncService.GetVersionAsync(_).Result)) ?? new PackageVersion[0];
        }

        private void ExtractFusion(string fusionId, IDeployAnnouncement announcement)
        {
            EnsureArg.IsNotNullOrEmpty(fusionId, nameof(fusionId));
            EnsureArg.IsNotNull(announcement, nameof(announcement));

            var fusionConfig = GetFusionConfig(fusionId);

            var packageVersions = announcement
                .GetPackageVersions(fusionConfig.Id)
                .Stale();

            packageVersionValidator
                .ConfirmAvailability(packageVersions);

            using (var packageStream = new MemoryStream())
            {
                var fusion = fusionFactory
                    .CreateNew(packageStream);

                try
                {
                    fusionBuilder.Build(fusionConfig, fusion, packageVersions);
                }
                finally
                {
                    (fusion as IDisposable)?.Dispose();
                }

                using (var packageStreamReadable = new MemoryStream(packageStream.ToArray()))
                {
                    fusionExtractor.Extract(fusionConfig, packageStreamReadable);
                }
            }
        }

        private FusePackConfig GetFusionConfig(string fusionId)
        {
            var result = configStore.Value?.Fuse?.Fusions?
                .SingleOrDefault(_ => string.Equals(_.Id, fusionId, StringComparison.OrdinalIgnoreCase));

            if (result == null)
            {
                throw new FusionException(FusionException.NotFound, fusionId);
            }

            return result;
        }
    }
}
