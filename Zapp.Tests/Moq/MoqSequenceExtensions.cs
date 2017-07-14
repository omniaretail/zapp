using Moq.Language.Flow;

namespace Zapp.Moq
{
    public static class MoqSequenceExtensions
    {
        public static ISetup<T> InSequence<T>(this ISetup<T> setup, MoqSequence sequence) where T : class
        {
            var seqId = sequence.Index(setup.ToString());

            setup.Callback(() => sequence.Callback(seqId));

            return setup;
        }

        public static ISetup<T, TResult> InSequence<T, TResult>(this ISetup<T, TResult> setup, MoqSequence sequence) where T : class
        {
            var seqId = sequence.Index(setup.ToString());

            setup.Callback(() => sequence.Callback(seqId));

            return setup;
        }
    }
}
