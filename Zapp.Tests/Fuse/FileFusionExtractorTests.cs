using log4net;
using Moq;
using NUnit.Framework;
using System.IO;
using Zapp.Assets;
using Zapp.Config;

namespace Zapp.Fuse
{
    [TestFixture]
    public class FileFusionExtractorTests : TestBiolerplate<FileFusionExtractor>
    {
        [Test]
        public void Extract_WhenCalled_ExtractsFilesToDirectory()
        {
            var cfg = new FusePackConfig { Id = "test" };
            var contentStream = AssetsHelper.Read("test.zip");

            var sut = GetSystemUnderTest();

            var fuseDir = config.Fuse.GetActualFusionDirectory(cfg.Id);

            try
            {
                sut.Extract(cfg, contentStream);

                var dirInfo = new DirectoryInfo(fuseDir);

                Assert.That(dirInfo.GetFiles(), Is.Not.Empty);

                kernel.GetMock<ILog>()
                    .Verify(m => m.Info(It.IsAny<string>()), Times.Exactly(1));
            }
            finally
            {
                if (Directory.Exists(fuseDir))
                {
                    Directory.Delete(fuseDir, true);
                }
            }
        }
    }
}
