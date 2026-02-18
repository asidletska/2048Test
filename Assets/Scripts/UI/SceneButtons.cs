using UnityEngine;

public sealed class SceneButtons : MonoBehaviour
{
    [SerializeField] private MonoBehaviour bootstrapperProvider; 

    private ISceneLoader Loader => ((IHasSceneLoader)bootstrapperProvider).SceneLoader;

    public void LoadMenu() => Loader.Load(Scenes.Menu);
    public void LoadGame() => Loader.Load(Scenes.Game);
    public void Restart()  => Loader.ReloadActive();
    public void Quit()     => Loader.Quit();
}
