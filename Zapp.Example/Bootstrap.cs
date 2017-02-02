using log4net;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using Ninject;
using System;
using System.Threading;

using Zapp.Rest;
using Zapp.Server;

namespace Zapp.Example
{
    public static class Bootstrap
    {
        public static void Main(string[] args)
        {
            BootstrapLog4Net();

            var kernel = new StandardKernel(
                new ZappModule()
            );

            kernel.Bind<OwinRestServiceConfig>()
                .ToConstant(new OwinRestServiceConfig
                {
                    Port = 6464
                });


            var server = kernel.Get<IZappServer>();
            server.Start();

            // **Note** zapp-server does not keep the ui-thread running.
            SyncConsoleInput();
        }

        private static void BootstrapLog4Net()
        {
            var tracer = new TraceAppender();
            var hierarchy = (Hierarchy)LogManager.GetRepository();
            hierarchy.Root.AddAppender(tracer);
            var patternLayout = new PatternLayout { ConversionPattern = "%m%n" };
            tracer.Layout = patternLayout;
            hierarchy.Configured = true;
        }

        private static void SyncConsoleInput()
        {
            string userInput = null;

            while ((userInput = Console.ReadLine()) != null)
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }
    }
}
