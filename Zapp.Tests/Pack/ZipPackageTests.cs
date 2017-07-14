using Ninject;
using Ninject.MockingKernel.Moq;
using NUnit.Framework;
using System.IO;
using System.Linq;
using Zapp.Assets;
using Zapp.Exceptions;

namespace Zapp.Pack
{
    [TestFixture]
    public class ZipPackageTests
    {
        private MoqMockingKernel kernel;

        private IPackageFactory factory;

        [SetUp]
        public void Setup()
        {
            kernel = new MoqMockingKernel();
            kernel.Load(new ZappModule());

            factory = kernel.Get<IPackageFactory>();
        }

        [Test]
        public void Constructor_WhenContentStreamNotValidZipFile_ThrowsException()
        {
            var version = new PackageVersion("package", "version");

            var contentStream = new MemoryStream();
            contentStream.WriteByte(0);

            Assert.That(() => factory.CreateNew(version, contentStream), Throws.InstanceOf<PackageException>());
        }

        [Test]
        public void GetEntries_WhenCalled_ReturnsExpectedValue()
        {
            var version = new PackageVersion("package", "version");

            using (var fs = AssetsHelper.Read("test.zip"))
            using (var pack = factory.CreateNew(version, fs) as ZipPackage)
            {
                var entries = pack.GetEntries();

                var content = ReadEntryContent(entries.Single());

                Assert.That(content, Is.EqualTo("test-value"));
            }
        }

        private string ReadEntryContent(IPackageEntry entry)
        {
            using (var stream = entry.Open())
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
