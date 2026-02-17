
public sealed class EconomyService : IEconomyService
{
    private const string CoinsKey = "coins";

    private readonly ISave _storage;
    private readonly IEventBus _bus;

    public int Coins { get; private set; }

    public EconomyService(ISave storage, IEventBus bus)
    {
        _storage = storage;
        _bus = bus;

        Coins = _storage.GetInt(CoinsKey, 0);
        _bus.Publish(new CoinsChangedEvent(Coins));
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0) return;

        Coins += amount;
        _storage.SetInt(CoinsKey, Coins);
        _storage.Flush();

        _bus.Publish(new CoinsChangedEvent(Coins));
    }

    public bool TrySpend(int amount)
    {
        if (amount <= 0) return true;
        if (Coins < amount) return false;

        Coins -= amount;
        _storage.SetInt(CoinsKey, Coins);
        _storage.Flush();

        _bus.Publish(new CoinsChangedEvent(Coins));
        return true;
    }
}
