using UnityEngine;

public sealed class SceneButtons : MonoBehaviour
{
    public void LoadMenu() => AppBootstrapper.Instance.SceneLoader.Load(Scenes.Menu);
    public void LoadGame() => AppBootstrapper.Instance.SceneLoader.Load(Scenes.Game);
    public void Restart()  => AppBootstrapper.Instance.SceneLoader.ReloadActive();
    public void Quit()     => AppBootstrapper.Instance.SceneLoader.Quit();
}
