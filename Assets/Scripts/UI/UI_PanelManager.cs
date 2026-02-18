using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_PanelManager : MonoBehaviour
{
    [SerializeField] private PanelView[] panels;

    private IEventBus _bus;
    private readonly Dictionary<UiPanelId, PanelView> _map = new();

    private IDisposable _openSub;
    private IDisposable _closeSub;
    private IDisposable _toggleSub;

    public void Construct(IEventBus bus)
    {
        _bus = bus;

        _map.Clear();
        for (int i = 0; i < panels.Length; i++)
        {
            var p = panels[i];
            if (p == null) continue;
            _map[p.Id] = p;
        }

        _openSub = _bus.Subscribe<OpenPanelEvent>(e => Open(e.Panel));
        _closeSub = _bus.Subscribe<ClosePanelEvent>(e => Close(e.Panel));
        _toggleSub = _bus.Subscribe<TogglePanelEvent>(e => Toggle(e.Panel));
    }

    private void OnDestroy()
    {
        _openSub?.Dispose();
        _closeSub?.Dispose();
        _toggleSub?.Dispose();
    }

    public void Open(UiPanelId id)
    {
        if (id == UiPanelId.None) return;
        if (!_map.TryGetValue(id, out var panel)) return;

        panel.Show();
    }

    public void Close(UiPanelId id)
    {
        if (id == UiPanelId.None) return;
        if (!_map.TryGetValue(id, out var panel)) return;

        panel.Hide();
    }

    public void Toggle(UiPanelId id)
    {
        if (id == UiPanelId.None) return;
        if (!_map.TryGetValue(id, out var panel)) return;

        if (panel.IsVisible) panel.Hide();
        else panel.Show();
    }
}
