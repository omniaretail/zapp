using NUnit.Framework;
using System.Collections.Generic;
using System.Text;

namespace Zapp.Moq
{
    public class MoqSequence : IMoqSequence
    {
        private int index;

        private List<int> expectedTimeline;
        private List<int> actualTimeline;

        private Dictionary<int, string> fingerprints;

        public MoqSequence()
        {
            expectedTimeline = new List<int>();
            actualTimeline = new List<int>();
            fingerprints = new Dictionary<int, string>();
        }

        public int Index(string fingerprint)
        {
            var id = ++index;

            expectedTimeline.Add(id);
            fingerprints.Add(id, fingerprint);

            return id;
        }

        public void Callback(int seqId) => actualTimeline.Add(seqId);

        public void Verify()
        {
            for (var i = 0; i < expectedTimeline.Count; i++)
            {
                var expected = expectedTimeline[i];
                var expectedFingerprint = fingerprints[expected];

                var actual = actualTimeline[i];
                var actualFingerprint = fingerprints[actual];

                Assert.That(actual, Is.EqualTo(expected), GetErrorMessage(i, expectedFingerprint, actualFingerprint));
            }
        }

        private string GetErrorMessage(int position, string expected, string actual)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Expected: {expected}");
            builder.AppendLine($"At Position: {position}");
            builder.AppendLine($"Actual: {actual}");
            return builder.ToString();
        }
    }
}
