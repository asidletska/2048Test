using System.Collections.Generic;

public sealed class CubeRegistry : ICubeRegistry
{
    private readonly HashSet<CubeActor> _active = new();

    public IReadOnlyCollection<CubeActor> Active => _active;

    public void Add(CubeActor cube)
    {
        if (cube != null) _active.Add(cube);
    }

    public void Remove(CubeActor cube)
    {
        if (cube != null) _active.Remove(cube);
    }
}
