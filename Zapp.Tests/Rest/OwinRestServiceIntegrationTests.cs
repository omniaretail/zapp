using Ninject;
using Ninject.MockingKernel.Moq;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using Zapp.Config;

namespace Zapp.Rest
{
    [TestFixture, Category("integration"), Explicit]
    public class OwinRestServiceIntegrationTests
    {
        private MoqMockingKernel kernel;

        private OwinRestService sut;

        [OneTimeSetUp]
        public void Setup()
        {
            var cfg = new ZappConfig();

            kernel = new MoqMockingKernel();

            kernel.GetMock<IConfigStore>()
                .Setup(m => m.Value)
                .Returns(() => cfg);

            sut = kernel.Get<OwinRestService>();
            sut.Listen();
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

            var response = client.GetAsync($"http://localhost:6464/api/clerk/deploy?packageId=test&deployVersion=0").Result;

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
    }
}
