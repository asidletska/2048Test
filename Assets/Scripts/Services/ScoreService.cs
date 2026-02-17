
using System;

public sealed class ScoreService : IScoreService
{
    public int Score { get; private set; }
    public event Action<int> ScoreChanged;

    public void Reset()
    {
        Score = 0;
        ScoreChanged?.Invoke(Score);
    }

    public void Add(int amount)
    {
        if (amount <= 0) return;
        Score += amount;
        ScoreChanged?.Invoke(Score);
    }
}
