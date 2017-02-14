using Moq;
using Ninject;
using Ninject.MockingKernel.Moq;
using NUnit.Framework;
using Zapp.Process.Client;
using Zapp.Process.Libraries;
using Zapp.Process.Meta;
using Zapp.Process.Rest;

namespace Zapp.Process
{
    [TestFixture]
    public class ZappProcessTests
    {
        private MoqMockingKernel kernel;

        [SetUp]
        public void Setup()
        {
            kernel = new MoqMockingKernel();
        }

        [Test]
        public void Start_WhenCalled_ActsExpected()
        {
            var bindablePort = 1234;

            kernel.GetMock<IPortProvider>()
                .Setup(m => m.FindBindablePort())
                .Returns(() => bindablePort);

            var sut = kernel.Get<ZappProcess>();

            sut.Start();

            kernel.GetMock<IMetaService>()
                .Verify(m => m.Load(), Times.Exactly(1));

            kernel.GetMock<ILibraryService>()
                .Verify(m => m.LoadAll(), Times.Exactly(1));

            kernel.GetMock<IRestService>()
                .Verify(m => m.Listen(bindablePort), Times.Exactly(1));

            kernel.GetMock<IZappClient>()
                .Verify(m => m.Announce(bindablePort), Times.Exactly(1));
        }
    }
}