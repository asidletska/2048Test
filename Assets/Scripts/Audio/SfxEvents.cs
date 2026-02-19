using UnityEngine;

public class SfxEvents : MonoBehaviour
{
    [SerializeField] private MonoBehaviour busProvider; 
    [SerializeField] private SfxPlay sfxPlayer;

    private IEventBus _bus;
    private System.IDisposable _mergeSub;
    private System.IDisposable _loseSub;

    private void Awake()
    {
        if (sfxPlayer == null)
            sfxPlayer = FindObjectOfType<SfxPlay>();

        if (busProvider is IHasEventBus hasBus) _bus = hasBus.Bus;
        else
        {
            var app = FindObjectOfType<AppBootstrapper>();
            _bus = app != null ? app.Bus : null;
        }

        if (_bus == null) return;

        _mergeSub = _bus.Subscribe<CubeMergedEvent>(_ => sfxPlayer?.PlayMerge());
        _loseSub = _bus.Subscribe<GameOverEvent>(_ => sfxPlayer?.PlayLose());
    }

    private void OnDestroy()
    {
        _mergeSub?.Dispose();
        _loseSub?.Dispose();
    }
}
