using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Zapp.Pack
{
    [TestFixture]
    public class ZipPackageTests
    {
        [Test]
        public void Constructor_WhenStreamIsNull_ThrowsException()
        {
            var stream = default(Stream);

            var exc = Assert.Throws<ArgumentNullException>(() => new ZipPackage(stream));

            Assert.That(exc.ParamName, Is.EqualTo(nameof(stream)));
        }

        [Test]
        public void Constructor_WhenStreamIsInvalid_ThrowsException()
        {
            var stream = new MemoryStream();

            var exc = Assert.Throws<PackageLoadFailureException>(() => new ZipPackage(stream));

            Assert.That(exc.Message, Is.Not.Empty);
            Assert.That(exc.InnerException, Is.InstanceOf<InvalidDataException>());
        }

        [TestCase(null), TestCase("")]
        public void ReadEntry_WhenNameIsNullOrEmpty_ThrowsException(string name)
        {
            var sut = new ZipPackage(CreateEmptyZipArchive());

            var exc = Assert.Throws<ArgumentException>(() => sut.ReadEntry(name));

            Assert.That(exc.ParamName, Is.EqualTo(nameof(name)));
        }

        [Test]
        public void ReadEntry_WhenNameIsNotFound_ThrowsException()
        {
            var name = "non-exist";

            var sut = new ZipPackage(CreateEmptyZipArchive());

            var exc = Assert.Throws<KeyNotFoundException>(() => sut.ReadEntry(name));

            Assert.That(exc.Message, Does.Contain(name));
        }

        [Test]
        public void Instance_WhenStreamIsValidZipArchive_ActsAsExpected()
        {
            var entryName = "name.ext";
            var content = Path.GetRandomFileName();

            using (var writeStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(writeStream, ZipArchiveMode.Create))
                {
                    var entry = archive.CreateEntry(entryName);

                    using (var entryStream = entry.Open())
                    using (var writer = new StreamWriter(entryStream))
                    {
                        writer.WriteLine(content);
                    }
                }

                using (var readStream = new MemoryStream(writeStream.ToArray()))
                using (var pack = new ZipPackage(readStream))
                using (var entry = pack.ReadEntry(entryName))
                using (var reader = new StreamReader(entry))
                {
                    var text = reader.ReadLine();

                    Assert.That(text, Is.EqualTo(content));
                }
            }
        }

        private Stream CreateEmptyZipArchive()
        {
            using (var writeStream = new MemoryStream())
            {
                (new ZipArchive(writeStream, ZipArchiveMode.Create)).Dispose();

                return new MemoryStream(writeStream.ToArray());
            }
        }
    }
}
