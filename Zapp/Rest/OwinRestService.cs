using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Net.Http.Formatting;
using System.Web.Http;
using Swashbuckle.Application;

namespace Zapp.Rest
{
    /// <summary>
    /// Represents a implementation of <see cref="IRestService"/> for the Owin NuGet package.
    /// </summary>
    public class OwinRestService : IDisposable, IRestService
    {
        private readonly OwinRestServiceConfig config;

        private IDisposable owinInstance;

        /// <summary>
        /// Initializes a new <see cref="OwinRestService"/>.
        /// </summary>
        /// <param name="config">The configuration for this instance.</param>
        public OwinRestService(OwinRestServiceConfig config)
        {
            this.config = config;
        }

        /// <summary>
        /// Starts the current instance of <see cref="OwinRestService"/>.
        /// </summary>
        public void Start()
        {
            var opts = new StartOptions { Port = config.Port };

            owinInstance = WebApp.Start(opts, appBuilder =>
            {
                var config = new HttpConfiguration();

                config.Routes.MapHttpRoute(
                    name: "Zapp",
                    routeTemplate: "api/{controller}/{id}",
                    defaults: new { id = RouteParameter.Optional }
                );

                config.Formatters.Clear();
                config.Formatters.Add(new JsonMediaTypeFormatter());

                appBuilder.UseWebApi(config);

                config
                    .EnableSwagger(c => c.SingleApiVersion("v1", "Zapp"))
                    .EnableSwaggerUi();
            });
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
