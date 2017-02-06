using Newtonsoft.Json;
using NUnit.Framework;

namespace Zapp.Config
{
    [TestFixture]
    public class PackConfigTests
    {
        [Test]
        public void Constructor_WhenCalled_SetupProperties()
        {
            var sut = new PackConfig();

            Assert.That(sut.RootDirectory, Is.Not.Null);
            Assert.That(sut.PackagePattern, Is.Not.Null);
            Assert.That(sut.Fusions, Is.Not.Null);
        }

        [Test]
        public void JsonConvert_FromAlias_ActsAsExpected()
        {
            var json = "{ rootDir: 'A', packagePattern: 'B', fusions: [] }";
            var sut = JsonConvert.DeserializeObject<PackConfig>(json);

            Assert.That(sut.RootDirectory, Is.EqualTo("A"));
            Assert.That(sut.PackagePattern, Is.EqualTo("B"));
            Assert.That(sut.Fusions, Is.Not.Null);
        }
    }
}
