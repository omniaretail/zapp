using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace Zapp.Rest
{
    public class OwinRestService : IDisposable, IRestService
    {
        private readonly string baseAddress;

        private IDisposable owinInstance;

        public OwinRestService(string baseAddress)
        {
            this.baseAddress = baseAddress;
        }

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

        public void Dispose()
        {
            owinInstance?.Dispose();
            owinInstance = null;
        }
    }
}
