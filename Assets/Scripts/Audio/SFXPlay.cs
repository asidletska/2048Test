using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SfxPlay : MonoBehaviour
{
    [SerializeField] private MonoBehaviour bootstrapperProvider;
    [Range(0f, 1f)] [SerializeField] private float volume = 0.8f;

    [Header("Clips")]
    [SerializeField] private AudioClip mergeClip;
    [SerializeField] private AudioClip loseClip;

    private AudioSource _source;
    private IEventBus _bus;
    private System.IDisposable _sub;
    private bool _sfxEnabled = true;

    private static SfxPlay _instance;

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
        _source.loop = false;
        _source.volume = volume;

        if (bootstrapperProvider is IHasEventBus hasBus) _bus = hasBus.Bus;
        else
        {
            var app = FindObjectOfType<AppBootstrapper>();
            _bus = app != null ? app.Bus : null;
        }

        if (_bus != null)
        {
            _sub = _bus.Subscribe<AudioSettingsChangedEvent>(e => _sfxEnabled = e.SfxEnabled);
        }

        var app2 = FindObjectOfType<AppBootstrapper>();
        if (app2 != null && app2.Audio != null)
            _sfxEnabled = app2.Audio.SfxEnabled;
    }

    private void OnDestroy()
    {
        _sub?.Dispose();
        if (_instance == this) _instance = null;
    }

    public void PlayMerge() => Play(mergeClip);
    public void PlayLose() => Play(loseClip);

    public void Play(AudioClip clip)
    {
        if (!_sfxEnabled) return;
        if (clip == null) return;

        _source.PlayOneShot(clip, volume);
    }
}
