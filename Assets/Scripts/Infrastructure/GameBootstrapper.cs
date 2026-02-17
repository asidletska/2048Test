using UnityEngine;

public class GameBootstrapper : MonoBehaviour
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

        private StateMachine _sm;
        private CoroutineRunner _runner;
        private IEventBus _bus;
        private void Awake()
        {
            _runner = gameObject.AddComponent<CoroutineRunner>();

            _poolsRoot = new GameObject("[Pools]").transform;

            _cubePool = new ObjectPool<CubeActor>(cubePrefab, config.cubePrewarm, _poolsRoot);

            if (mergeFxPrefab != null)
                _fxPool = new ObjectPool<ParticleSystem>(mergeFxPrefab, config.fxPrewarm, _poolsRoot);

            _registry = new CubeRegistry();
            _bus = new EventBus();
            _roller = new SpawnRoller(config.probability2);
            _score = new ScoreService();
            _settle = new SettleDetector(_registry, config.settleSpeedThreshold, config.settleTime);
            _spawner = new CubeSpawner(_cubePool, _registry, boardBounds);

            _merge = new MergeService(config, _registry, _cubePool, _runner);

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

            CubeEvents.CubeSpawned += OnCubeSpawned;
            CubeEvents.CubeDespawned += OnCubeDespawned;

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
            CubeEvents.CubeSpawned -= OnCubeSpawned;
            CubeEvents.CubeDespawned -= OnCubeDespawned;

            if (_score != null && scoreHud != null)
                _score.ScoreChanged -= scoreHud.SetScore;
        }

        private void Update() => _sm?.Tick(Time.deltaTime);
        private void FixedUpdate() => _sm?.FixedTick(Time.fixedDeltaTime);

        private void OnCubeSpawned(CubeActor cube)
        {
            if (cube == null) return;
            _registry.Add(cube);

            var relay = cube.GetComponent<CubeCollision>();
            if (relay != null) _merge.Register(relay);
        }

        private void OnCubeDespawned(CubeActor cube)
        {
            if (cube == null) return;
            _registry.Remove(cube);

            var relay = cube.GetComponent<CubeCollision>();
            if (relay != null) _merge.Unregister(relay);
        }
    }
