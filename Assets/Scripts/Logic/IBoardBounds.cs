using UnityEngine;

public interface IBoardBounds
{
    float ClampX(float x);
    Vector3 SpawnPosition { get; }
    Vector3 Forward { get; }
}