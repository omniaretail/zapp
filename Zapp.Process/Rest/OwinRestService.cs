using Microsoft.Owin.Hosting;
using Ninject;
using Ninject.Web.Common.OwinHost;
using Ninject.Web.WebApi.OwinHost;
using Owin;
using Swashbuckle.Application;
using System;
using System.Net;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Zapp.Core.Clauses;
using Zapp.Core.Owin;

namespace Zapp.Process.Rest
{
    /// <summary>
    /// Represents an implementation of <see cref="IRestService"/> which uses the owin rest-api.
    /// </summary>
    public class OwinRestService : IRestService, IDisposable
    {
        private readonly IKernel kernel;

        private IDisposable owinInstance;

        /// <summary>
        /// Initializes a new <see cref="OwinRestService"/>.
        /// </summary>
        /// <param name="kernel">Ninject kernel instance.</param>
        public OwinRestService(IKernel kernel)
        {
            this.kernel = kernel;
        }

        /// <summary>
        /// Starts the rest-service on the provided port.
        /// </summary>
        /// <param name="port">Port that the service should bind onto.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="port"/> is out of bounds.</exception>
        /// <inheritdoc />
        public void Listen(int port)
        {
            Guard.ParamNotOutOfRange(port, IPEndPoint.MinPort, IPEndPoint.MaxPort, nameof(port));

            owinInstance = WebApp.Start(new StartOptions { Port = port }, Startup);
        }

        private void Startup(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            config.Services.Replace(typeof(IAssembliesResolver), new StandardAssembliesResolver());

            config.MapHttpAttributeRoutes();

            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());

            //config
            //    .EnableSwagger(c => c.SingleApiVersion("v1", "ZappProcess"))
            //    .EnableSwaggerUi();

            app.UseNinjectMiddleware(() => kernel).UseNinjectWebApi(config);

            config.EnsureInitialized();
        }

        /// <summary>
        /// Releases all used resources by the <see cref="OwinRestService"/> instances.
        /// </summary>
        public void Dispose()
        {
            owinInstance?.Dispose();
            owinInstance = null;
        }
    }
}
