using FluentValidation;

namespace Zapp.Config
{
    /// <summary>
    /// Represents a validator for <see cref="SyncConfig"/>.
    /// </summary>
    public class SyncConfigValidator : AbstractValidator<SyncConfig>
    {
        /// <summary>
        /// Initializes a new <see cref="FusePackConfigValidator"/>.
        /// </summary>
        public SyncConfigValidator()
        {
            RuleFor(_ => _.ConnectionString).NotEmpty();
        }
    }
}
