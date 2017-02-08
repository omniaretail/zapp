using AntPathMatching;
using Ninject.Extensions.Factory;
using NUnit.Framework;

namespace Zapp.Pack
{
    [TestFixture]
    public class PackServiceTests : TestBiolerplate<FilePackService>
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();

            kernel.Bind<IAntFactory>().ToFactory();
            kernel.Bind<IAntDirectoryFactory>().ToFactory();
        }

        [Test]
        public void LoadPackage_WhenPackageDoesNotExists_ThrowsException()
        {
            var sut = GetSystemUnderTest();

            var version = new PackageVersion("shared", "v1.0.0.0");

            var exc = Assert.Throws<PackageException>(() => sut.LoadPackage(version));

            Assert.That(exc.Version, Is.EqualTo(version));
        }
    }
}
