using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;

namespace Zapp.Rest
{
    [TestFixture, Category("integration"), Explicit]
    public class OwinRestServiceIntegrationTests
    {
        private OwinRestService sut;

        [OneTimeSetUp]
        public void Setup()
        {
            sut = new OwinRestService(new OwinRestServiceConfig
            {
                Port = 0
            });
            sut.Start();
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

            var response = client.GetAsync($"http://localhost:6464/api/simple/").Result;

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
    }
}
