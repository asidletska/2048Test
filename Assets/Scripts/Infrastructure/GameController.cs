using UnityEngine;

public class GameController : MonoBehaviour
{
    private readonly GameConfig _config;
    private readonly IBoardBounds _bounds;
    private readonly ISpawnRoller _roller;
    private readonly ICubeSpawner _spawner;
    private readonly ISettleDetector _settle;

    public CubeActor CurrentCube { get; private set; }

    private float _targetX;

    public GameController(GameConfig config, IBoardBounds bounds, ISpawnRoller roller, ICubeSpawner spawner, ISettleDetector settle)
    {
        _config = config;
        _bounds = bounds;
        _roller = roller;
        _spawner = spawner;
        _settle = settle;
    }

    public void Spawn()
    {
        var v = _roller.Roll();
        CurrentCube = _spawner.SpawnAtStart(v);
        _targetX = CurrentCube.transform.position.x;
    }

    public void BeginAim()
    {
        if (CurrentCube == null) return;
        var p = _bounds.SpawnPosition;
        p.x = _targetX;
        CurrentCube.transform.position = p;
    }

    public void Drag(float screenDx)
    {
        if (CurrentCube == null) return;

        _targetX = _bounds.ClampX(_targetX + screenDx * _config.dragSensitivity);
    }

    public void FixedAimUpdate(float fdt)
    {
        if (CurrentCube == null) return;
        if (!CurrentCube.Rigidbody.isKinematic) return;

        var p = CurrentCube.transform.position;
        p.x = Mathf.Lerp(p.x, _targetX, fdt * _config.aimMoveSpeed);
        CurrentCube.transform.position = p;
    }

    public void Launch()
    {
        if (CurrentCube == null) return;

        CurrentCube.LaunchMode();
        CurrentCube.Rigidbody.AddForce(_bounds.Forward * _config.launchImpulse, ForceMode.Impulse);
        _settle.Reset();
    }

    public bool IsSettled(float dt) => _settle.IsSettled(dt);
}

