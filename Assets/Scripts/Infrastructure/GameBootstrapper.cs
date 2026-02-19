using UnityEngine;

public sealed class GameBootstrapper : MonoBehaviour, IHasEventBus
{
    [Header("Config")]
    [SerializeField] private GameConfig config;

    [Header("Scene refs")]
    [SerializeField] private BoardBounds boardBounds;
    [SerializeField] private DragInput inputSource;
    [SerializeField] private ScoreHud scoreHud;
    

    [Header("Optional scene refs")]
    [SerializeField] private UI_PanelManager uiPanels;
    [SerializeField] private LozeZone lozeZone;

    [Header("Prefabs")]
    [SerializeField] private CubeActor cubePrefab;
    [SerializeField] private ParticleSystem mergeFxPrefab;

    public ISceneLoader SceneLoader { get; private set; }
    public IEventBus Bus { get; private set; }

    private Transform _poolsRoot;

    private ObjectPool<CubeActor> _cubePool;
    private ObjectPool<ParticleSystem> _fxPool;

    private ICubeRegistry _registry;
    private ISpawnRoller _roller;
    private IScoreService _score;
    private ISettleDetector _settle;
    private ICubeSpawner _spawner;
    private IMergeService _merge;
    private IMergeFxPlayer _fx;
    private GameController _game;
    private PauseController _pause;

    private AppBootstrapper _app;

    private StateMachine _sm;
    private CoroutineRunner _runner;

    private System.IDisposable _restartSub;
    private System.IDisposable _gameOverSub;
    

    private void Awake()
    {
        Time.timeScale = 1f;
        _app = FindObjectOfType<AppBootstrapper>();
        if (_app == null)
        {
            return;
        }

        Bus = _app.Bus;
        SceneLoader = _app.SceneLoader;

        _pause = new PauseController(Bus);
        _runner = gameObject.AddComponent<CoroutineRunner>();

        if (uiPanels != null) uiPanels.Construct(Bus);
        if (lozeZone != null) lozeZone.Construct(Bus);

        Bus.Publish(new CoinsChangedEvent(_app.Economy.Coins));
        Bus.Publish(new BestScoreChangedEvent(_app.BestScore.BestScore));
        _app.Daily.RefreshState();

        _poolsRoot = new GameObject("[Pools]").transform;

        _cubePool = new ObjectPool<CubeActor>(cubePrefab, config.cubePrewarm, _poolsRoot);
        if (mergeFxPrefab != null)
            _fxPool = new ObjectPool<ParticleSystem>(mergeFxPrefab, config.fxPrewarm, _poolsRoot);

        _registry = new CubeRegistry();
        _roller = new SpawnRoller(config.probability2);
        _score = new ScoreService();
        _settle = new SettleDetector(_registry, config.settleSpeedThreshold, config.settleTime);
        _merge = new MergeService(config, _registry, _cubePool, _runner);
        _spawner = new CubeSpawner(_cubePool, _registry, boardBounds, _merge);

        _fx = new VisualEffects(_runner, mergeFxPrefab, _fxPool);
        _game = new GameController(config, boardBounds, _roller, _spawner, _settle);

        _score.ScoreChanged += s =>
        {
            scoreHud?.SetScore(s);
            Bus.Publish(new ScoreChangedEvent(s));
            _app.BestScore.TrySetBest(s);
        };

        _merge.Merged += r =>
        {
            _score.Add(ScoreCalculator.RewardForMerge(r.FromValue));
            _fx?.PlayMergeFx(r.Position);
            _fx?.PlayPop(r.Winner.transform, config.popScale, config.popDuration);
            FindObjectOfType<SfxPlay>()?.PlayMerge();
        };
        _restartSub = Bus.Subscribe<RestartRequestedEvent>(_ =>
        {
            Time.timeScale = 1f;
            SceneLoader?.ReloadActive();
        });
        _gameOverSub = Bus.Subscribe<GameOverEvent>(_ =>
        {
            int rewardCoins = config != null ? config.rewardCoinsPerGame : 10;

            if (rewardCoins > 0)
            {
                _app.Economy.AddCoins(rewardCoins);
                Bus.Publish(new RewardGrantedEvent(rewardCoins));
            }
            FindObjectOfType<SfxPlay>()?.PlayLose();
        });
        _sm = new StateMachine();
        _sm.Add(new BootstrapState(_sm, inputSource, _score));
        _sm.Add(new SpawnState(_sm, _game, inputSource));
        _sm.Add(new AimState(_sm, _game, inputSource));
        _sm.Add(new LaunchState(_sm, _game, inputSource));
        _sm.Add(new ResolveState(_sm, _game));
        inputSource.Enabled = true;
        _sm.Set<BootstrapState>();
    }
    private void OnEnable()
    {
        Time.timeScale = 1f;
        if (inputSource != null)
            inputSource.ResetInputState();
    }
    private void OnDestroy()
    {
        _restartSub?.Dispose();
        _gameOverSub?.Dispose();
        _pause?.Dispose();
        if (_poolsRoot != null)
            Destroy(_poolsRoot.gameObject);
    }

    private void Update() => _sm?.Tick(Time.deltaTime);
    private void FixedUpdate() => _sm?.FixedTick(Time.fixedDeltaTime);
}
