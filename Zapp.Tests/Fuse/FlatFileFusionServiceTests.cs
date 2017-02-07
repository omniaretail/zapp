using Ninject;
using Ninject.MockingKernel.Moq;
using NUnit.Framework;

namespace Zapp.Fuse
{
    [TestFixture]
    public class FlatFileFusionServiceTests
    {
        private MoqMockingKernel kernel;

        private FusionService sut;

        [SetUp]
        public void Setup()
        {
            kernel = new MoqMockingKernel();

            sut = kernel.Get<FusionService>();
        }
    }
}
