using System;

public sealed class ScoreService : IScoreService
{
    public int Score { get; private set; }
    public event Action<int> ScoreChanged;

    public void Reset() => Set(0);

    public void Add(int amount)
    {
        if (amount <= 0) return;
        Set(Score + amount);
    }

    public void Set(int score)
    {
        Score = score < 0 ? 0 : score;
        ScoreChanged?.Invoke(Score);
    }
}
