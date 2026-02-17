using System;

public interface IInputSource
{
    event Action OnPress;
    event Action<float> OnDragDeltaX;
    event Action OnRelease;
    bool Enabled { get; set; }
}