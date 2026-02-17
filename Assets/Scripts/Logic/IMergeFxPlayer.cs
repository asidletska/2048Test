using UnityEngine;

public interface IMergeFxPlayer
{
    void PlayMergeFx(Vector3 position);
    void PlayPop(Transform target, float popScale, float duration);
}