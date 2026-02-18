public interface ICubeSpawner
{
    CubeActor SpawnAtStart(in Po2Value value);
    void Despawn(CubeActor cube);
}
