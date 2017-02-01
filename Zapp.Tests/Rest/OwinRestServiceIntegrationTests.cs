using NUnit.Framework;
using System.Net;
using System.Net.Http;

namespace Zapp.Rest
{
    [TestFixture, Category("integration"), Explicit]
    public class OwinRestServiceIntegrationTests
    {
        private const string baseAddress = "http://localhost:6464/";

        private OwinRestService sut;

        [OneTimeSetUp]
        public void Setup()
        {
            sut = new OwinRestService(baseAddress);
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

            var response = client.GetAsync($"{baseAddress}api/simple/").Result;

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
    }
}
