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
            Assert.That(sut.Pack, Is.Not.Null);
            Assert.That(sut.Sync, Is.Not.Null);
        }

        [Test]
        public void JsonConvert_FromAlias_ActsAsExpected()
        {
            var json = "{ rest: {}, pack: {}, sync: {} }";
            var sut = JsonConvert.DeserializeObject<ZappConfig>(json);

            Assert.That(sut.Rest, Is.Not.Null);
            Assert.That(sut.Pack, Is.Not.Null);
            Assert.That(sut.Sync, Is.Not.Null);
        }
    }
}
