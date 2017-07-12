using Ninject;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Zapp.Catalogue
{
    [TestFixture, Explicit]
    public class FusionCatalogueIntegrationTests
    {
        private const string subjectFusionId = "somethingAwesome";

        private List<DirectoryInfo> directories;

        private FusionCatalogue sut;

        [SetUp]
        public void SetUp()
        {
            var kernel = new StandardKernel(new ZappModule());

            sut = kernel.Get<FusionCatalogue>();

            directories = new[] {
                Directory.CreateDirectory(sut.CreateLocation(subjectFusionId)),
                Directory.CreateDirectory(sut.CreateLocation(subjectFusionId))
            }.ToList();
        }

        [TearDown]
        public void TearDown()
        {
            directories.FirstOrDefault()?.Parent?.Delete(true);
        }

        [Test]
        public void CreateLocation_WhenCalled_ReturnsExpectedValue()
        {
            var actual = sut.CreateLocation(subjectFusionId);

            Assert.That(() => Path.IsPathRooted(actual), Is.True);
            Assert.That(() => Path.GetDirectoryName(actual), Is.Not.Null.Or.Empty);
            Assert.That(() => Path.GetDirectoryName(actual), Is.Not.EqualTo(subjectFusionId));

            Assert.That(actual, Does.Contain(AppDomain.CurrentDomain.BaseDirectory));
            Assert.That(actual, Does.Contain(subjectFusionId));
        }

        [Test]
        public void GetActiveLocation_WhenCalled_ReturnsExpectedValue()
        {
            var actual = sut.GetActiveLocation(subjectFusionId);
            var actualInfo = new DirectoryInfo(actual);

            var expected = directories.Last();

            AssertDirectoryInfo(actualInfo, expected);
        }

        [Test]
        public void GetAllLocations_WhenCalled_ReturnsExpectedValues()
        {
            var actual = sut
                .GetAllLocations(subjectFusionId)
                .Select(_ => new DirectoryInfo(_))
                .ToArray();

            for (var i = 0; i < actual.Length; i++)
            {
                var left = actual[i];
                var right = directories[i];

                AssertDirectoryInfo(left, right);
            }
        }

        private void AssertDirectoryInfo(DirectoryInfo left, DirectoryInfo right)
        {
            Assert.That(left.Name, Is.EqualTo(right.Name));
            Assert.That(left.Parent.FullName, Is.EqualTo(right.Parent.FullName));
        }
    }
}
