using Moq;
using Ninject;
using Ninject.MockingKernel.Moq;
using NUnit.Framework;
using Zapp.Rest;

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
        public void Start_WhenCalled_CascadesToRestService()
        {
            sut.Start();

            kernel.GetMock<IRestService>().Verify(m => m.Listen(), Times.Exactly(1));
        }
    }
}
