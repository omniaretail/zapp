using NUnit.Framework;
using System;
using System.IO;

namespace Zapp.Pack
{
    [TestFixture]
    public class ZipPackageTests
    {
        [TestCase(null), TestCase("")]
        public void Load_WhenFileLocationIsNullOrEmpty_ThrowsException(string fileLocation)
        {
            var exc = Assert.Throws<ArgumentException>(() => new ZipPackage(fileLocation));

            Assert.That(exc.ParamName, Is.EqualTo(nameof(fileLocation)));
            Assert.That(exc.Message, Does.Contain("non-empty"));
        }

        [Test]
        public void Load_WhenFileLocationDoesExists_ReturnsInstance()
        {
            var fileLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "empty-package.zip");

            var result = new ZipPackage(fileLocation);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetEntry_WhenEntryExists_ReturnsStream()
        {
            var fileLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "non-empty-package.zip");

            var sut = new ZipPackage(fileLocation);

            var result = sut.GetEntry("Zapp.dll");

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void AddEntry_WhenCalled_IsRepresentedInEntriesCollection()
        {
            var fileLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "new-package.zip");

            try
            {
                using (var writer = new ZipPackage(fileLocation, PackageMode.Write))
                {
                    writer.AddEntry("test.dll", new MemoryStream());
                }

                using (var reader = new ZipPackage(fileLocation))
                {
                    Assert.That(reader.Entries, Does.Contain("test.dll"));
                }
            }
            finally
            {
                File.Delete(fileLocation);
            }
        }
    }
}
