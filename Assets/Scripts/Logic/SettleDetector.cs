using UnityEngine;

public sealed class SettleDetector : ISettleDetector
{
    private readonly ICubeRegistry _registry;
    private readonly float _speedThreshold;
    private readonly float _requiredTime;

    private float _timer;

    public SettleDetector(ICubeRegistry registry, float speedThreshold, float requiredTime)
    {
        _registry = registry;
        _speedThreshold = Mathf.Max(0.001f, speedThreshold);
        _requiredTime = Mathf.Max(0.01f, requiredTime);
    }

    public void ResetTimer() => _timer = 0f;

    public bool IsSettled(float dt)
    {
        foreach (var cube in _registry.Active)
        {
            if (cube == null) continue;
            if (cube.IsMerging) { _timer = 0f; return false; }

            var rb = cube.Rigidbody;
            if (rb == null) continue;

            if (!rb.isKinematic && rb.velocity.sqrMagnitude > _speedThreshold * _speedThreshold)
            {
                _timer = 0f;
                return false;
            }
        }

        _timer += dt;
        return _timer >= _requiredTime;
    }

    public void Reset()
    {
        _timer = 0f;
    }
}
