using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : ISceneLoader
{
    private readonly MonoBehaviour _runner;

    public bool IsLoading { get; private set; }

    public SceneLoader(MonoBehaviour runner)
    {
        _runner = runner;
    }

    public void Load(string sceneName) => Load(sceneName, null);

    public void Load(string sceneName, Action onLoaded)
    {
        if (IsLoading) return;
        _runner.StartCoroutine(LoadRoutine(sceneName, onLoaded));
    }

    public void ReloadActive()
    {
        var active = SceneManager.GetActiveScene();
        Load(active.name);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    private IEnumerator LoadRoutine(string sceneName, Action onLoaded)
    {
        IsLoading = true;

        yield return null; 

        var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        op.allowSceneActivation = true;

        while (!op.isDone)
        {
            yield return null;
        }


        IsLoading = false;
        onLoaded?.Invoke();
    }
}