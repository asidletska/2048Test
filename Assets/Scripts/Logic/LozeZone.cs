using System.Collections.Generic;
using UnityEngine;

public class LozeZone : MonoBehaviour
{
    [Header("Lose condition")]
    [SerializeField] private float stuckSeconds = 1.5f;

    [Header("Tag filter")]
    [SerializeField] private string cubeTag = "Cube";

    private IEventBus _bus;

    private readonly Dictionary<CubeActor, float> _timers = new(64);

    private bool _gameOver;

    public void Construct(IEventBus bus)
    {
        _bus = bus;
    }

    private void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnDisable()
    {
        _timers.Clear();
        _gameOver = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (_gameOver) return;
        if (_bus == null) return;

        if (!other.CompareTag(cubeTag))
            return;

        var cube = other.GetComponentInParent<CubeActor>();

        if (cube == null)
            return;

        if (!cube.gameObject.activeInHierarchy)
        {
            _timers.Remove(cube);
            return;
        }

        float time = 0f;

        if (_timers.TryGetValue(cube, out var prev))
            time = prev;

        time += Time.fixedDeltaTime;

        _timers[cube] = time;

        if (time >= stuckSeconds)
            TriggerGameOver();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(cubeTag))
            return;

        var cube = other.GetComponentInParent<CubeActor>();

        if (cube != null)
            _timers.Remove(cube);
    }

    private void TriggerGameOver()
    {
        if (_gameOver) return;

        _gameOver = true;

        _bus.Publish(new GameOverEvent());

        _bus.Publish(new OpenPanelEvent(UiPanelId.GameOver));

        _bus.Publish(new PauseRequestedEvent());
    }
}
