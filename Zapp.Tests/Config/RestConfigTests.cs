using Newtonsoft.Json;
using NUnit.Framework;

namespace Zapp.Config
{
    [TestFixture]
    public class RestConfigTests
    {
        [Test]
        public void Constructor_WhenCalled_SetupProperties()
        {
            var sut = new RestConfig();

            Assert.That(sut.IpAddressPattern, Is.EqualTo("*"));
            Assert.That(sut.Port, Is.EqualTo(6464));
        }

        [Test]
        public void JsonConvert_FromAlias_ActsAsExpected()
        {
            var json = "{ ipAddressPattern: '*', port: 1234 }";
            var sut = JsonConvert.DeserializeObject<RestConfig>(json);

            Assert.That(sut.IpAddressPattern, Is.EqualTo("*"));
            Assert.That(sut.Port, Is.EqualTo(1234));
        }
    }
}
