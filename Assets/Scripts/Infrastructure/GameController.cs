using UnityEngine;

public sealed class GameController
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
        if (CurrentCube == null) return;

        // sync aim target to current cube position
        _targetX = CurrentCube.Rigidbody.position.x;
    }

    public void BeginAim()
    {
        if (CurrentCube == null) return;

        // Ensure aim mode
        CurrentCube.EnterAimMode();
        _targetX = CurrentCube.Rigidbody.position.x;
    }

    public void Drag(float screenDx)
    {
        if (CurrentCube == null) return;

        _targetX = _bounds.ClampX(_targetX + screenDx * _config.dragSensitivity);
    }

    public void FixedAimUpdate(float fdt)
    {
        if (CurrentCube == null) return;

        var rb = CurrentCube.Rigidbody;
        if (rb == null || !rb.isKinematic) return;

        var p = rb.position;
        p.x = Mathf.Lerp(p.x, _targetX, fdt * _config.aimMoveSpeed);
        rb.MovePosition(p);
    }

    public void Launch()
    {
        if (CurrentCube == null) return;

        CurrentCube.LaunchMode();
        CurrentCube.Rigidbody.AddForce(_bounds.Forward * _config.launchImpulse, ForceMode.Impulse);

        // Important: reset settle timer so Resolve doesn't instantly spawn again
        _settle.Reset();

        // After launch current cube is no longer controlled by aim
        CurrentCube = null;
    }

    public bool IsSettled(float dt) => _settle.IsSettled(dt);
}
