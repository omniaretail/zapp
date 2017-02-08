using NUnit.Framework;

namespace Zapp.Fuse
{
    [TestFixture]
    public class FusionProcessEntryTests
    {
        [Test]
        public void Name_WhenCalled_ReturnsExpectedValue()
        {
            var sut = new FusionProcessEntry();

            Assert.That(sut.Name, Is.EqualTo("Zapp.Process.exe"));
        }

        [Test]
        public void Name_WhenEdited_DoesNotThrow()
        {
            var sut = new FusionProcessEntry();

            Assert.DoesNotThrow(() => sut.Name = "test");

            Assert.That(sut.Name, Is.EqualTo("test"));
        }

        [Test]
        public void Open_WhenCalled_ReturnsExpectedValue()
        {
            var sut = new FusionProcessEntry();

            using (var stream = sut.Open())
            {
                Assert.That(stream.Length, Is.Not.EqualTo(0));
            }
        }
    }
}
