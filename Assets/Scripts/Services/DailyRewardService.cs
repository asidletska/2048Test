using System;
using System.Globalization;

public sealed class DailyRewardService : IDailyRewardService
{
    private const string LastClaimKey   = "daily.last_claim_utc";
    private const string DayIndexKey    = "daily.day_index";     
    private const string ClaimedMaskKey = "daily.claimed";  

    private readonly ISave _save;
    private readonly IEventBus _bus;
    private readonly IEconomyService _economy;

    private static readonly int[] Rewards = { 50, 75, 100, 150, 200, 250, 300 };

    public DailyRewardService(ISave storage, IEventBus bus, IEconomyService economy)
    {
        _save = storage;
        _bus = bus;
        _economy = economy;

        _bus.Subscribe<ClaimDailyDayRequestedEvent>(e => TryClaim(e.DayIndex));
        RefreshState();
    }

    public void RefreshState()
    {
        var s = ComputeState();
        _bus.Publish(new DailyStateEvent(s.canClaim, s.dayIndex, s.reward, s.claimedMask, s.nextClaimUtc));
    }

    public bool TryClaim(int pressedDayIndex)
    {
        var s = ComputeState();

        if (pressedDayIndex != s.dayIndex)
        {
            RefreshState();
            return false;
        }

        bool alreadyClaimed = (s.claimedMask & (1 << (s.dayIndex - 1))) != 0;
        if (alreadyClaimed)
        {
            RefreshState();
            return false;
        }

        if (!s.canClaim)
        {
            RefreshState();
            return false;
        }

        _economy.AddCoins(s.reward);

        int mask = s.claimedMask | (1 << (s.dayIndex - 1));

        var now = DateTime.UtcNow;
        _save.SetString(LastClaimKey, now.ToString("O"));

        int nextDay = s.dayIndex + 1;

        if (nextDay > Rewards.Length)
        {
            nextDay = 1;
            mask = 0; 
        }

        _save.SetInt(ClaimedMaskKey, mask);
        _save.SetInt(DayIndexKey, nextDay);
        _save.Flush();

        RefreshState();
        return true;
    }


    private (bool canClaim, int dayIndex, int reward, int claimedMask, string nextClaimUtc) ComputeState()
    {
        var now = DateTime.UtcNow;

        int dayIndex = _save.GetInt(DayIndexKey, 1);
        if (dayIndex < 1 || dayIndex > Rewards.Length) dayIndex = 1;

        int claimedMask = _save.GetInt(ClaimedMaskKey, 0);

        if (!TryReadUtc(_save.GetString(LastClaimKey, ""), out var lastClaim))
        {
            return (true, dayIndex, Rewards[dayIndex - 1], claimedMask, now.ToString("O"));
        }

        var next = lastClaim.AddSeconds(24);
        bool canClaim = now >= next;

        return (canClaim, dayIndex, Rewards[dayIndex - 1], claimedMask, next.ToString("O"));
    }

    private static bool TryReadUtc(string iso, out DateTime dt)
    {
        if (string.IsNullOrWhiteSpace(iso))
        {
            dt = default;
            return false;
        }

        return DateTime.TryParse(iso, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dt)
               && dt.Kind == DateTimeKind.Utc;
    }
}
