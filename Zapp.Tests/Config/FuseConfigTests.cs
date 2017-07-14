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
            Assert.That(sut.IsLoadFromRemoteSourcesEnabled, Is.EqualTo(false));
            Assert.That(sut.IsGcServerEnabled, Is.EqualTo(false));
            Assert.That(sut.IsGcConcurrentEnabled, Is.EqualTo(true));
            Assert.That(sut.IsGcVeryLargeObjectsAllowed, Is.EqualTo(false));
        }
    }
}
