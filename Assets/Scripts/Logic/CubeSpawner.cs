
public sealed class CubeSpawner : ICubeSpawner
{
    private readonly IPool<CubeActor> _pool;
    private readonly ICubeRegistry _registry;
    private readonly IBoardBounds _bounds;

    public CubeSpawner(IPool<CubeActor> pool, ICubeRegistry registry, IBoardBounds bounds)
    {
        _pool = pool;
        _registry = registry;
        _bounds = bounds;
    }

    public CubeActor SpawnAtStart(in Po2Value value)
    {
        var cube = _pool.Get();
        cube.transform.position = _bounds.SpawnPosition;
        cube.transform.rotation = UnityEngine.Quaternion.identity;
        cube.SetValue(value);
        cube.EnterAimMode();
        _registry.Add(cube);
        return cube;
    }
}