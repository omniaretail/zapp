using EnsureThat;
using System;
using System.Collections.Generic;
using System.Linq;
using Zapp.Exceptions;
using Zapp.Extensions;

namespace Zapp.Pack
{
    /// <summary>
    /// Represents a validator that checks <see cref="PackageVersion"/> if they are available or not.
    /// </summary>
    public class PackageVersionValidator : IPackageVersionValidator
    {
        private readonly IPackService packService;

        /// <summary>
        /// Creates a new instance of the <see cref="PackageVersionValidator"/> with the 
        /// necessary dependencies.
        /// </summary>
        /// <param name="packService">The service that is used to check if a package is deployed.</param>
        public PackageVersionValidator(IPackService packService)
        {
            this.packService = packService;
        }

        /// <summary>
        /// Validates if a <see cref="PackageVersion"/> is available to load.
        /// </summary>
        /// <param name="versions">The instances of <see cref="PackageVersion"/> that needs to be validated.</param>
        /// <exception cref="AggregateException">The <paramref name="versions"/> argument is null.</exception>
        /// <inheritdoc />
        public void ConfirmAvailability(IEnumerable<PackageVersion> versions)
        {
            EnsureArg.IsNotNull(versions, nameof(versions));

            var nonExistingPackages = versions
                .Where(_ =>
                    _.IsUnknown ||
                    !packService.IsPackageVersionDeployed(_))
                .Stale();

            if (nonExistingPackages.Any())
            {
                var errors = nonExistingPackages
                    .Select(_ => new PackageException(PackageException.NotFound, _));

                throw new AggregateException(errors);
            }
        }
    }
}
