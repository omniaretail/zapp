using NUnit.Framework;
using Zapp.Pack;

namespace Zapp.Exceptions
{
    [TestFixture]
    public class PackageExceptionTests
    {
        [Test]
        public void Message_WhenCalled_ReturnsExpectedValue()
        {
            var message = "Failed to load.";

            var version = new PackageVersion("shared", "v1.0.0");

            var sut = new PackageException(message, version);

            var actual = sut.Message;

            Assert.That(actual, Does.Contain(message));
            Assert.That(actual, Does.Contain(version.PackageId));
            Assert.That(actual, Does.Contain(version.DeployVersion));
        }

        [Test]
        public void Message_WhenCalledWithEmptyArguments_ReturnsExpectedValue()
        {
            var message = "Failed to load.";

            var sut = new PackageException(message, null);

            var actual = sut.Message;

            Assert.That(actual, Does.Contain(message));
            Assert.That(actual, Does.Contain("?"));
        }
    }
}
