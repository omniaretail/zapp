using AntPathMatching;
using log4net;
using Ninject.Extensions.Factory;
using Ninject.Modules;
using Zapp.Config;
using Zapp.Pack;
using Zapp.Rest;
using Zapp.Server;
using Zapp.Sync;

namespace Zapp
{
    /// <summary>
    /// Represents the <see cref="NinjectModule"/> version of the zapp package.
    /// </summary>
    public class ZappModule : NinjectModule
    {
        /// <summary>
        /// Registers all the required bindings to get the package working.
        /// </summary>
        public override void Load()
        {
            Bind<ILog>().ToMethod(ctx => LogManager.GetLogger(ctx.Request.Target.Member.DeclaringType));

            Bind<IConfigStore>().To<JsonConfigStore>().InSingletonScope();

            Bind<IZappServer>().To<ZappServer>().InSingletonScope();
            Bind<IRestService>().To<OwinRestService>().InSingletonScope();
            Bind<ISyncService>().To<RedisSyncService>().InSingletonScope();
            Bind<IPackService>().To<FlatFilePackService>().InSingletonScope();

            Bind<IAnt>().To<Ant>();
            Bind<IAntFactory>().ToFactory();

            Bind<IAntDirectory>().To<AntDirectory>();
            Bind<IAntDirectoryFactory>().ToFactory();

            Bind<IPackageFactory>().ToFactory();
            Bind<IPackage>().To<ZipPackage>();

            Bind<IPackageEntryFactory>().ToFactory();
            Bind<IPackageEntry>().To<LazyPackageEntry>();
        }
    }
}
