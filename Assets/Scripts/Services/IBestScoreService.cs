public interface IBestScoreService
{
    int BestScore { get; }
    void TrySetBest(int score);
}