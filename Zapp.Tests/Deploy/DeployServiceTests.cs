using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Zapp.Fuse;
using Zapp.Pack;
using Zapp.Schedule;
using Zapp.Sync;

namespace Zapp.Deploy
{
    [TestFixture]
    public class DeployServiceTests : TestBiolerplate<DeployService>
    {
        [Test]
        public void Announce_WhenPackageIsNotDeployed_Throws()
        {
            var version = new PackageVersion("lib1", "1.0.0");

            kernel.GetMock<ISyncService>()
                .Setup(m => m.Announce(version))
                .Returns(true);

            kernel.GetMock<IPackService>()
                .Setup(m => m.IsPackageVersionDeployed(version))
                .Returns(false);

            var sut = GetSystemUnderTest();

            Assert.That(() => sut.Announce(version), Throws.InstanceOf<AggregateException>());
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

            kernel.GetMock<IFusionService>()
                .Setup(m => m.GetAffectedFusions(version.PackageId))
                .Returns(new string[0]);

            var sut = GetSystemUnderTest();

            Assert.That(() => sut.Announce(version), Throws.Nothing);

            kernel.GetMock<IScheduleService>()
                .Verify(m => m.ScheduleMultiple(It.IsAny<IEnumerable<string>>()), Times.Exactly(1));
        }
    }
}
