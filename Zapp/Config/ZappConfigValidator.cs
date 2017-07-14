using FluentValidation;

namespace Zapp.Config
{
    /// <summary>
    /// Represents a validator for <see cref="ZappConfig"/>.
    /// </summary>
    public class ZappConfigValidator : AbstractValidator<ZappConfig>
    {
        /// <summary>
        /// Initializes a new <see cref="ZappConfigValidator"/>.
        /// </summary>
        /// <param name="restValidator">Validator for the <see cref="ZappConfig.Rest"/> property.</param>
        /// <param name="packValidator">Validator for the <see cref="ZappConfig.Pack"/> property.</param>
        /// <param name="fuseValidator">Validator for the <see cref="ZappConfig.Fuse"/> property.</param>
        /// <param name="syncValidator">Validator for the <see cref="ZappConfig.Sync"/> property.</param>
        public ZappConfigValidator(
            IValidator<RestConfig> restValidator,
            IValidator<PackConfig> packValidator,
            IValidator<FuseConfig> fuseValidator,
            IValidator<SyncConfig> syncValidator)
        {
            RuleFor(_ => _.Rest).SetValidator(restValidator);
            RuleFor(_ => _.Pack).SetValidator(packValidator);
            RuleFor(_ => _.Fuse).SetValidator(fuseValidator);
            RuleFor(_ => _.Sync).SetValidator(syncValidator);
        }
    }
}
