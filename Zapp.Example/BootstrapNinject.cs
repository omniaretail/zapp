using Ninject;
using Zapp.Rest;

namespace Zapp.Example
{
    public static class BootstrapNinject
    {
        public static void Main(string[] args)
        {
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
        }
    }
}
