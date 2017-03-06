using System.Collections.Generic;
using System.Reflection;
using System.Web.Http.Dispatcher;

namespace Zapp.Core.Owin
{
    /// <summary>
    /// Represents an override of <see cref="DefaultAssembliesResolver"/>.
    /// </summary>
    public class PredefinedAssembliesResolver : DefaultAssembliesResolver
    {
        private readonly Assembly[] assemblies;

        /// <summary>
        /// Initializes a new <see cref="PredefinedAssembliesResolver"/>.
        /// </summary>
        public PredefinedAssembliesResolver(Assembly[] assemblies)
        {
            this.assemblies = assemblies;
        }

        /// <inheritdoc />
        public override ICollection<Assembly> GetAssemblies() => assemblies;
    }
}
