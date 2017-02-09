using System;

namespace Zapp.Core.Clauses
{
    /// <summary>
    /// Represents a static class for object validation
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Validates if the param is null.
        /// </summary>
        /// <param name="paramValue">Value of the parameter.</param>
        /// <param name="paramName">Name of the parameter.</param>
        public static void ParamNotNull(object paramValue, string paramName)
        {
            if (paramValue == null) throw new ArgumentNullException(paramName);
        }

        /// <summary>
        /// Validates if the param is null or empty.
        /// </summary>
        /// <param name="paramValue">Value of the parameter.</param>
        /// <param name="paramName">Name of the parameter.</param>
        public static void ParamNotNullOrEmpty(string paramValue, string paramName)
        {
            if (string.IsNullOrEmpty(paramValue)) throw new ArgumentException("Must be non-empty.", paramName);
        }

        /// <summary>
        /// Validates if the param is between bounds.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramValue">Value of the parameter.</param>
        /// <param name="lowerBounds">Bounds of the lower value.</param>
        /// <param name="upperBounds">Bounds of the upper value.</param>
        /// <param name="paramName">Name of the parameter.</param>
        public static void ParamNotOutOfRange<T>(T paramValue, T lowerBounds, T upperBounds, string paramName) where T : IComparable<T>
        {
            if (paramValue?.CompareTo(lowerBounds) < 0) throw new ArgumentOutOfRangeException(paramName, $"Must be greater than {lowerBounds}");
            if (paramValue?.CompareTo(upperBounds) > 0) throw new ArgumentOutOfRangeException(paramName, $"Must be less than {upperBounds}");
        }
    }
}
