using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public sealed class CubeActor : MonoBehaviour, IPoolable
{
    [SerializeField] private CubeView view;

    public Rigidbody Rigidbody { get; private set; }
    public Po2Value Value { get; private set; }
    public bool IsMerging { get; private set; }

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    public void SetValue(in Po2Value value)
    {
        Value = value;
        if (view != null) view.SetValue(Value.Value);
    }

    public void SetMerging(bool merging) => IsMerging = merging;

    public void EnterAimMode()
    {
        IsMerging = false;

        Rigidbody.isKinematic = true;
        Rigidbody.linearVelocity = Vector3.zero;
        Rigidbody.angularVelocity = Vector3.zero;

        Rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        Rigidbody.interpolation = RigidbodyInterpolation.None;

        Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    public void LaunchMode()
    {
        Rigidbody.isKinematic = false;
        Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

        Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    public void OnGetFromPool()
    {
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        gameObject.SetActive(true);
        EnterAimMode();
    }

    public void OnReturnToPool()
    {
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        IsMerging = false;
        Rigidbody.linearVelocity = Vector3.zero;
        Rigidbody.angularVelocity = Vector3.zero;
    }
}
