using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace Zapp.Fuse
{
    [TestFixture]
    public class FusionMetaEntryTests
    {
        [Test]
        public void Name_WhenSetterCalled_ThrowsException()
        {
            var sut = new FusionMetaEntry();

            Assert.Throws<NotSupportedException>(() => sut.Name = "bier");
        }

        [Test]
        public void Name_WhenGetterCalled_ReturnsExpectedValue()
        {
            var sut = new FusionMetaEntry();

            Assert.That(sut.Name, Is.EqualTo("fusion-meta.json"));
        }

        [Test]
        public void Open_WhenCalled_DoesContainsSerializedInfo()
        {
            var sut = new FusionMetaEntry();
            sut.SetInfo("key", "value");

            using (var stream = sut.Open())
            using (var reader = new StreamReader(stream))
            {
                var content = reader.ReadToEnd();
                var actual = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);

                Assert.That(actual["key"], Is.EqualTo("value"));
            }
        }
    }
}
