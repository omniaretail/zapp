using Ninject;
using NUnit.Framework;
using System.Net;
using System.Net.Http;
using Zapp.Config;
using Zapp.Server;

namespace Zapp.Rest
{
    [TestFixture, Category("integration"), Explicit]
    public class OwinRestServiceIntegrationTests
    {
        private IKernel kernel;

        private OwinRestService sut;

        [OneTimeSetUp]
        public void Setup()
        {
            var cfg = new ZappConfig();

            kernel = new StandardKernel(new ZappModule());

            var server = kernel.Get<IZappServer>();
            server.Start();

            sut = kernel.Get<OwinRestService>();
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            sut?.Dispose();
            sut = null;
        }

        [Test]
        public void Integration_WhenRestServiceCalled_RepliesWithStatusCodeOk()
        {
            var client = new HttpClient();

            var response = client.GetAsync($"http://localhost:6464/api/clerk/deploy/packageId/deployVersion").Result;

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
    }
}
