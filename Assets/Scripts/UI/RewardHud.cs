using System;
using System.Collections;
using UnityEngine;

public sealed class RewardHud : MonoBehaviour
{
   [SerializeField] private DailyRewardsConfig config;
    [SerializeField] private DailyRewardDayButton[] dayButtons;

    private IEventBus _bus;
    private IDisposable _sub;
    private Coroutine _initRoutine;

    private void OnEnable()
    {
        _initRoutine = StartCoroutine(InitWhenReady());
    }

    private void OnDisable()
    {
        if (_initRoutine != null)
        {
            StopCoroutine(_initRoutine);
            _initRoutine = null;
        }

        _sub?.Dispose();
        _sub = null;
        _bus = null;
    }

    private IEnumerator InitWhenReady()
    {
        int safetyFrames = 30;
        while (AppBootstrapper.Instance == null && safetyFrames-- > 0)
            yield return null;

        var app = AppBootstrapper.Instance != null ? AppBootstrapper.Instance : FindObjectOfType<AppBootstrapper>();
        if (app == null || app.Bus == null)
        {
            Debug.LogError("RewardHud: AppBootstrapper/Bus not found. Ensure AppBootstrapper exists in first scene.");
            yield break;
        }

        _bus = app.Bus;
        _sub?.Dispose();
        _sub = _bus.Subscribe<DailyStateEvent>(ApplyState);
        BuildButtonsStatic();
        app.Daily.RefreshState();
    }

    private void BuildButtonsStatic()
    {
        if (config == null || config.days == null) return;
        if (dayButtons == null || dayButtons.Length == 0) return;
        if (_bus == null) return;

        int n = Mathf.Min(dayButtons.Length, config.days.Length);

        for (int i = 0; i < n; i++)
        {
            var b = dayButtons[i];
            if (b == null) continue;

            b.Construct(_bus);

            var d = config.days[i];
            b.Setup(i + 1, d.coins, d.icon);
        }
    }

    private void ApplyState(DailyStateEvent e)
    {
        if (config == null || config.days == null) return;
        if (dayButtons == null || dayButtons.Length == 0) return;

        int n = Mathf.Min(dayButtons.Length, config.days.Length);

        for (int i = 0; i < n; i++)
        {
            int dayIndex = i + 1;
            bool isToday = dayIndex == e.DayIndex;
            bool isClaimed = (e.ClaimedMask & (1 << (dayIndex - 1))) != 0;

            dayButtons[i]?.SetState(isToday, e.CanClaim, isClaimed);
        }
    }
}
