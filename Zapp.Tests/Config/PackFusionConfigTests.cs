using Newtonsoft.Json;
using NUnit.Framework;
using System.Linq;

namespace Zapp.Config
{
    [TestFixture]
    public class PackFusionConfigTests
    {
        [Test]
        public void Constructor_WhenCalled_SetupProperties()
        {
            var sut = new FusePackConfig();

            Assert.That(sut.PackageIds, Is.Not.Null);
        }

        [Test]
        public void JsonConvert_FromAlias_ActsAsExpected()
        {
            var json = "{ id: 'A', packageIds: [ 'B' ] }";
            var sut = JsonConvert.DeserializeObject<FusePackConfig>(json);

            Assert.That(sut.Id, Is.EqualTo("A"));
            Assert.That(sut.PackageIds.Single(), Is.EqualTo("B"));
        }
    }
}
