using UnityEngine;

public sealed class CubeSpawner : ICubeSpawner
{
    private readonly IPool<CubeActor> _pool;
    private readonly ICubeRegistry _registry;
    private readonly IBoardBounds _bounds;
    private readonly IMergeService _merge;

    public CubeSpawner(IPool<CubeActor> pool, ICubeRegistry registry, IBoardBounds bounds, IMergeService merge)
    {
        _pool = pool;
        _registry = registry;
        _bounds = bounds;
        _merge = merge;
    }

    public CubeActor SpawnAtStart(in Po2Value value)
    {
        var cube = _pool.Get();

        var p = _bounds.SpawnPosition;
        var col = cube.GetComponent<Collider>();
        if (col != null) p.y += col.bounds.extents.y + 0.02f;

        cube.transform.position = p;
        cube.transform.rotation = Quaternion.identity;

        cube.SetValue(value);
        cube.EnterAimMode();

        _registry.Add(cube);

        var relay = cube.GetComponent<CubeCollision>();
        if (relay != null) _merge.Register(relay);

        return cube;
    }

    public void Despawn(CubeActor cube)
    {
        if (cube == null) return;

        var relay = cube.GetComponent<CubeCollision>();
        if (relay != null) _merge.Unregister(relay);

        _registry.Remove(cube);
        _pool.Release(cube);
    }
}
