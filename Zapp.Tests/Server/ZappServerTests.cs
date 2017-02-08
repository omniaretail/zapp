using Moq;
using NUnit.Framework;
using Zapp.Rest;
using Zapp.Sync;

namespace Zapp.Server
{
    [TestFixture]
    public class ZappServerTests : TestBiolerplate<ZappServer>
    {
        [Test]
        public void Start_WhenCalled_CascadesToRestServices()
        {
            var sut = GetSystemUnderTest();

            sut.Start();

            kernel.GetMock<IRestService>().Verify(m => m.Listen(), Times.Exactly(1));
            kernel.GetMock<ISyncService>().Verify(m => m.Connect(), Times.Exactly(1));
        }
    }
}
