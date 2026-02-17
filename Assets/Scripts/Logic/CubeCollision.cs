using System;
using UnityEngine;

[RequireComponent(typeof(CubeActor))]
public sealed class CubeCollision: MonoBehaviour
{
    public event Action<CubeActor, Collision> CollisionEntered;

    private CubeActor _cube;

    private void Awake() => _cube = GetComponent<CubeActor>();

    private void OnCollisionEnter(Collision collision)
    {
        CollisionEntered?.Invoke(_cube, collision);
    }
}
