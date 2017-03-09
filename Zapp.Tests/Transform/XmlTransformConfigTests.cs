using NUnit.Framework;
using System.IO;
using Zapp.Assets;

namespace Zapp.Transform
{
    [TestFixture]
    public class XmlTransformConfigTests : TestBiolerplate<XmlTransformConfig>
    {
        [Test]
        public void Transform_WhenConfigIsNotNull_UsesValues()
        {
            config.Fuse.IsLoadFromRemoteSourcesEnabled = true;
            config.Fuse.IsGcServerEnabled = true;
            config.Fuse.IsGcConcurrentEnabled = true;
            config.Fuse.IsGcVeryLargeObjectsAllowed = true;

            var text = Transform();

            Assert.That(text, Does.Contain("<loadFromRemoteSources enabled=\"true\" />"));
            Assert.That(text, Does.Contain("<gcServer enabled=\"true\" />"));
            Assert.That(text, Does.Contain("<gcConcurrent enabled=\"true\" />"));
            Assert.That(text, Does.Contain("<gcAllowVeryLargeObjects enabled=\"true\" />"));
        }

        [Test]
        public void Transform_WhenConfigIsNull_UsesDefaultValues()
        {
            config.Fuse = null;

            var text = Transform();

            Assert.That(text, Does.Contain("<loadFromRemoteSources enabled=\"false\" />"));
            Assert.That(text, Does.Contain("<gcServer enabled=\"false\" />"));
            Assert.That(text, Does.Contain("<gcConcurrent enabled=\"true\" />"));
            Assert.That(text, Does.Contain("<gcAllowVeryLargeObjects enabled=\"false\" />"));
        }

        private string Transform()
        {
            var sut = GetSystemUnderTest();

            using (var testCase = AssetsHelper.Read("case-app.config"))
            using (var output = new MemoryStream())
            {
                sut.Transform(testCase, output);

                output.Seek(0, SeekOrigin.Begin);

                using (var reader = new StreamReader(output))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
