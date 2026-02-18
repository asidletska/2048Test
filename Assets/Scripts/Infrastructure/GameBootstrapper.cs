using UnityEngine;

public sealed class GameBootstrapper : MonoBehaviour, IHasEventBus
{
    [Header("Config")]
    [SerializeField] private GameConfig config;

    [Header("Scene refs")]
    [SerializeField] private BoardBounds boardBounds;
    [SerializeField] private DragInput inputSource;
    [SerializeField] private ScoreHud scoreHud;

    [Header("Prefabs")]
    [SerializeField] private CubeActor cubePrefab;
    [SerializeField] private ParticleSystem mergeFxPrefab;
    
    public ISceneLoader SceneLoader { get; private set; }

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
    private StateMachine _sm;
    private CoroutineRunner _runner;

    private void Awake()
    {
        var app = FindObjectOfType<AppBootstrapper>();
        if (app == null)
        {
            Debug.LogError("AppBootstrapper not found on scene. Add Bootstrap scene or AppBootstrapper prefab.");
            return;
        }
        Bus = app.Bus;                    
        SceneLoader = app.SceneLoader;   
        _pause = new PauseController(Bus);
        _runner = gameObject.AddComponent<CoroutineRunner>();

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

        if (scoreHud != null)
        {
            _score.ScoreChanged += scoreHud.SetScore;
            scoreHud.SetScore(0);
        }

        _merge.Merged += r =>
        {
            _score.Add(ScoreCalculator.RewardForMerge(r.FromValue));
            _fx?.PlayMergeFx(r.Position);
            _fx?.PlayPop(r.Winner.transform, config.popScale, config.popDuration);
        };

        _sm = new StateMachine();
        _sm.Add(new BootstrapState(_sm, inputSource, _score));
        _sm.Add(new SpawnState(_sm, _game, inputSource));
        _sm.Add(new AimState(_sm, _game, inputSource));
        _sm.Add(new LaunchState(_sm, _game, inputSource));
        _sm.Add(new ResolveState(_sm, _game));

        _sm.Set<BootstrapState>();
    }

    private void OnDestroy()
    {
        if (_score != null && scoreHud != null)
            _score.ScoreChanged -= scoreHud.SetScore;
        
        _pause?.Dispose();
    }

    private void Update() => _sm?.Tick(Time.deltaTime);
    private void FixedUpdate() => _sm?.FixedTick(Time.fixedDeltaTime);
    public IEventBus Bus { get; private set; }
}