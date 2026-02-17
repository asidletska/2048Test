using System;

public sealed class DailyRewardService : IDailyRewardService
    {
        private const string LastClaimKey = "daily_last_claim_utc";
        private const string StreakKey = "daily_streak";

        private readonly ISave _storage;
        private readonly IEventBus _bus;
        private readonly IEconomyService _economy;

        public DailyRewardService(ISave storage, IEventBus bus, IEconomyService economy)
        {
            _storage = storage;
            _bus = bus;
            _economy = economy;

            _bus.Subscribe<DailyClaimRequestedEvent>(_ => TryClaim());
            RefreshState();
        }

        public void RefreshState()
        {
            var (canClaim, reward, streak, nextUtc) = ComputeState();
            _bus.Publish(new DailyStateEvent(canClaim, reward, streak, nextUtc));
        }

        private void TryClaim()
        {
            var (canClaim, reward, streak, nextUtc) = ComputeState();
            if (!canClaim)
            {
                _bus.Publish(new DailyStateEvent(false, reward, streak, nextUtc));
                return;
            }

            _economy.AddCoins(reward);

            var now = DateTime.UtcNow;
            _storage.SetString(LastClaimKey, now.ToString("O"));

            int newStreak = streak <= 0 ? 1 : streak;
            _storage.SetInt(StreakKey, newStreak);
            _storage.Flush();

            RefreshState();
        }

        private (bool canClaim, int reward, int streak, string nextUtc) ComputeState()
        {
            var now = DateTime.UtcNow;

            int streak = _storage.GetInt(StreakKey, 0);
            if (!TryReadUtc(_storage.GetString(LastClaimKey, ""), out var lastClaim))
            {
                int reward0 = RewardForStreak(1);
                return (true, reward0, 1, now.ToString("O"));
            }

            var next = lastClaim.AddHours(24);
            bool canClaim = now >= next;

            int newStreak;
            if (canClaim)
            {
                var hours = (now - lastClaim).TotalHours;
                newStreak = hours <= 48 ? (streak > 0 ? streak + 1 : 1) : 1;
            }
            else
            {
                newStreak = streak > 0 ? streak : 1;
            }

            int reward = RewardForStreak(newStreak);
            return (canClaim, reward, newStreak, next.ToString("O"));
        }

        private static bool TryReadUtc(string iso, out DateTime dt)
        {
            if (string.IsNullOrWhiteSpace(iso))
            {
                dt = default;
                return false;
            }

            return DateTime.TryParse(iso, null, System.Globalization.DateTimeStyles.RoundtripKind, out dt)
                   && dt.Kind == DateTimeKind.Utc;
        }

        private static int RewardForStreak(int streak)
        {
            if (streak <= 1) return 50;
            if (streak == 2) return 75;
            if (streak == 3) return 100;
            if (streak == 4) return 150;
            return 200;
        }
    }
