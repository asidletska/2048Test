using System.Collections.Generic;

public interface ICubeRegistry
{
    IReadOnlyCollection<CubeActor> Active { get; }
    void Add(CubeActor cube);
    void Remove(CubeActor cube);
}