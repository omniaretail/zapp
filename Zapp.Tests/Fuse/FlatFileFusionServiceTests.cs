using Ninject;
using Ninject.MockingKernel.Moq;
using NUnit.Framework;
using System;

namespace Zapp.Fuse
{
    [TestFixture]
    public class FlatFileFusionServiceTests : IDisposable
    {
        private MoqMockingKernel kernel;

        private FusionService sut;

        [SetUp]
        public void Setup()
        {
            kernel = new MoqMockingKernel();

            sut = kernel.Get<FusionService>();
        }

        public void Dispose()
        {
            kernel?.Dispose();
            kernel = null;
        }
    }
}
