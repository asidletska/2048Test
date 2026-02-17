using System;
using System.Collections.Generic;

public sealed class StateMachine
{
    private readonly Dictionary<Type, IState> _states = new(16);
    private IState _current;

    public void Add<T>(T state) where T : class, IState => _states[typeof(T)] = state;

    public void Set<T>() where T : class, IState
    {
        if (!_states.TryGetValue(typeof(T), out var next))
            throw new InvalidOperationException($"State not registered: {typeof(T).Name}");

        _current?.Exit();
        _current = next;
        _current.Enter();
    }

    public void Tick(float dt) => _current?.Tick(dt);
    public void FixedTick(float fdt) => _current?.FixedTick(fdt);
}
