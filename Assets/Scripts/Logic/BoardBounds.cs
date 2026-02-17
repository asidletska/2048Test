using UnityEngine;

public sealed class BoardBounds : MonoBehaviour, IBoardBounds
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float minX = -1.2f;
    [SerializeField] private float maxX =  1.2f;
    [SerializeField] private Vector3 forward = Vector3.forward;

    public float ClampX(float x) => Mathf.Clamp(x, minX, maxX);
    public Vector3 SpawnPosition => spawnPoint != null ? spawnPoint.position : transform.position;
    public Vector3 Forward => forward.normalized;
}
