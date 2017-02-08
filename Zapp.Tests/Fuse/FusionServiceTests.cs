using AntPathMatching;
using log4net;
using Moq;
using Newtonsoft.Json;
using Ninject.Extensions.Factory;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Zapp.Config;
using Zapp.Pack;
using Zapp.Sync;

namespace Zapp.Fuse
{
    [TestFixture]
    public class FusionServiceTests : TestBiolerplate<FusionService>
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();

            kernel.Bind<IAntFactory>().ToFactory();
            kernel.Bind<IAntDirectoryFactory>().ToFactory();

            kernel.Bind<IFusionFactory>().ToFactory();

            config.Fuse.Fusions.Add(new FusePackConfig
            {
                Id = "test",
                PackageIds = new List<string>() { "library1" }
            });

            config.Fuse.Fusions.Add(new FusePackConfig
            {
                Id = "testVersions",
                PackageIds = new List<string>() { "test", "test2" }
            });
        }

        [Test]
        public void GetPackageVersions_WhenCalled_ReturnsExpectedValue()
        {
            kernel.GetMock<ISyncService>()
                .Setup(m => m.Sync("test"))
                .Returns("0.0.1");

            var sut = GetSystemUnderTest();
            var actual = sut.GetPackageVersions("testVersions");

            var expected = new List<PackageVersion>()
            {
                new PackageVersion("test", "0.0.1"),
                new PackageVersion("test2"),
            };

            var actualJson = JsonConvert.SerializeObject(actual);
            var expectedJson = JsonConvert.SerializeObject(expected);

            Assert.That(actualJson, Is.EqualTo(expectedJson));
        }

        [Test]
        public void TryExtractFusion_WhenCalledWithKnownLibraries_ReturnsTrue()
        {
            var version = new PackageVersion("library1", "1.0.0");

            kernel.GetMock<ISyncService>()
                .Setup(m => m.Sync(version.PackageId))
                .Returns(version.DeployVersion);

            var packageMock = kernel.GetMock<IPackage>();
            var packageEntryMock = kernel.GetMock<IPackageEntry>();

            kernel.GetMock<IAnt>()
                .Setup(m => m.IsMatch(It.IsAny<string>()))
                .Returns(true);

            packageMock
                .Setup(m => m.GetEntries())
                .Returns(() => new[] { packageEntryMock.Object });

            kernel.GetMock<IPackService>()
                .Setup(m => m.LoadPackage(version))
                .Returns(() => packageMock.Object);

            kernel.GetMock<IPackService>()
                .Setup(m => m.IsPackageVersionDeployed(version))
                .Returns(true);

            var sut = GetSystemUnderTest();

            var result = sut.TryExtractFusion("test");

            Assert.That(result, Is.EqualTo(true));

            kernel.GetMock<ILog>()
                .Verify(m => m.Debug(It.IsAny<string>()), Times.Exactly(3));

            kernel.GetMock<IFusionExtracter>()
                .Verify(m => m.Extract(It.IsAny<FusePackConfig>(), It.IsAny<Stream>()), Times.Exactly(1));

            kernel.GetMock<IFusion>()
                .Verify(m => m.AddEntry(packageEntryMock.Object), Times.Exactly(1));

            kernel.GetMock<IFusion>()
                .Verify(m => m.AddEntry(It.IsAny<IPackageEntry>()), Times.Exactly(3));
        }

        [Test]
        public void TryExtractFusion_WhenCalledWithUnknownLibraries_ReturnsFalse()
        {
            var version = new PackageVersion("library1", "1.0.0");

            kernel.GetMock<ISyncService>()
                .Setup(m => m.Sync(version.PackageId))
                .Returns(() => null);

            var sut = GetSystemUnderTest();

            var result = sut.TryExtractFusion("test");

            Assert.That(result, Is.EqualTo(false));

            kernel.GetMock<ILog>()
                .Verify(m => m.Warn(It.IsAny<string>()), Times.Exactly(1));
        }

        [Test]
        public void TryExtractFusion_WhenCalledWithNonDeployedLibraries_ReturnsFalse()
        {
            var version = new PackageVersion("library1", "1.0.0");

            kernel.GetMock<ISyncService>()
                .Setup(m => m.Sync(version.PackageId))
                .Returns(version.DeployVersion);

            var packageMock = kernel.GetMock<IPackage>();
            var packageEntryMock = kernel.GetMock<IPackageEntry>();

            kernel.GetMock<IPackService>()
                .Setup(m => m.IsPackageVersionDeployed(version))
                .Returns(false);

            var sut = GetSystemUnderTest();

            var result = sut.TryExtractFusion("test");

            Assert.That(result, Is.EqualTo(false));

            kernel.GetMock<ILog>()
                .Verify(m => m.Warn(It.IsAny<string>()), Times.Exactly(1));
        }
    }
}
