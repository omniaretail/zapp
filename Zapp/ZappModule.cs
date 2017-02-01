using Ninject.Extensions.Factory;
using Ninject.Modules;
using Zapp.Pack;
using Zapp.Rest;

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
            Bind<IZappServer>().To<ZappServer>();

            Bind<IRestService>().To<OwinRestService>();

            Bind<IPackage>().To<ZipPackage>();
            Bind<IPackageFactory>().ToFactory();
            Bind<IPackageService>().To<PackageService>();
        }
    }
}
