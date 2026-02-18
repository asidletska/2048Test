using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class EventBus : IEventBus
{
    private readonly Dictionary<Type, List<Delegate>> _handlers = new(32);

    public void Publish<T>(T evt) where T : struct
    {
        if (_handlers.TryGetValue(typeof(T), out var list) == false) return;

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] is Action<T> a)
                a.Invoke(evt);
        }
    }

    public IDisposable Subscribe<T>(Action<T> handler) where T : struct
    {
        if (handler == null) throw new ArgumentNullException(nameof(handler));

        var type = typeof(T);
        if (_handlers.TryGetValue(type, out var list) == false)
        {
            list = new List<Delegate>(8);
            _handlers[type] = list;
        }

        list.Add(handler);
        return new Subscription<T>(this, handler);
    }

    private void Unsubscribe<T>(Action<T> handler) where T : struct
    {
        var type = typeof(T);
        if (_handlers.TryGetValue(type, out var list) == false) return;

        list.Remove(handler);
        if (list.Count == 0) _handlers.Remove(type);
    }

    private sealed class Subscription<T> : IDisposable where T : struct
    {
        private readonly EventBus _bus;
        private Action<T> _handler;

        public Subscription(EventBus bus, Action<T> handler)
        {
            _bus = bus;
            _handler = handler;
        }

        public void Dispose()
        {
            if (_handler == null) return;
            _bus.Unsubscribe(_handler);
            _handler = null;
        }
    }
}

  public readonly struct ScoreChangedEvent
    {
        public readonly int Score;
        public ScoreChangedEvent(int score) => Score = score;
    }

    public readonly struct BestScoreChangedEvent
    {
        public readonly int BestScore;
        public BestScoreChangedEvent(int bestScore) => BestScore = bestScore;
    }

    public readonly struct CoinsChangedEvent
    {
        public readonly int Coins;
        public CoinsChangedEvent(int coins) => Coins = coins;
    }

    public readonly struct CubeMergedEvent
    {
        public readonly int FromValue;
        public readonly int ToValue;
        public readonly Vector3 Position;

        public CubeMergedEvent(int fromValue, int toValue, Vector3 position)
        {
            FromValue = fromValue;
            ToValue = toValue;
            Position = position;
        }
    }

    public readonly struct DailyClaimRequestedEvent { }

    public readonly struct DailyStateEvent
    {
        public readonly bool CanClaim;
        public readonly int Reward;
        public readonly int Streak;
        public readonly string NextClaimUtc;

        public DailyStateEvent(bool canClaim, int reward, int streak, string nextClaimUtc)
        {
            CanClaim = canClaim;
            Reward = reward;
            Streak = streak;
            NextClaimUtc = nextClaimUtc;
        }
    }

    public readonly struct AudioSettingsChangedEvent
    {
        public readonly bool MusicEnabled;
        public readonly bool SfxEnabled;

        public AudioSettingsChangedEvent( bool musicEnabled, bool sfxEnabled)
        {
            MusicEnabled = musicEnabled;
            SfxEnabled = sfxEnabled;
        }
    }
    public readonly struct PauseRequestedEvent { }
    public readonly struct RestartRequestedEvent { }

    public readonly struct GameOverEvent { }
    public readonly struct ResumeRequestedEvent { }

    public readonly struct OpenPanelEvent
    {
        public readonly UiPanelId Panel;
        public OpenPanelEvent(UiPanelId panel) => Panel = panel;
    }
    public readonly struct ClosePanelEvent
    {
        public readonly UiPanelId Panel;
        public ClosePanelEvent(UiPanelId panel) => Panel = panel;
    }
    public readonly struct TogglePanelEvent
    {
        public readonly UiPanelId Panel;
        public TogglePanelEvent(UiPanelId panel) => Panel = panel;
    }