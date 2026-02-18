using System;

public interface ISceneLoader
{
    bool IsLoading { get; }
    void Load(string sceneName);
    void Load(string sceneName, Action onLoaded);
    void ReloadActive();
    void Quit();
}