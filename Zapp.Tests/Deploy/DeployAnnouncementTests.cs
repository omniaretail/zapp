using Ninject;
using Ninject.Extensions.Factory;
using NUnit.Framework;
using Zapp.Pack;

namespace Zapp.Deploy
{
    [TestFixture]
    public class DeployAnnouncementTests : TestBiolerplate<DeployAnnouncement>
    {
        public override void Setup()
        {
            base.Setup();

            kernel.Bind<IDeployAnnouncement>().To<DeployAnnouncement>();
            kernel.Bind<IDeployAnnouncementFactory>().ToFactory();
        } 

        [Test]
        public void IsDelta_WhenPackageVersionsIsEmpty_ReturnsFalse()
        {
            var sut = WithFusionIds(new[] { "fusionId" });

            Assert.That(sut.IsDelta(), Is.False);
        }

        [Test]
        public void IsDelta_WhenPackageVersionsIsNotEmpty_ReturnsTrue()
        {
            var sut = WithPackageVersions(new[] { new PackageVersion("a", "b") });

            Assert.That(sut.IsDelta(), Is.True);
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
