using System.Collections;
using UnityEngine;

public interface ICoroutineRunner
{
    Coroutine Run(IEnumerator routine);
}