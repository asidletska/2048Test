
public sealed class EconomyService : IEconomyService
{
    private const string CoinsKey = "coins";

    private readonly ISave _save;
    private readonly IEventBus _bus;

    public int Coins { get; private set; }

    public EconomyService(ISave save, IEventBus bus)
    {
        _save = save;
        _bus = bus;

        Coins = _save.GetInt(CoinsKey, 0);
        _bus.Publish(new CoinsChangedEvent(Coins));
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0) return;

        Coins += amount;
        _save.SetInt(CoinsKey, Coins);
        _save.Flush();

        _bus.Publish(new CoinsChangedEvent(Coins));
    }

  /*  public bool TrySpend(int amount)
    {
        if (amount <= 0) return true;
        if (Coins < amount) return false;

        Coins -= amount;
        _save.SetInt(CoinsKey, Coins);
        _save.Flush();

        _bus.Publish(new CoinsChangedEvent(Coins));
        return true;
    }*/
}
