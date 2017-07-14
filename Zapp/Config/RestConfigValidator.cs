using FluentValidation;
using System.Net;

namespace Zapp.Config
{
    /// <summary>
    /// Represents a validator for <see cref="RestConfig"/>.
    /// </summary>
    public class RestConfigValidator : AbstractValidator<RestConfig>
    {
        /// <summary>
        /// Initializes a new <see cref="RestConfigValidator"/>.
        /// </summary>
        public RestConfigValidator()
        {
            RuleFor(_ => _.IpAddressPattern).NotEmpty();
            RuleFor(_ => _.Port).GreaterThan(IPEndPoint.MinPort).LessThan(IPEndPoint.MaxPort);
        }
    }
}
