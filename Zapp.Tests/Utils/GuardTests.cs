using NUnit.Framework;
using System;

namespace Zapp.Utils
{
    [TestFixture]
    public class GuardTests
    {
        [Test]
        public void ParamNotNull_WhenParamValueIsNull_ThrowsException()
        {
            var paramValue = default(object);
            var paramName = "paramName";

            var exc = Assert.Throws<ArgumentNullException>(() => Guard.ParamNotNull(paramValue, paramName));

            Assert.That(exc.ParamName, Is.EqualTo(paramName));
        }

        [TestCase(null), TestCase("")]
        public void ParamNotNullOrEmpty_WhenParamValueIsNullOrEmpty_ThrowsException(string paramValue)
        {
            var paramName = "paramName";

            var exc = Assert.Throws<ArgumentException>(() => Guard.ParamNotNullOrEmpty(paramValue, paramName));

            Assert.That(exc.ParamName, Is.EqualTo(paramName));
        }
    }
}
