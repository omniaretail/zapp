using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using System;

namespace Zapp.Pack
{
    [TestFixture]
    public class PackageVersionValidatorTests
    {
        private IFixture fixture;

        [SetUp]
        public void Setup()
        {
            fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
        }

        [Test]
        public void ConfirmAvailability_WhenAllPackagesAreAvailable_ThrowsNothing()
        {
            var packServiceMock = fixture.Freeze<Mock<IPackService>>();

            packServiceMock
                .Setup(_ => _.IsPackageVersionDeployed(It.IsAny<PackageVersion>()))
                .Returns(true);

            var versions = fixture
                .CreateMany<PackageVersion>(5);

            var sut = fixture.Create<PackageVersionValidator>();

            Assert.That(() => sut.ConfirmAvailability(versions), Throws.Nothing);
        }

        [Test]
        public void ConfirmAvailability_WhenPackageVersionIsUnknown_Throws()
        {
            var packServiceMock = fixture.Freeze<Mock<IPackService>>();

            packServiceMock
                .Setup(_ => _.IsPackageVersionDeployed(It.IsAny<PackageVersion>()))
                .Returns(true);

            var versions = new[]
            {
                new PackageVersion("packageId")
            };

            var sut = fixture.Create<PackageVersionValidator>();

            Assert.That(() => sut.ConfirmAvailability(versions), Throws.InstanceOf<AggregateException>());
        }

        [Test]
        public void ConfirmAvailability_WhenPackageVersionIsNotDeployed_Throws()
        {
            var packServiceMock = fixture.Freeze<Mock<IPackService>>();

            packServiceMock
                .Setup(_ => _.IsPackageVersionDeployed(It.IsAny<PackageVersion>()))
                .Returns(false);

            var versions = fixture
                .CreateMany<PackageVersion>(5);

            var sut = fixture.Create<PackageVersionValidator>();

            Assert.That(() => sut.ConfirmAvailability(versions), Throws.InstanceOf<AggregateException>());
        }
    }
}
