using FluentValidation;

namespace Zapp.Config
{
    /// <summary>
    /// Represents a validator for <see cref="PackConfig"/>.
    /// </summary>
    public class PackConfigValidator : AbstractValidator<PackConfig>
    {
        private const string deployVersionFormat = "{deployVersion}";
        private const string packageIdFormat = "{packageId}";

        /// <summary>
        /// Initializes a new <see cref="PackConfigValidator"/>.
        /// </summary>
        public PackConfigValidator()
        {
            RuleFor(_ => _.RootDirectory).NotEmpty();

            RuleFor(_ => _.PackagePattern)
                .NotEmpty()
                .Must(_ => _.Contains(deployVersionFormat))
                .WithMessage($"Must contain: '{deployVersionFormat}'")
                .Must(_ => _.Contains(packageIdFormat))
                .WithMessage($"Must contain: '{packageIdFormat}'");
        }
    }
}
