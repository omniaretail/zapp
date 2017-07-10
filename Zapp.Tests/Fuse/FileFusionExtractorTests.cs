using log4net;
using Moq;
using NUnit.Framework;
using System;
using System.IO;
using Zapp.Assets;
using Zapp.Catalogue;
using Zapp.Config;

namespace Zapp.Fuse
{
    [TestFixture]
    public class FileFusionExtractorTests : TestBiolerplate<FusionExtractor>
    {
        [Test]
        public void Extract_WhenCalled_ExtractsFilesToDirectory()
        {
            var cfg = new FusePackConfig { Id = "test" };
            var fuseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test");

            var contentStream = AssetsHelper.Read("test.zip");

            kernel.GetMock<IFusionCatalogue>()
                .Setup(_ => _.CreateLocation(cfg.Id))
                .Returns(() => fuseDir);

            var sut = GetSystemUnderTest();

            try
            {
                sut.Extract(cfg, contentStream);

                var dirInfo = new DirectoryInfo(fuseDir);

                Assert.That(dirInfo.GetFiles(), Is.Not.Empty);

                kernel.GetMock<ILog>()
                    .Verify(_ => _.Info(It.IsAny<string>()), Times.Once);

                kernel.GetMock<IFusionMaid>()
                    .Verify(_ => _.CleanAll(cfg.Id), Times.Once);
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
