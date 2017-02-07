using System;

namespace Zapp.Utils
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
    }
}
