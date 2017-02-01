using Ninject;
using Zapp;

namespace Zap.Example
{
    public static class BootstrapNinject
    {
        public static void Main(string[] args)
        {
            var kernel = new StandardKernel(
                new ZappModule()
            );

            var server = kernel.Get<IZappServer>();
            server.Start();

            // **Note** zapp-server does not keep the ui-thread running.
        }
    }
}
