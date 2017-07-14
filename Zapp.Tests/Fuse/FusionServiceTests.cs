using Moq;
using Moq.Sequences;
using Newtonsoft.Json;
using NUnit.Framework;
using Ploeh.AutoFixture;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Zapp.Config;
using Zapp.Deploy;
using Zapp.Extensions;
using Zapp.Moq;
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
            var syncMock = fixture.Freeze<Mock<ISyncService>>();

            syncMock.Setup(_ => _.GetVersionAsync("test")).ReturnsAsync("0.0.1");

            var sut = fixture.Create<FusionService>();
            var actual = sut.GetPackageVersions("testVersions").Stale();

            var expected = new[]
            {
                new PackageVersion("test", "0.0.1"),
                new PackageVersion("test2"),
            };

            var actualJson = JsonConvert.SerializeObject(actual);
            var expectedJson = JsonConvert.SerializeObject(expected);

            Assert.That(actualJson, Is.EqualTo(expectedJson));
        }

        [Test]
        public void ExtractMultiple_WhenCalled_DoesWhatExpected()
        {
            var announcementMock = fixture.Freeze<Mock<IDeployAnnouncement>>();
            var fusionMock = fixture.Freeze<Mock<IFusion>>();
            var builderMock = fixture.Freeze<Mock<IFusionBuilder>>();
            var versionValidatorMock = fixture.Freeze<Mock<IPackageVersionValidator>>();
            var extractorMock = fixture.Freeze<Mock<IFusionExtracter>>();

            var seq = new MoqSequence();

            announcementMock.Setup(_ => _.GetFusionIds()).InSequence(seq).Returns(() => new[] { "test" });
            announcementMock.Setup(_ => _.GetPackageVersions(It.IsAny<string>())).InSequence(seq);
            versionValidatorMock.Setup(_ => _.ConfirmAvailability(It.IsAny<IEnumerable<PackageVersion>>())).InSequence(seq);
            builderMock.Setup(_ => _.Build(It.IsAny<FusePackConfig>(), It.IsAny<IFusion>(), It.IsAny<IEnumerable<PackageVersion>>())).InSequence(seq);
            extractorMock.Setup(_ => _.Extract(It.IsAny<FusePackConfig>(), It.IsAny<Stream>())).InSequence(seq);

            var sut = fixture.Create<FusionService>();

            Assert.That(() => sut.Extract(announcementMock.Object, CancellationToken.None), Throws.Nothing);

            seq.Verify();
        }
    }
}
