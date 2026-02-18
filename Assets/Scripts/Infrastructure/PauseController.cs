using System;
using UnityEngine;

public sealed class PauseController : IDisposable
{
    private readonly IEventBus _bus;
    private IDisposable _pauseSub;
    private IDisposable _resumeSub;

    public bool IsPaused { get; private set; }

    public PauseController(IEventBus bus)
    {
        _bus = bus;

        _pauseSub = _bus.Subscribe<PauseRequestedEvent>(_ => Pause());
        _resumeSub = _bus.Subscribe<ResumeRequestedEvent>(_ => Resume());
    }

    public void Pause()
    {
        if (IsPaused) return;
        IsPaused = true;
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        if (!IsPaused) return;
        IsPaused = false;
        Time.timeScale = 1f;
    }

    public void Dispose()
    {
        _pauseSub?.Dispose();
        _resumeSub?.Dispose();
    }
}
