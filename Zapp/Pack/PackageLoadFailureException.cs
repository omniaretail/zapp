using System;

namespace Zapp.Pack
{
    /// <summary>
    /// Represents error for loading an invalid packages.
    /// </summary>
    [Serializable]
    public class PackageLoadFailureException : Exception
    {
        /// <summary>
        /// Initializes a new <see cref="PackageLoadFailureException"/>.
        /// </summary>
        /// <param name="message">The message of the error.</param>
        /// <param name="inner">The inner <see cref="Exception"/> of the error.</param>
        public PackageLoadFailureException(string message, Exception inner)
            : base(message, inner) { }
    }
}
