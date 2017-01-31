using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace Zapp.Rest
{
    /// <summary>
    /// Represents a implementation of <see cref="IRestService"/> for the Owin NuGet package.
    /// </summary>
    public class OwinRestService : IDisposable, IRestService
    {
        private readonly string baseAddress;

        private IDisposable owinInstance;

        /// <summary>
        /// Initializes a new <see cref="OwinRestService"/>.
        /// </summary>
        /// <param name="baseAddress">The address to bind the web listener on.</param>
        public OwinRestService(string baseAddress)
        {
            this.baseAddress = baseAddress;
        }

        /// <summary>
        /// Starts the current instance of <see cref="OwinRestService"/>.
        /// </summary>
        public void Start()
        {
            owinInstance = WebApp.Start(baseAddress, appBuilder =>
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
