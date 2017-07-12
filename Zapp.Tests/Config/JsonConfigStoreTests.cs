using FluentValidation;
using FluentValidation.Results;
using log4net;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Zapp.Perspectives;

namespace Zapp.Config
{
    [TestFixture]
    public class JsonConfigStoreTests : TestBiolerplate<JsonConfigStore>
    {
        [Test]
        public void Value_WhenFileDoesNotExists_WritesFileAndLogsWarning()
        {
            var fileMock = kernel.GetMock<IFile>();
            var logMock = kernel.GetMock<ILog>();

            ArrangeFileExists(false);

            Assert.That(() => sut.Value, Throws.Nothing);

            AssertWriteFile(Times.Once());
            AssertReadFile(Times.Never());
            AssertLogWarning(Times.Once());
        }

        [Test]
        public void Value_WhenFileDoesExists_ReadsFile()
        {
            ArrangeFileExists(true);
            ArrangeFileText("{}");

            IgnoreValidationResult();

            Assert.That(() => sut.Value, Throws.Nothing);

            AssertWriteFile(Times.Never());
            AssertReadFile(Times.Once());
            AssertValidation(Times.Once());
        }

        [Test]
        public void Value_WhenFileIsCorrupt_Throws()
        {
            ArrangeFileExists(true);
            ArrangeFileText("{{}");

            Assert.That(() => sut.Value, Throws.InstanceOf<JsonReaderException>());

            AssertWriteFile(Times.Never());
            AssertReadFile(Times.Once());
        }

        private void ArrangeFileText(string text)
        {
            kernel.GetMock<IFile>()
                .Setup(_ => _.ReadAllText(It.IsAny<string>()))
                .Returns(text);
        }

        private void ArrangeFileExists(bool exists)
        {
            kernel.GetMock<IFile>()
                .Setup(_ => _.Exists(It.IsAny<string>())).Returns(exists);
        }

        private void IgnoreValidationResult()
        {
            kernel.GetMock<IValidator<ZappConfig>>()
                .Setup(_ => _.Validate(It.IsAny<ValidationContext>()))
                .Returns(() => new ValidationResult());
        }

        private void AssertWriteFile(Times times)
        {
            kernel.GetMock<IFile>()
                .Verify(_ => _.WriteAllText(It.IsAny<string>(), It.IsAny<string>()), times);
        }

        private void AssertReadFile(Times times)
        {
            kernel.GetMock<IFile>()
                .Verify(_ => _.ReadAllText(It.IsAny<string>()), times);
        }

        private void AssertLogWarning(Times times)
        {
            kernel.GetMock<ILog>()
                .Verify(_ => _.Warn(It.IsAny<string>()), times);
        }

        private void AssertValidation(Times times)
        {
            kernel.GetMock<IValidator<ZappConfig>>()
                .Verify(_ => _.Validate(It.IsAny<ValidationContext>()), times);
        }
    }
}
