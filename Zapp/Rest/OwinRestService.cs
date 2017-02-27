using AntPathMatching;
using log4net;
using Microsoft.Owin.Host.HttpListener;
using Microsoft.Owin.Hosting;
using Ninject;
using Ninject.Web.Common.OwinHost;
using Ninject.Web.WebApi.OwinHost;
using Owin;
using Swashbuckle.Application;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
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
    public class OwinRestService : IDisposable, IRestService
    {
        private readonly IKernel kernel;

        private readonly ILog logService;
        private readonly IConfigStore configStore;
        private readonly IAntFactory antFactory;

        private IDisposable owinInstance;

        /// <summary>
        /// Initializes a new <see cref="OwinRestService"/>.
        /// </summary>
        /// <param name="kernel">Ninject kernel instance.</param>
        /// <param name="logService">Service used for logging.</param>
        /// <param name="configStore">Configuration storage instance.</param>
        /// <param name="antFactory">Factory used for instantiating <see cref="IAnt"/>.</param>
        public OwinRestService(
            IKernel kernel,
            ILog logService,
            IConfigStore configStore,
            IAntFactory antFactory)
        {
            this.kernel = kernel;

            this.logService = logService;
            this.configStore = configStore;

            this.antFactory = antFactory;
        }

        /// <summary>
        /// Starts the current instance of <see cref="OwinRestService"/>.
        /// </summary>
        public void Listen()
        {
            var opts = new StartOptions();
            opts.ServerFactory = typeof(OwinHttpListener).Namespace;

            var port = configStore.Value.Rest.Port;
            var ipAddresses = GetIpAddresses();

            foreach (string ipAddress in ipAddresses)
            {
                opts.Urls.Add($"http://{ipAddress}:{port}");
            }

            owinInstance = WebApp.Start(opts, Startup);

            logService.Info($"listening on port: {opts.Port}");
        }

        private void Startup(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            config.Services.Replace(typeof(IAssembliesResolver), new StandardAssembliesResolver());

            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());

            config
                .EnableSwagger(c => c.SingleApiVersion("v1", "Zapp"))
                .EnableSwaggerUi();

            app.UseNinjectMiddleware(() => kernel)
                .UseNinjectWebApi(config);

            config.MapHttpAttributeRoutes();
        }

        private IReadOnlyCollection<string> GetIpAddresses()
        {
            var ant = antFactory.CreateNew(configStore.Value.Rest.IpAddressPattern);

            var ipAddresses = NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(i => i.OperationalStatus == OperationalStatus.Up)
                .SelectMany(i => i.GetIPProperties().UnicastAddresses)
                .Where(a => a.Address.AddressFamily == AddressFamily.InterNetwork)
                .Select(a => a.Address.ToString())
                .ToList();

            if (!IsAdministratorRole())
            {
                ipAddresses.Clear();
            }

            ipAddresses.Add("127.0.0.1");
            ipAddresses.Add("localhost");

            return ipAddresses
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Where(i => ant.IsMatch(i))
                .ToList();
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
            owinInstance?.Dispose();
            owinInstance = null;
        }
    }

    internal class MyAssembliesResolver : DefaultAssembliesResolver
    {
        public override ICollection<Assembly> GetAssemblies()
        {
            return new[] { typeof(ZappModule).Assembly };
        }
    }
}
