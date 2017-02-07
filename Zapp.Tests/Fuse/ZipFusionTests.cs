using NUnit.Framework;
using System.IO;
using System.IO.Compression;
using System.Text;
using Zapp.Pack;

namespace Zapp.Fuse
{
    public class ZipFusionTests
    {
        [Test]
        public void AddEntry_WhenCalled_ActsAsExpected()
        {
            var entryName = "test.txt";
            var entryContent = "test-value";

            using (var writeable = new MemoryStream())
            {
                using (var sut = new ZipFusion(writeable))
                {
                    sut.AddEntry(
                        new LazyPackageEntry(entryName,
                            new LazyStream(() => CreateEntryContent(entryContent))));
                }

                var data = writeable.ToArray();

                using (var readable = new MemoryStream(data))
                using (var archive = new ZipArchive(readable))
                {
                    var actualEntry = archive.GetEntry(entryName);
                    var actualEntryContent = ReadEntryContent(actualEntry.Open());

                    Assert.That(actualEntryContent, Is.EqualTo(entryContent));
                }
            }
        }

        private Stream CreateEntryContent(string content)
        {
            var binary = Encoding.UTF8.GetBytes(content);
            return new MemoryStream(binary);
        }

        private string ReadEntryContent(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
