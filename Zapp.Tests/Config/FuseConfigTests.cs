using Newtonsoft.Json;
using NUnit.Framework;

namespace Zapp.Config
{
    [TestFixture]
    public class FuseConfigTests
    {
        [Test]
        public void Constructor_WhenCalled_SetupProperties()
        {
            var sut = new FuseConfig();

            Assert.That(sut.RootDirectory, Is.Not.Null);
            Assert.That(sut.EntryPattern, Is.Not.Null);
            Assert.That(sut.FusionDirectoryPattern, Is.Not.Null);
            Assert.That(sut.Fusions, Is.Not.Null);
        }

        [Test]
        public void JsonConvert_FromAlias_ActsAsExpected()
        {
            var json = "{ rootDir: 'A', entryPattern: 'B', fusionDirPattern: 'C', fusions: [] }";
            var sut = JsonConvert.DeserializeObject<FuseConfig>(json);

            Assert.That(sut.RootDirectory, Is.EqualTo("A"));
            Assert.That(sut.EntryPattern, Is.EqualTo("B"));
            Assert.That(sut.FusionDirectoryPattern, Is.EqualTo("C"));
            Assert.That(sut.Fusions, Is.Not.Null);
        }
    }
}
