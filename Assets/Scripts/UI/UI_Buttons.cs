using UnityEngine;

public sealed class UI_Buttons : MonoBehaviour
{
    [SerializeField] private MonoBehaviour busProvider;

    private IEventBus _bus;

    private void Awake()
    {
        _bus = BusResolver.Resolve(busProvider, this);
    }

    private IEventBus Bus => _bus ??= BusResolver.Resolve(busProvider, this);

    public void Pause()   => Bus?.Publish(new PauseRequestedEvent());
    public void Resume()  => Bus?.Publish(new ResumeRequestedEvent());
  //  public void Restart() => Bus?.Publish(new RestartRequestedEvent());

 //  public void ClaimDaily() => Bus?.Publish(new DailyClaimRequestedEvent());

    public void OpenPause()
    {
        Bus?.Publish(new OpenPanelEvent(UiPanelId.Pause));
        Bus?.Publish(new PauseRequestedEvent());
    }

    public void Continue()
    {
        Bus?.Publish(new ClosePanelEvent(UiPanelId.Pause));
        Bus?.Publish(new ResumeRequestedEvent());
    }

    public void OpenSettings() => Bus?.Publish(new TogglePanelEvent(UiPanelId.Settings));
    public void OpenDaily()    => Bus?.Publish(new TogglePanelEvent(UiPanelId.Daily));
}
