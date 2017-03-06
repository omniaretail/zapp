using System.Reflection;
using System.Web.Http.Dispatcher;

namespace Zapp.Core.Owin
{
    /// <summary>
    /// Represents an interface for creating instances of <see cref="IAssembliesResolver"/>.
    /// </summary>
    public interface IAssembliesResolverFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="IAssembliesResolver"/>.
        /// </summary>
        /// <param name="assemblies">Collection of assemblies to resolve.</param>
        IAssembliesResolver CreateNew(params Assembly[] assemblies);
    }
}
