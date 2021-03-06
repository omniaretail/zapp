﻿using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using System;
using System.Threading;
using System.Threading.Tasks;
using Zapp.Fuse;
using Zapp.Moq;
using Zapp.Pack;
using Zapp.Schedule;
using Zapp.Sync;

namespace Zapp.Deploy
{
    [TestFixture]
    public class DeployServiceTests : TestBiolerplate<DeployService>
    {
        [Test]
        public void AnnounceAsync_WhenPackageUnavailable_Throws()
        {
            var versions = fixture.CreateMany<PackageVersion>(5);
            var validatorMock = fixture.Freeze<Mock<IPackageVersionValidator>>();

            validatorMock.Setup(_ => _.ConfirmAvailability(versions)).Throws<AggregateException>();

            var sut = fixture.Create<DeployService>();

            Assert.That(() => sut.AnnounceAsync(versions, CancellationToken.None), Throws.InstanceOf<AggregateException>());
        }

        [Test]
        public void AnnounceAsync_WhenPackageIsOk_CallsExpectedMethodsAndThrowsNothing()
        {
            var announcementMock = fixture.Freeze<Mock<IDeployAnnouncement>>();
            var announcementFactoryMock = fixture.Freeze<Mock<IDeployAnnouncementFactory>>();

            var versions = fixture.CreateMany<PackageVersion>(5);
            var syncMock = fixture.Freeze<Mock<ISyncService>>();
            var fusionMock = fixture.Freeze<Mock<IFusionService>>();
            var scheduleMock = fixture.Freeze<Mock<IScheduleService>>();
            var validatorMock = fixture.Freeze<Mock<IPackageVersionValidator>>();

            var sequence = new MoqSequence();

            var token = CancellationToken.None;

            validatorMock.Setup(_ => _.ConfirmAvailability(versions)).InSequence(sequence);
            announcementFactoryMock.Setup(_ => _.CreateNew(new string[0], versions)).InSequence(sequence).Returns(() => announcementMock.Object);
            scheduleMock.Setup(_ => _.ScheduleAsync(announcementMock.Object, token)).InSequence(sequence).Returns(Task.FromResult(true));

            var sut = fixture.Create<DeployService>();

            Assert.That(() => sut.AnnounceAsync(versions, token), Throws.Nothing);

            sequence.Verify();
        }
    }
}
