using log4net;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.IO;

namespace Zapp.Config
{
    [TestFixture]
    public class JsonConfigStoreTests : TestBiolerplate<JsonConfigStore>
    {
        private string filePath;

        [TearDown]
        public override void Teardown()
        {
            base.Teardown();

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            filePath = ConstructFilePath();

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void Lazy_WhenFileDoesNotExists_PrefabsFileAndLogsWarning()
        {
            Assert.That(File.Exists(filePath), Is.EqualTo(false));

            var sut = GetSystemUnderTest();

            var config = sut.Value;

            Assert.That(config, Is.Not.Null);
            Assert.That(File.Exists(filePath), Is.EqualTo(true));

            kernel.GetMock<ILog>()
                .Verify(m => m.Warn(It.IsAny<string>()), Times.Exactly(1));
        }

        [Test]
        public void Lazy_WhenFileExists_LoadsOneTime()
        {
            var cfg = new ZappConfig();
            var content = JsonConvert.SerializeObject(cfg);

            File.WriteAllText(filePath, content);

            var sut = GetSystemUnderTest();

            var config = sut.Value;

            Assert.That(config, Is.Not.Null);
            Assert.That(config.Rest, Is.Not.Null);

            cfg.Rest = null;

            File.WriteAllText(filePath, content);

            config = sut.Value;

            Assert.That(config, Is.Not.Null);
            Assert.That(config.Rest, Is.Not.Null);
        }

        private string ConstructFilePath() => Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "zapp-config.deploy.json");
    }
}
