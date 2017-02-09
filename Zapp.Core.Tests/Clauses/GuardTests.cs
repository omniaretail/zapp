using NUnit.Framework;
using System;

namespace Zapp.Core.Clauses
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

        [Test]
        public void ParamNotOutOfRange_WhenParamValueIsGreaterThanUpperBounds_ThrowsException()
        {
            var paramName = "paramName";
            var paramValue = 50;

            var lowerBounds = 10;
            var upperBounds = 30;

            var exc = Assert.Throws<ArgumentOutOfRangeException>(() => Guard.ParamNotOutOfRange(paramValue, lowerBounds, upperBounds, paramName));

            Assert.That(exc.ParamName, Is.EqualTo(paramName));
            Assert.That(exc.Message, Does.Contain("less"));
            Assert.That(exc.Message, Does.Contain(upperBounds.ToString()));
        }

        [Test]
        public void ParamNotOutOfRange_WhenParamValueIsLessThanLowerBounds_ThrowsException()
        {
            var paramName = "paramName";
            var paramValue = 5;

            var lowerBounds = 10;
            var upperBounds = 30;

            var exc = Assert.Throws<ArgumentOutOfRangeException>(() => Guard.ParamNotOutOfRange(paramValue, lowerBounds, upperBounds, paramName));

            Assert.That(exc.ParamName, Is.EqualTo(paramName));
            Assert.That(exc.Message, Does.Contain("greater"));
            Assert.That(exc.Message, Does.Contain(lowerBounds.ToString()));
        }

        [Test]
        public void ParamNotOutOfRange_WhenParamValueIsOk_DoesNotThrow()
        {
            var paramName = "paramName";
            var paramValue = 15;

            var lowerBounds = 10;
            var upperBounds = 30;

            Assert.DoesNotThrow(() => Guard.ParamNotOutOfRange(paramValue, lowerBounds, upperBounds, paramName));
        }
    }
}
