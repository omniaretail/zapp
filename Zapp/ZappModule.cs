using AntPathMatching;
using FluentValidation;
using log4net;
using Ninject.Extensions.Factory;
using Ninject.Extensions.Perspectives;
using Ninject.Modules;
using System.IO;
using Zapp.Catalogue;
using Zapp.Config;
using Zapp.Core;
using Zapp.Deploy;
using Zapp.Fuse;
using Zapp.Hospital;
using Zapp.Pack;
using Zapp.Perspectives;
using Zapp.Rest;
using Zapp.Schedule;
using Zapp.Server;
using Zapp.Sync;
using Zapp.Transform;

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
            Kernel.Load(new[]
            {
                new ZappCoreModule()
            });

            Bind<ILog>().ToMethod(_ => LogManager.GetLogger(_.Request.Target.Member.DeclaringType));

            Bind<IConfigStore>().To<JsonConfigStore>().InSingletonScope();

            Bind<IZappServer>().To<ZappServer>().InSingletonScope();
            Bind<IRestService>().To<OwinRestService>().InSingletonScope();
            Bind<ISyncService>().To<RedisSyncService>().InSingletonScope();
            Bind<IPackService>().To<FilePackService>().InSingletonScope();
            Bind<IFusionService>().To<FusionService>().InSingletonScope();
            Bind<IScheduleService>().To<ScheduleService>().InSingletonScope();
            Bind<IDeployService>().To<DeployService>().InSingletonScope();

            Bind<IAnt>().To<Ant>();
            Bind<IAntFactory>().ToFactory();

            Bind<IAntDirectory>().To<AntDirectory>();
            Bind<IAntDirectoryFactory>().ToFactory();

            Bind<IPackageVersionValidator>().To<PackageVersionValidator>().InSingletonScope();

            Bind<IPackageFactory>().ToFactory();
            Bind<IPackage>().To<ZipPackage>();

            Bind<IPackageEntryFactory>().ToFactory();
            Bind<IPackageEntry>().To<LazyPackageEntry>();

            Bind<IFusion>().To<ZipFusion>();
            Bind<IFusionFactory>().ToFactory();

            Bind<IFusionBuilder>().To<FusionBuilder>().InSingletonScope();

            Bind<IFusionExtracter>().To<FusionExtractor>().InSingletonScope();

            Bind<IFusionProcess>().To<FusionProcess>();
            Bind<IFusionProcessFactory>().ToFactory();

            Bind<IFusionCatalogue>().To<FusionCatalogue>().InSingletonScope();

            Bind<IFusionMaid>().To<FusionMaid>().InSingletonScope();

            Bind<ITransformConfig>().To<XmlTransformConfig>().InSingletonScope();

            Bind<IFrameworkPackageEntry>().To<FusionMetaEntry>();
            Bind<IFrameworkPackageEntry>().To<FusionProcessEntry>();
            Bind<IFrameworkPackageEntry>().To<FusionProcessConfigEntry>();

            Bind<IFrameworkPackageEntryFactory>().ToFactory();

            Bind<IFile>().ToPerspective(typeof(File));

            Bind<IConnectionMultiplexerFactory>().To<ConnectionMultiplexerFactory>().InSingletonScope();

            Bind<IDeployAnnouncement>().To<DeployAnnouncement>().InTransientScope();
            Bind<IDeployAnnouncementFactory>().ToFactory();

            Bind<IHospitalService>().To<HospitalService>().InSingletonScope();

            LoadPerspectives();
            LoadValidators();
        }

        private void LoadPerspectives()
        {
            Bind<IDirectoryInfo>().ToPerspective(typeof(DirectoryInfo));
            Bind<IDirectoryInfoFactory>().ToFactory();
        }

        private void LoadValidators()
        {
            Bind<IValidator<FuseConfig>>().To<FuseConfigValidator>().InSingletonScope();
            Bind<IValidator<FusePackConfig>>().To<FusePackConfigValidator>().InSingletonScope();
            Bind<IValidator<PackConfig>>().To<PackConfigValidator>().InSingletonScope();
            Bind<IValidator<RestConfig>>().To<RestConfigValidator>().InSingletonScope();
            Bind<IValidator<SyncConfig>>().To<SyncConfigValidator>().InSingletonScope();
            Bind<IValidator<ZappConfig>>().To<ZappConfigValidator>().InSingletonScope();
        }
    }
}
