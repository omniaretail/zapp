using NUnit.Framework;

namespace Zapp.Config
{
    [TestFixture]
    public class FusePackConfigTests
    {
        [Test]
        public void Constructor_WhenCalled_SetupProperties()
        {
            var sut = new FusePackConfig();

            Assert.That(sut.Id, Is.EqualTo(null));
            Assert.That(sut.Order, Is.EqualTo(0));
            Assert.That(sut.PackageIds, Is.Not.Null);
        }
    }
}
