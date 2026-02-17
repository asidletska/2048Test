using UnityEngine;

public sealed class SpawnRoller : ISpawnRoller
{
    private readonly float _probability2;

    public SpawnRoller(float probability2)
    {
        _probability2 = Mathf.Clamp01(probability2);
    }

    public Po2Value Roll()
    {
        return Random.value <= _probability2 ? new Po2Value(2) : new Po2Value(4);
    }
}