using UnityEngine;
public sealed class AppBootstrapper : MonoBehaviour, IHasEventBus, IHasSceneLoader
{
    public IEventBus Bus { get; private set; }
    public ISceneLoader SceneLoader { get; private set; }

    public IEconomyService Economy { get; private set; }
    public IBestScoreService BestScore { get; private set; }
    public IDailyRewardService Daily { get; private set; }
    public IAudioSettingsService Audio { get; private set; }

    private static AppBootstrapper _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        Initialize();
    }

    private void Initialize()
    {
        var save = new PrgressService();

        Bus = new EventBus();

        SceneLoader = new SceneLoader(this);

        Economy = new EconomyService(save, Bus);
        BestScore = new BestScoreService(save, Bus);
        Daily = new DailyRewardService(save, Bus, Economy);
        Audio = new AudioSettingsService(save, Bus);
    }
}