using System;

public interface IPlayerInput
{
    event Action<float> HorizontalInputChanged;
    event Action JumpKeyPressed;

    void Enable();
    void Disable();
    void Update();
}
