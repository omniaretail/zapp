using Ninject;
using Ninject.Extensions.Factory;
using NUnit.Framework;
using System.Collections.Generic;
using Zapp.Config;
using Zapp.Pack;

namespace Zapp.Deploy
{
    [TestFixture]
    public class DeployAnnouncementTests : TestBiolerplate<DeployAnnouncement>
    {
        public override void Setup()
        {
            base.Setup();

            config.Fuse.Fusions = new List<FusePackConfig>()
            {
                new FusePackConfig { Id = "fusion1" },
                new FusePackConfig { Id = "fusion2" }
            };

            kernel.Bind<IDeployAnnouncement>().To<DeployAnnouncement>();
            kernel.Bind<IDeployAnnouncementFactory>().ToFactory();
        }

        [Test]
        public void IsDelta_WhenAffectedFusionIdsDoesNotMatchConfigured_ReturnsTrue()
        {
            var sut = WithFusionIds(new[] { "fusion1" });

            Assert.That(sut.IsDelta(), Is.True);
        }

        [Test]
        public void IsDelta_WhenAffectedFusionIdsMatchesConfigured_ReturnsFalse()
        {
            var sut = WithFusionIds(new[] { "fusion1", "fusion2" });

            Assert.That(sut.IsDelta(), Is.False);
        }

        private DeployAnnouncement WithPackageVersions(params PackageVersion[] packageVersions)
        {
            return kernel.Get<IDeployAnnouncementFactory>()
                .CreateNew(new string[0], packageVersions) as DeployAnnouncement;
        }

        private DeployAnnouncement WithFusionIds(params string[] fusionIds)
        {
            return kernel.Get<IDeployAnnouncementFactory>()
                .CreateNew(fusionIds, new PackageVersion[0]) as DeployAnnouncement;
        }
    }
}
