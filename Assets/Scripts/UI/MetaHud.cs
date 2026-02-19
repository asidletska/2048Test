using System;
using TMPro;
using UnityEngine;

public sealed class MetaHud : MonoBehaviour
{
     [Header("Dependencies")]
    [SerializeField] private MonoBehaviour busProvider;

    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text bestScoreText;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private TMP_Text dailyTimerText;

    private IEventBus _bus;
    private IDisposable _scoreSub;
    private IDisposable _bestSub;
    private IDisposable _coinsSub;
    private IDisposable _dailySub;

    private DateTime _nextClaimUtc;
    private bool _canClaim;

    private float _timerAccumulator;

    private void Awake()
    {
        _bus = BusResolver.Resolve(busProvider, this);

        if (_bus == null) return;

        _scoreSub = _bus.Subscribe<ScoreChangedEvent>(e => SetScore(e.Score));
        _bestSub  = _bus.Subscribe<BestScoreChangedEvent>(e => SetBest(e.BestScore));
        _coinsSub = _bus.Subscribe<CoinsChangedEvent>(e => SetCoins(e.Coins));
        _dailySub = _bus.Subscribe<DailyStateEvent>(OnDailyState);
    }

    private void OnDestroy()
    {
        _scoreSub?.Dispose();
        _bestSub?.Dispose();
        _coinsSub?.Dispose();
        _dailySub?.Dispose();
    }

    private void Update()
    {
        if (dailyTimerText == null) return;

        _timerAccumulator += Time.unscaledDeltaTime;
        if (_timerAccumulator < 1f) return;
        _timerAccumulator = 0f;

        UpdateDailyTimer();
    }

    private void SetScore(int score)
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }

    private void SetBest(int best)
    {
        if (bestScoreText != null)
            bestScoreText.text = best.ToString();
    }

    private void SetCoins(int coins)
    {
        if (coinsText != null)
            coinsText.text = coins.ToString();
    }

    private void OnDailyState(DailyStateEvent e)
    {
        _canClaim = e.CanClaim;

        if (!string.IsNullOrEmpty(e.NextClaimUtc))
        {
            DateTime.TryParse(
                e.NextClaimUtc,
                null,
                System.Globalization.DateTimeStyles.RoundtripKind,
                out _nextClaimUtc);
        }

        UpdateDailyTimer();
    }

    private void UpdateDailyTimer()
    {
        if (dailyTimerText == null) return;

        if (_canClaim)
        {
            dailyTimerText.text = "Ready";
            return;
        }

        var remaining = _nextClaimUtc - DateTime.UtcNow;

        if (remaining.TotalSeconds <= 0)
        {
            dailyTimerText.text = "Ready";
            _canClaim = true;
            return;
        }

        dailyTimerText.text =
            $"{remaining.Hours:00}:{remaining.Minutes:00}:{remaining.Seconds:00}";
    }
}
