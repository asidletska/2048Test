using System.Collections;
using UnityEngine;

public sealed class CoroutineRunner : MonoBehaviour, ICoroutineRunner
{
    public Coroutine Run(IEnumerator routine) => StartCoroutine(routine);
}
