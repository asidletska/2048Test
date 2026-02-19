using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class AppBootstrapper : MonoBehaviour, IHasEventBus, IHasSceneLoader
{
    public static AppBootstrapper Instance { get; private set; }

    public IEventBus Bus { get; private set; }
    public ISceneLoader SceneLoader { get; private set; }

    public ISave Save { get; private set; }

    public IEconomyService Economy { get; private set; }
    public IBestScoreService BestScore { get; private set; }
    public IDailyRewardService Daily { get; private set; }
    public IAudioSettingsService Audio { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Initialize();

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Initialize()
    {
        Save = new PrgressService();

        Bus = new EventBus();
        SceneLoader = new SceneLoader(this);

        Economy = new EconomyService(Save, Bus);
        BestScore = new BestScoreService(Save, Bus);
        Daily = new DailyRewardService(Save, Bus, Economy);
        Audio = new AudioSettingsService(Save, Bus);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Bus.Publish(new CoinsChangedEvent(Economy.Coins));
        Bus.Publish(new BestScoreChangedEvent(BestScore.BestScore));
        Daily.RefreshState();
        Bus.Publish(new AudioSettingsChangedEvent(Audio.MusicEnabled, Audio.SfxEnabled));
    }
}
