using NUnit.Framework;

namespace Zapp.Fuse
{
    [TestFixture]
    public class FusionProcessConfigEntryTests : TestBiolerplate<FusionProcessConfigEntry>
    {
        [Test]
        public void Open_WhenCalled_DoesNotThrow()
        {
            var sut = GetSystemUnderTest();

            Assert.DoesNotThrow(() => sut.Open());
        }
    }
}
