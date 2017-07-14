using AntPathMatching;
using Moq;
using NUnit.Framework;
using System.IO;
using System.Linq;
using Zapp.Perspectives;

namespace Zapp.Catalogue
{
    public class FusionCatalogueTests : TestBiolerplate<FusionCatalogue>
    {
        private DirectoryInfo[] directories;

        public override void Setup()
        {
            base.Setup();

            directories = new[]
            {
                new DirectoryInfo("fuse_test_10"),
                new DirectoryInfo("fuse_test_11")
            };

            kernel.GetMock<IAnt>()
                .Setup(_ => _.IsMatch(It.IsAny<string>()))
                .Returns(true);

            kernel.GetMock<IAntFactory>()
                .Setup(_ => _.CreateNew(It.IsAny<string>()))
                .Returns(() => kernel.GetMock<IAnt>().Object);

            kernel.GetMock<IDirectoryInfoFactory>()
                .Setup(_ => _.CreateNew(It.IsAny<string>()))
                .Returns(() => kernel.GetMock<IDirectoryInfo>().Object);
        }

        [Test]
        public void CreateLocation_WhenCalled_ReturnsExpectedValue()
        {
            config.Fuse.RootDirectory = @"C:\somedir"; ;
            config.Fuse.FusionDirectoryPattern = "fuse_{fusionId}";

            var fusionId = "test";
            var sut = GetSystemUnderTest();

            var actual = sut.CreateLocation(fusionId);

            Assert.That(actual, Does.Contain(config.Fuse.RootDirectory));
            Assert.That(actual, Does.Contain($"fuse_test"));
        }

        [Test]
        public void GetActiveLocation_WhenCalled_ReturnsExpectedValue()
        {
            kernel.GetMock<IDirectoryInfo>()
                .Setup(_ => _.GetDirectories())
                .Returns(() => directories);

            var fusionId = "test";
            var sut = GetSystemUnderTest();

            var actual = sut.GetActiveLocation(fusionId);

            Assert.That(actual, Does.Contain("fuse_test_11"));
        }

        [Test]
        public void GetAllLocations_WhenCalled_ReturnsExpectedValue()
        {
            kernel.GetMock<IDirectoryInfo>()
                .Setup(_ => _.GetDirectories())
                .Returns(() => directories);

            var fusionId = "test";
            var sut = GetSystemUnderTest();

            var actual = sut.GetAllLocations(fusionId).ToArray();

            Assert.That(actual, Has.Length.EqualTo(2));
        }

        [Test]
        public void GetAllLocations_WhenNotMatched_DoesNotIncludeInResult()
        {
            kernel.GetMock<IAnt>()
                .Setup(_ => _.IsMatch("fuse_test_10"))
                .Returns(false);

            kernel.GetMock<IAnt>()
                .Setup(_ => _.IsMatch("fuse_test_11")) 
                .Returns(true);

            kernel.GetMock<IDirectoryInfo>()
                .Setup(_ => _.GetDirectories())
                .Returns(() => directories);

            var fusionId = "test";
            var sut = GetSystemUnderTest();

            var actual = sut.GetAllLocations(fusionId).ToArray();

            Assert.That(actual, Has.Length.EqualTo(1));
        }
    }
}
