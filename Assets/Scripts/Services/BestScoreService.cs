
public sealed class BestScoreService : IBestScoreService
{
    private const string BestKey = "best_score";

    private readonly ISave _storage;
    private readonly IEventBus _bus;

    public int BestScore { get; private set; }

    public BestScoreService(ISave storage, IEventBus bus)
    {
        _storage = storage;
        _bus = bus;

        BestScore = _storage.GetInt(BestKey, 0);
        _bus.Publish(new BestScoreChangedEvent(BestScore));
    }

    public void TrySetBest(int score)
    {
        if (score <= BestScore) return;

        BestScore = score;
        _storage.SetInt(BestKey, BestScore);
        _storage.Flush();

        _bus.Publish(new BestScoreChangedEvent(BestScore));
    }
}
