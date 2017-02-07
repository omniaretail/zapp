using NUnit.Framework;
using System;

namespace Zapp.Pack
{
    [TestFixture]
    public class PackageVersionTests
    {
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
