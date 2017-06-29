using AntPathMatching;
using Ninject.Extensions.Factory;
using NUnit.Framework;
using Zapp.Exceptions;

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

            Assert.That(() => sut.LoadPackage(version), Throws.InstanceOf<PackageException>());
        }
    }
}
