using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using Zapp.Rest;
using Zapp.Sync;

namespace Zapp.Server
{
    [TestFixture]
    public class ZappServerTests : TestBiolerplate<ZappServer>
    {
        [Test]
        public async Task StartAsync_WhenCalled_CascadesToRestServices()
        {
            var sut = GetSystemUnderTest();

            await sut.StartAsync(CancellationToken.None);

            kernel.GetMock<IRestService>().Verify(m => m.Listen(), Times.Exactly(1));
            kernel.GetMock<ISyncService>().Verify(m => m.Connect(), Times.Exactly(1));
        }
    }
}
