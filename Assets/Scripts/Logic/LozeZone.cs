using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public sealed class LozeZone : MonoBehaviour
{
    [Header("Rules")]
    [SerializeField] private float stuckSeconds = 1.5f;
    
    [SerializeField] private string cubeTag = "Cube";

    [Header("Dependencies")]
    [SerializeField] private MonoBehaviour busProvider;

    private IEventBus _bus;
    private readonly Dictionary<CubeActor, float> _timers = new(64);
    private bool _gameOver;

    public void Construct(IEventBus bus) => _bus = bus;

    private void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        _bus ??= BusResolver.Resolve(busProvider, this);
    }

    private void OnDisable()
    {
        _timers.Clear();
        _gameOver = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (_gameOver) return;
        _bus ??= BusResolver.Resolve(busProvider, this);
        if (_bus == null) return;

        var cube = other.GetComponentInParent<CubeActor>();
        if (cube == null) return;

        if (!string.IsNullOrWhiteSpace(cubeTag))
        {
            bool ok = cube.CompareTag(cubeTag) || other.CompareTag(cubeTag);
            if (!ok) return;
        }

        if (!cube.gameObject.activeInHierarchy)
        {
            _timers.Remove(cube);
            return;
        }

        float t = _timers.TryGetValue(cube, out var prev) ? prev : 0f;
        t += Time.deltaTime;
        _timers[cube] = t;

        if (t >= stuckSeconds)
            TriggerGameOver();
    }

    private void OnTriggerExit(Collider other)
    {
        var cube = other.GetComponentInParent<CubeActor>();
        if (cube == null) return;

        if (!string.IsNullOrWhiteSpace(cubeTag))
        {
            bool ok = cube.CompareTag(cubeTag) || other.CompareTag(cubeTag);
            if (!ok) return;
        }

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
