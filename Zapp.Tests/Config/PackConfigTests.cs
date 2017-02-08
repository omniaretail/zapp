using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.IO;
using Zapp.Pack;

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
        }

        [Test]
        public void JsonConvert_FromAlias_ActsAsExpected()
        {
            var json = "{ rootDir: 'A', packagePattern: 'B' }";
            var sut = JsonConvert.DeserializeObject<PackConfig>(json);

            Assert.That(sut.RootDirectory, Is.EqualTo("A"));
            Assert.That(sut.PackagePattern, Is.EqualTo("B"));
        }


        [Test]
        public void GetActualRootDirectory_WhenCalled_ReturnsExpectedValue()
        {
            var sut = new PackConfig();

            var actual = ExcludeDividers(sut.GetActualRootDirectory());
            var expected = ExcludeDividers(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pack"));

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetActualPackagePattern_WhenCalled_ReturnsExpectedValue()
        {
            var sut = new PackConfig();

            var version = new PackageVersion("packId", "0.0.1");

            var actual = sut.GetActualPackagePattern(version);
            var expected = "containers-0.0.1-dev/packId*.{nupkg,zip}";

            Assert.That(actual, Is.EqualTo(expected));
        }

        private string ExcludeDividers(string input) => input
            .Replace("/", "")
            .Replace(@"\", "");
    }
}

