using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.IO;

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

        [Test]
        public void GetActualRootDirectory_WhenCalled_ReturnsExpectedValue()
        {
            var sut = new FuseConfig();

            var actual = ExcludeDividers(sut.GetActualRootDirectory());
            var expected = ExcludeDividers(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fuse"));

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetActualFusionDirectory_WhenCalled_ReturnsExpectedValue()
        {
            var sut = new FuseConfig();

            var fusionId = "test";

            var actual = ExcludeDividers(sut.GetActualFusionDirectory(fusionId));
            var expected = ExcludeDividers(Path.Combine(
                sut.GetActualRootDirectory(),
                fusionId));

            Assert.That(actual, Is.EqualTo(expected));
        }

        private string ExcludeDividers(string input) => input
            .Replace("/", "")
            .Replace(@"\", "");
    }
}
