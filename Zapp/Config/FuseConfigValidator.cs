using FluentValidation;

namespace Zapp.Config
{
    /// <summary>
    /// Represents a validator for <see cref="FuseConfig"/>.
    /// </summary>
    public class FuseConfigValidator : AbstractValidator<FuseConfig>
    {
        private const string fusionIdFormat = "{fusionId}";

        /// <summary>
        /// Initializes a new <see cref="FuseConfigValidator"/>.
        /// </summary>
        /// <param name="fusionValidator">Validator for the <see cref="FuseConfig.Fusions"/> property.</param>
        public FuseConfigValidator(
            IValidator<FusePackConfig> fusionValidator)
        {
            RuleFor(_ => _.RootDirectory).NotEmpty();
            RuleFor(_ => _.EntryPattern).NotEmpty();

            RuleFor(_ => _.FusionDirectoryPattern).NotNull()
                .Must(_ => _.Contains(fusionIdFormat))
                .WithMessage($"Must contain: '{fusionIdFormat}'");

            RuleFor(_ => _.Fusions).NotNull();
            RuleForEach(_ => _.Fusions).SetValidator(fusionValidator);
        }
    }
}
