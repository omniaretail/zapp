using System.Collections.Generic;
using System.Linq;

namespace Zapp.Extensions
{
    /// <summary>
    /// Represents a set of extensions methods for the <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Exhausts the <see cref="IEnumerable{T}"/> and returns a stale collection.
        /// </summary>
        /// <typeparam name="T">Type of the source it's items.</typeparam>
        /// <param name="source">The collection that needs to be exhausted.</param>
        public static T[] Exhaust<T>(this IEnumerable<T> source) => 
            source as T[] ?? source.ToArray();
    }
}
