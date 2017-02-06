using AntPathMatching;
using Ninject;
using Ninject.Extensions.Factory;
using Ninject.MockingKernel.Moq;
using NUnit.Framework;
using System;
using Zapp.Config;

namespace Zapp.Pack
{
    [TestFixture]
    public class PackServiceTests
    {
        private MoqMockingKernel kernel;

        private PackService sut;
        private ZappConfig config;

        [SetUp]
        public void Setup()
        {
            config = new ZappConfig();

            kernel = new MoqMockingKernel();
            kernel.Bind<IAntFactory>().ToFactory();
            kernel.Bind<IAntDirectoryFactory>().ToFactory();

            kernel.GetMock<IConfigStore>()
                .Setup(m => m.Value)
                .Returns(() => config);

            sut = kernel.Get<PackService>();

            // todo: constructor breaks when configstore doesn't provider config.
        }

        [Test]
        public void LoadPackage_WhenVersionIsNul_ThrowsException()
        {
            var version = default(PackageVersion);

            var exc = Assert.Throws<ArgumentNullException>(() => sut.LoadPackage(version));

            Assert.That(exc.ParamName, Is.EqualTo(nameof(version)));
        }

        [Test]
        public void LoadPackage_WhenPackageDoesNotExists_ThrowsException()
        {
            var version = new PackageVersion("shared", "v1.0.0.0");

            var exc = Assert.Throws<PackageException>(() => sut.LoadPackage(version));

            Assert.That(exc.Version, Is.EqualTo(version));
        }
    }
}
