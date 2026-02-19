using UnityEngine;
public static class BusResolver
{
    public static IEventBus Resolve(MonoBehaviour provider, Object fallbackContext = null)
    {
        if (provider != null && provider is IHasEventBus hasBus && hasBus.Bus != null)
            return hasBus.Bus;

        var app = Object.FindObjectOfType<AppBootstrapper>();
        if (app != null && app.Bus != null)
            return app.Bus;

        var game = Object.FindObjectOfType<GameBootstrapper>();
        if (game != null && game.Bus != null)
            return game.Bus;

      //  Debug.LogError($"EventBus not found. Assign a busProvider that implements {nameof(IHasEventBus)} " +
      //                 "or add AppBootstrapper to the scene.");
        return null;
    }
}
