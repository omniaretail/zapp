using Ninject.Modules;
using System.Web.Http.Dispatcher;
using Zapp.Core.NuGet;
using Zapp.Core.Owin;

namespace Zapp.Core
{
    /// <summary>
    /// Represents a class with the core bindings for zapp.
    /// </summary>
    public class ZappCoreModule : NinjectModule
    {
        /// <inheritdoc />
        public override void Load()
        {
            Bind<IAssembliesResolver>().To<CallingAssembliesResolver>().InSingletonScope();
            Bind<INuGetPackageResolver>().To<XmlNuGetPackageResolver>().InSingletonScope();
        }
    }
}
