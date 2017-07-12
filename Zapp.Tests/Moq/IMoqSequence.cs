namespace Zapp.Moq
{
    public interface IMoqSequence
    {
        void Callback(int seqId);

        int Index(string fingerprint);

        void Verify();
    }
}