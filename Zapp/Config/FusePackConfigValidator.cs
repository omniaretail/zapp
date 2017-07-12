using FluentValidation;

namespace Zapp.Config
{
    /// <summary>
    /// Represents a validator for <see cref="FusePackConfig"/>.
    /// </summary>
    public class FusePackConfigValidator : AbstractValidator<FusePackConfig>
    {
        /// <summary>
        /// Initializes a new <see cref="FusePackConfigValidator"/>.
        /// </summary>
        public FusePackConfigValidator()
        {
            RuleFor(_ => _.Id).NotEmpty();
            RuleFor(_ => _.PackageIds).NotEmpty();
        }
    }
}
