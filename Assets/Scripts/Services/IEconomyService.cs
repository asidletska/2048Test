public interface IEconomyService
{
    int Coins { get; }
    void AddCoins(int amount);
    bool TrySpend(int amount);
}