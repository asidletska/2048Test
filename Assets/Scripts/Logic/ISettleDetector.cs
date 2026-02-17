public interface ISettleDetector
{
    void ResetTimer();
    bool IsSettled(float dt);

    void Reset();
}