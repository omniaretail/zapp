using Newtonsoft.Json;
using NUnit.Framework;

namespace Zapp.Config
{
    [TestFixture]
    public class SyncConfigTests
    {
        [Test]
        public void Constructor_WhenCalled_SetupProperties()
        {
            var sut = new SyncConfig();

            Assert.That(sut.ConnectionString, Is.Not.Null);
        }

        [Test]
        public void JsonConvert_FromAlias_ActsAsExpected()
        {
            var json = "{ connectionString: 'A' }";
            var sut = JsonConvert.DeserializeObject<SyncConfig>(json);

            Assert.That(sut.ConnectionString, Is.EqualTo("A"));
        }
    }
}
