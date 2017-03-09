using log4net;
using Moq;
using NUnit.Framework;
using Zapp.Perspectives;

namespace Zapp.Config
{
    [TestFixture]
    public class JsonConfigStoreTests : TestBiolerplate<JsonConfigStore>
    {
        [Test]
        public void Value_WhenFileDoesNotExist_PerformsPrefab()
        {
            kernel.GetMock<IFile>()
                .Setup(m => m.Exists(It.IsAny<string>()))
                .Returns(false);

            var result = sut.Value;

            kernel.GetMock<IFile>()
                .Verify(m => m.WriteAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));

            kernel.GetMock<ILog>()
                .Verify(m => m.Warn(It.IsAny<string>()), Times.Exactly(1));
        }

        [Test]
        public void Value_WhenFileDoesExist_ReadsFromFile()
        {
            kernel.GetMock<IFile>()
                .Setup(m => m.Exists(It.IsAny<string>()))
                .Returns(true);

            kernel.GetMock<IFile>()
                .Setup(m => m.ReadAllText(It.IsAny<string>()))
                .Returns("{}");

            var result = sut.Value;

            kernel.GetMock<IFile>()
                .Verify(m => m.ReadAllText(It.IsAny<string>()), Times.Exactly(1));
        }
    }
}
