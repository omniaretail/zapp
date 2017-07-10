using EnsureThat;
using Microsoft.Owin.Hosting;
using Ninject;
using Ninject.Extensions.ChildKernel;
using Ninject.Web.Common.OwinHost;
using Ninject.Web.WebApi.OwinHost;
using Owin;
using Swashbuckle.Application;
using System;
using System.Net;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Zapp.Core.Owin;

namespace Zapp.Process.Rest
{
    /// <summary>
    /// Represents an implementation of <see cref="IRestService"/> which uses the owin rest-api.
    /// </summary>
    public sealed class OwinRestService : IRestService, IDisposable
    {
        private readonly IAssembliesResolver assembliesResolver;

        private IKernel hostKernel;
        private IDisposable owinInstance;

        /// <summary>
        /// Initializes a new <see cref="OwinRestService"/>.
        /// </summary>
        /// <param name="kernel">Ninject kernel instance.</param>
        /// <param name="assembliesResolverFactory">Factory used for instantiating <see cref="IAssembliesResolver"/>.</param>
        public OwinRestService(
            IKernel kernel,
            IAssembliesResolverFactory assembliesResolverFactory)
        {
            this.hostKernel = new ChildKernel(kernel);

            assembliesResolver = assembliesResolverFactory
                .CreateNew(typeof(OwinRestService).Assembly);
        }

        /// <summary>
        /// Starts the rest-service on the provided port.
        /// </summary>
        /// <param name="port">Port that the service should bind onto.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="port"/> is out of bounds.</exception>
        /// <inheritdoc />
        public void Listen(int port)
        {
            EnsureArg.IsGt(port, IPEndPoint.MinPort, nameof(port));
            EnsureArg.IsLt(port, IPEndPoint.MaxPort, nameof(port));

            owinInstance = WebApp.Start(new StartOptions { Port = port }, Startup);
        }

        private void Startup(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            config.Services.Replace(typeof(IAssembliesResolver), assembliesResolver);

            config.MapHttpAttributeRoutes();

            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());

            var assemblyName = typeof(ZappProcessModule).Assembly.GetName();

            var apiName = assemblyName.Name;
            var apiVersion = assemblyName.Version.ToString();

            config
                .EnableSwagger(c => c.SingleApiVersion(apiVersion, apiName))
                .EnableSwaggerUi();

            app.UseNinjectMiddleware(() => hostKernel).UseNinjectWebApi(config);

            config.EnsureInitialized();
        }

        /// <summary>
        /// Releases all used resources by the <see cref="OwinRestService"/> instances.
        /// </summary>
        public void Dispose()
        {
            hostKernel?.Dispose();
            hostKernel = null;

            owinInstance?.Dispose();
            owinInstance = null;
        }
    }
}
