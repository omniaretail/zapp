using Newtonsoft.Json;
using NUnit.Framework;

namespace Zapp.Config
{
    [TestFixture]
    public class ZappConfigTests
    {
        [Test]
        public void Constructor_WhenCalled_SetupProperties()
        {
            var sut = new ZappConfig();

            Assert.That(sut.Rest, Is.Not.Null);
        }

        [Test]
        public void JsonConvert_FromAlias_ActsAsExpected()
        {
            var json = "{ rest: {} }";
            var sut = JsonConvert.DeserializeObject<ZappConfig>(json);

            Assert.That(sut.Rest, Is.Not.Null);
        }
    }
}
