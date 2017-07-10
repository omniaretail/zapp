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
        private readonly IFusionCatalogue fusionCatalogue;

        /// <summary>
        /// Initializes a new <see cref="FusionMaid"/> with it's dependencies.
        /// </summary>
        /// <param name="fusionCatalogue">Catalogue used to trace the location of fusion(s).</param>
        public FusionMaid(
            IFusionCatalogue fusionCatalogue)
        {
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
                }
                catch (Exception) { }
            }
        }
    }
}
