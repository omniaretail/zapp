using System.Collections.Generic;

namespace Zapp.Catalogue
{
    /// <summary>
    /// Represents a file catalogue for the fusions.
    /// </summary>
    public interface IFusionCatalogue
    {
        /// <summary>
        /// Gets the active location of the requested fusion.
        /// </summary>
        /// <param name="fusionId">Id of the requested fusion.</param>
        string GetActiveLocation(string fusionId);

        /// <summary>
        /// Gets all the locations of the requested fusion.
        /// </summary>
        /// <param name="fusionId">Id of the requested fusion.</param>
        IEnumerable<string> GetAllLocations(string fusionId);

        /// <summary>
        /// Creates a new location for the requested fusion.
        /// </summary>
        /// <param name="fusionId">Id of the requested fusion.</param>
        string CreateLocation(string fusionId);
    }
}
