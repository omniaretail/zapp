using Zapp.Pack;
using Zapp.Rest;

namespace Zapp.Example
{
    public static class Bootstrap
    {
        public static void Main(string[] args)
        {
            var server = new ZappServer(
                new OwinRestService("http://localhost:6464"),
                new PackageService(
                    new ZipPackageFactory()
                )
            );

            server.Start();

            // **Note** zapp-server does not keep the ui-thread running.
        }
    }
}
