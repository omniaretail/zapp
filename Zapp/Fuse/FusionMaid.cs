using log4net;
using System;
using System.IO;
using Zapp.Catalogue;

namespace Zapp.Fuse
{
    /// <summary>
    /// Represents a maid for the fusion stored as directories on disk.
    /// </summary>
    /// <inheritDoc />
    public class FusionMaid : IFusionMaid
    {
        private readonly ILog logService;
        private readonly IFusionCatalogue fusionCatalogue;

        /// <summary>
        /// Initializes a new <see cref="FusionMaid"/> with it's dependencies.
        /// </summary>
        /// <param name="logService">Service used for logging.</param>
        /// <param name="fusionCatalogue">Catalogue used to trace the location of fusion(s).</param>
        public FusionMaid(
            ILog logService,
            IFusionCatalogue fusionCatalogue)
        {
            this.logService = logService;
            this.fusionCatalogue = fusionCatalogue;
        }

        /// <summary>
        /// Requests the maid to clean the old fusion's directories.
        /// </summary>
        /// <param name="fusionId">The id of the fusion where the old directories needs to becleaned of.</param>
        /// <inheritDoc />
        public void CleanAll(string fusionId)
        {
            var fusions = fusionCatalogue
                .GetAllLocations(fusionId);

            foreach (var fusion in fusions)
            {
                try
                {
                    Directory.Delete(fusion, true);

                    logService.Debug($"Fusion directory: '{fusion}' has been removed.");
                }
                catch (Exception ex) when (
                    ex is IOException || 
                    ex is UnauthorizedAccessException || 
                    ex is DirectoryNotFoundException
                )
                {
                    logService.Warn($"Fusion directory: '{fusion}' failed to delete due: '{ex.Message}'.");
                }
            }
        }
    }
}
