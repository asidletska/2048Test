using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public sealed class BackgroundMusic : MonoBehaviour
{
    [SerializeField] private MonoBehaviour bootstrapperProvider; 
    [SerializeField] private AudioClip musicClip;

    private AudioSource _source;
    private IEventBus _bus;
    private System.IDisposable _sub;

    private static BackgroundMusic _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        DontDestroyOnLoad(gameObject);

        _source = GetComponent<AudioSource>();
        _source.playOnAwake = false;
        _source.loop = true;

        if (musicClip != null)
            _source.clip = musicClip;

        if (bootstrapperProvider is IHasEventBus hasBus) _bus = hasBus.Bus;
        else
        {
            var app = FindObjectOfType<AppBootstrapper>();
            _bus = app != null ? app.Bus : null;
        }

        if (_bus != null)
        {
            _sub = _bus.Subscribe<AudioSettingsChangedEvent>(OnAudioSettings);
        }

        ApplyCurrentState();
    }

    private void OnDestroy()
    {
        _sub?.Dispose();
        if (_instance == this) _instance = null;
    }

    private void ApplyCurrentState()
    {
        var app = FindObjectOfType<AppBootstrapper>();
        if (app == null || app.Audio == null)
        {
            TryPlay();
            return;
        }

        SetEnabled(app.Audio.MusicEnabled);
    }

    private void OnAudioSettings(AudioSettingsChangedEvent e)
    {
        SetEnabled(e.MusicEnabled);
    }

    private void SetEnabled(bool enabled)
    {
        if (enabled) TryPlay();
        else _source.Pause();
    }

    private void TryPlay()
    {
        if (_source.clip == null) return;
        if (!_source.isPlaying) _source.Play();
    }
}
