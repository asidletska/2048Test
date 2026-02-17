using System;

public interface IEventBus
{
    void Publish<T>(T evt) where T : struct;
    IDisposable Subscribe<T>(Action<T> handler) where T : struct;
}