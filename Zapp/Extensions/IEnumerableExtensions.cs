using System.Collections.Generic;
using System.Linq;

namespace Zapp.Extensions
{
    /// <summary>
    /// Represents a set of extensions for the <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Gets a stale version of the <paramref name="source"/> to re-use the outcome.
        /// </summary>
        /// <typeparam name="T">Type of the items in <paramref name="source"/>.</typeparam>
        /// <param name="source"><see cref="IEnumerable{T}"/> that needs to be stale.</param>
        public static IEnumerable<T> Stale<T>(this IEnumerable<T> source) =>
            source as T[] ?? source?.ToArray() ?? new T[0];

        /// <summary>
        /// Gets a stale and read-only version of the <paramref name="source"/> to re-use the outcome.
        /// </summary>
        /// <typeparam name="T">Type of the items in <paramref name="source"/>.</typeparam>
        /// <param name="source"><see cref="IEnumerable{T}"/> that needs to be stale.</param>
        public static IReadOnlyCollection<T> StaleReadOnly<T>(this IEnumerable<T> source) =>
            source as T[] ?? source?.ToArray() ?? new T[0];
    }
}
