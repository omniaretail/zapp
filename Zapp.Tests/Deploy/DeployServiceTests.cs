using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using Zapp.Pack;
using Zapp.Schedule;
using Zapp.Sync;

namespace Zapp.Deploy
{
    [TestFixture]
    public class DeployServiceTests : TestBiolerplate<DeployService>
    {
        [Test]
        public void Announce_WhenSyncAnnounceFailed_ReturnsInternalError()
        {
            var version = new PackageVersion("lib1", "1.0.0");

            kernel.GetMock<ISyncService>()
                .Setup(m => m.Announce(version))
                .Returns(false);

            var sut = GetSystemUnderTest();

            var result = sut.Announce(version);

            Assert.That(result, Is.EqualTo(AnnounceResult.InternalError));
        }

        [Test]
        public void Announce_WhenPackageIsNotDeployed_ReturnsNotFound()
        {
            var version = new PackageVersion("lib1", "1.0.0");

            kernel.GetMock<ISyncService>()
                .Setup(m => m.Announce(version))
                .Returns(true);

            kernel.GetMock<IPackService>()
                .Setup(m => m.IsPackageVersionDeployed(version))
                .Returns(false);

            var sut = GetSystemUnderTest();

            var result = sut.Announce(version);

            Assert.That(result, Is.EqualTo(AnnounceResult.NotFound));
        }

        [Test]
        public void Announce_WhenEverythingIsOk_ReturnsOk()
        {
            var version = new PackageVersion("lib1", "1.0.0");

            kernel.GetMock<ISyncService>()
                .Setup(m => m.Announce(version))
                .Returns(true);

            kernel.GetMock<IPackService>()
                .Setup(m => m.IsPackageVersionDeployed(version))
                .Returns(true);

            var sut = GetSystemUnderTest();

            var result = sut.Announce(version);

            Assert.That(result, Is.EqualTo(AnnounceResult.Ok));

            kernel.GetMock<IScheduleService>()
                .Verify(m => m.Schedule(It.IsAny<IReadOnlyCollection<string>>(), true), Times.Exactly(1));
        }
    }
}
