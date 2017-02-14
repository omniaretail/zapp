using Ninject.Modules;
using Zapp.Process.Client;
using Zapp.Process.Controller;
using Zapp.Process.Libraries;
using Zapp.Process.Meta;
using Zapp.Process.Rest;

namespace Zapp.Process
{
    /// <summary>
    /// Represents the main ninject-module for the zapp-process.
    /// </summary>
    public class ZappProcessModule : NinjectModule
    {
        /// <summary>
        /// Registers all bindings required for this assembly.
        /// </summary>
        /// <inheritdoc />
        public override void Load()
        {
            Bind<IZappProcess>().To<ZappProcess>().InSingletonScope();

            Bind<IProcessController>().To<ProcessController>().InSingletonScope();
            Bind<IPortProvider>().To<FuzzyPortProvider>().InSingletonScope();

            Bind<IZappClient>().To<RestZappClient>().InSingletonScope();

            Bind<ILibraryService>().To<LibraryService>().InSingletonScope();
            Bind<IRestService>().To<OwinRestService>().InSingletonScope();
            Bind<IMetaService>().To<StandardMetaService>().InSingletonScope();
        }
    }
}
