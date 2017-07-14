using System.Collections.Generic;

namespace Zapp.Pack
{
    /// <summary>
    /// Represents a validator for <see cref="PackageVersion"/>.
    /// </summary>
    public interface IPackageVersionValidator
    {
        /// <summary>
        /// Validates if a <see cref="PackageVersion"/> is available to load.
        /// </summary>
        /// <param name="versions">The instances of <see cref="PackageVersion"/> that needs to be validated.</param>
        void ConfirmAvailability(IEnumerable<PackageVersion> versions);
    }
}