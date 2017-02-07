using Moq;
using Ninject;
using Ninject.MockingKernel.Moq;
using NUnit.Framework;
using Zapp.Rest;
using Zapp.Sync;

namespace Zapp.Server
{
    [TestFixture]
    public class ZappServerTests
    {
        private MoqMockingKernel kernel;

        private ZappServer sut;

        [SetUp]
        public void Setup()
        {
            kernel = new MoqMockingKernel();

            sut = kernel.Get<ZappServer>();
        }

        [Test]
        public void Start_WhenCalled_CascadesToRestServices()
        {
            sut.Start();

            kernel.GetMock<IRestService>().Verify(m => m.Listen(), Times.Exactly(1));
            kernel.GetMock<ISyncService>().Verify(m => m.Connect(), Times.Exactly(1));
        }
    }
}
