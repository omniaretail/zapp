using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Net.Http.Formatting;
using System.Web.Http;
using Swashbuckle.Application;
using Zapp.Config;
using log4net;

namespace Zapp.Rest
{
    /// <summary>
    /// Represents a implementation of <see cref="IRestService"/> for the Owin NuGet package.
    /// </summary>
    public class OwinRestService : IDisposable, IRestService
    {
        private readonly ILog logService;
        private readonly IConfigStore configStore;

        private IDisposable owinInstance;

        /// <summary>
        /// Initializes a new <see cref="OwinRestService"/>.
        /// </summary>
        /// <param name="logService">Service used for logging.</param>
        /// <param name="configStore">Configuration storage instance.</param>
        public OwinRestService(
            ILog logService,
            IConfigStore configStore)
        {
            this.logService = logService;
            this.configStore = configStore;
        }

        /// <summary>
        /// Starts the current instance of <see cref="OwinRestService"/>.
        /// </summary>
        public void Listen()
        {
            var opts = new StartOptions
            {
                Port = configStore.Value.Rest.Port
            };

            owinInstance = WebApp.Start(opts, Startup);

            logService.Info($"listening on port: {opts.Port}");
        }

        private void Startup(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            config.Routes.MapHttpRoute(
                name: "Zapp",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());

            app.UseWebApi(config);

            config
                .EnableSwagger(c => c.SingleApiVersion("v1", "Zapp"))
                .EnableSwaggerUi();
        }

        /// <summary>
        /// Releases all resourced used by the <see cref="OwinRestService"/> instance.
        /// </summary>
        public void Dispose()
        {
            owinInstance?.Dispose();
            owinInstance = null;
        }
    }
}
