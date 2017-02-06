using NUnit.Framework;
using System;

namespace Zapp.Pack
{
    [TestFixture]
    public class PackageVersionTests
    {
        [TestCase(null), TestCase("")]
        public void Constructor_WhenPackageIdIsNullOrEmpty(string packageId)
        {
            var exc = Assert.Throws<ArgumentException>(() => new PackageVersion(packageId, "v1.0.0"));

            Assert.That(exc.ParamName, Is.EqualTo(nameof(packageId)));
        }

        [TestCase(null), TestCase("")]
        public void Constructor_WhenDeployVersionIsNullOrEmpty(string deployVersion)
        {
            var exc = Assert.Throws<ArgumentException>(() => new PackageVersion("shared", deployVersion));

            Assert.That(exc.ParamName, Is.EqualTo(nameof(deployVersion)));
        }

        [Test]
        public void Equality_WhenPropertiesMatchIgnoreCase_ReturnsExpectedValues()
        {
            var a = new PackageVersion("a", "v1");
            var b = new PackageVersion("A", "v1");

            Assert.That(a.Equals(b), Is.EqualTo(true));
            Assert.That(a == b, Is.EqualTo(true));
            Assert.That(a != b, Is.EqualTo(false));
            Assert.That(a.Equals(b as object), Is.EqualTo(true));
        }
    }
}
