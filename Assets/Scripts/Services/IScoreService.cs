using System;

public interface IScoreService
{
    int Score { get; }
    event Action<int> ScoreChanged;

    void Reset();
    void Add(int amount);

    void Set(int score);
}
