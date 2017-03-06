using System.Collections.Generic;
using System.Reflection;
using System.Web.Http.Dispatcher;

namespace Zapp.Core.Owin
{
    /// <summary>
    /// Represents an override of <see cref="DefaultAssembliesResolver"/>.
    /// </summary>
    public class CallingAssembliesResolver : DefaultAssembliesResolver
    {
        private readonly Assembly bootstrappingAssembly;

        /// <summary>
        /// Initializes a new <see cref="CallingAssembliesResolver"/>.
        /// </summary>
        public CallingAssembliesResolver()
        {
            bootstrappingAssembly = Assembly.GetCallingAssembly();
        }

        /// <inheritdoc />
        public override ICollection<Assembly> GetAssemblies() => new[] { bootstrappingAssembly };
    }
}
