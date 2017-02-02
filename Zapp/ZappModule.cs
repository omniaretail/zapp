﻿using log4net;
using Ninject.Modules;
using Zapp.Rest;
using Zapp.Server;

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

            Bind<ILog>().ToMethod(ctx => LogManager.GetLogger(ctx.Request.Target.Member.ReflectedType));
        }
    }
}
