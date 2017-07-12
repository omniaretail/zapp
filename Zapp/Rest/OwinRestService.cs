using AntPathMatching;
using log4net;
using Microsoft.Owin.Host.HttpListener;
using Microsoft.Owin.Hosting;
using Ninject;
using Ninject.Extensions.ChildKernel;
using Ninject.Web.Common.OwinHost;
using Ninject.Web.WebApi.OwinHost;
using Owin;
using Swashbuckle.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Principal;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Zapp.Config;
using Zapp.Core.Owin;

namespace Zapp.Rest
{
    /// <summary>
    /// Represents a implementation of <see cref="IRestService"/> for the Owin NuGet package.
    /// </summary>
    public sealed class OwinRestService : IRestService, IDisposable
    {
        private static readonly string[] standardIpAddresses = { "127.0.0.1", "localhost" };

        private readonly ILog logService;
        private readonly IConfigStore configStore;
        private readonly IAntFactory antFactory;
        private readonly IAssembliesResolver assembliesResolver;

        private IKernel hostKernel;
        private IDisposable owinInstance;

        /// <summary>
        /// Initializes a new <see cref="OwinRestService"/>.
        /// </summary>
        /// <param name="kernel">Ninject kernel instance.</param>
        /// <param name="logService">Service used for logging.</param>
        /// <param name="configStore">Configuration storage instance.</param>
        /// <param name="antFactory">Factory used for instantiating <see cref="IAnt"/>.</param>
        /// <param name="assembliesResolverFactory">Factory used for instantiating <see cref="IAssembliesResolver"/>.</param>
        public OwinRestService(
            IKernel kernel,
            ILog logService,
            IConfigStore configStore,
            IAntFactory antFactory,
            IAssembliesResolverFactory assembliesResolverFactory)
        {
            hostKernel = new ChildKernel(kernel);

            this.logService = logService;
            this.configStore = configStore;

            this.antFactory = antFactory;

            assembliesResolver = assembliesResolverFactory
                .CreateNew(typeof(OwinRestService).Assembly);
        }

        /// <summary>
        /// Starts the current instance of <see cref="OwinRestService"/>.
        /// </summary>
        public void Listen()
        {
            var opts = new StartOptions()
            {
                Port = configStore.Value.Rest.Port,
                ServerFactory = typeof(OwinHttpListener).Namespace
            };

            var ipAddresses = GetIpAddresses();

            foreach (string ipAddress in ipAddresses)
            {
                opts.Urls.Add($"http://{ipAddress}:{opts.Port}");
            }

            owinInstance = WebApp.Start(opts, Startup);

            logService.Info($"Server API listening on port: '{opts.Port}'.");
        }

        private void Startup(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            config.Services.Replace(typeof(IAssembliesResolver), assembliesResolver);

            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());

            var assemblyName = typeof(ZappModule).Assembly.GetName();

            var apiName = assemblyName.Name;
            var apiVersion = assemblyName.Version.ToString();

            config
                .EnableSwagger(c => c.SingleApiVersion(apiVersion, apiName))
                .EnableSwaggerUi();

            app.UseNinjectMiddleware(() => hostKernel).UseNinjectWebApi(config);

            config.MapHttpAttributeRoutes();

            config.EnsureInitialized();
        }

        private IEnumerable<string> GetIpAddresses()
        {
            var ant = antFactory.CreateNew(configStore.Value.Rest.IpAddressPattern);

            var ipAddresses = NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(i => i.OperationalStatus == OperationalStatus.Up)
                .SelectMany(i => i.GetIPProperties().UnicastAddresses)
                .Where(a => a.Address.AddressFamily == AddressFamily.InterNetwork)
                .Select(a => a.Address.ToString())
                .Concat(standardIpAddresses);

            if (!IsAdministratorRole())
            {
                ipAddresses = standardIpAddresses;
            }

            return ipAddresses
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Where(_ => ant.IsMatch(_));
        }

        private bool IsAdministratorRole()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);

            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// Releases all resourced used by the <see cref="OwinRestService"/> instance.
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
