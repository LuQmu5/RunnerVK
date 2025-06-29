using UnityEngine;

public class MovementHandler
{
    private readonly Transform _transform;
    private readonly MovementSettings _settings;

    private float _sideOffset = 0f;

    public MovementHandler(Transform transform, MovementSettings settings)
    {
        _transform = transform;
        _settings = settings;
    }

    public void Update(float horizontalInput, float deltaTime)
    {
        // просто накапливаем смещение без ограничения
        _sideOffset += horizontalInput * _settings.SideSpeed * deltaTime;

        // движение вбок — локальная ось X
        Vector3 sideMove = _transform.right * horizontalInput * _settings.SideSpeed * deltaTime;

        // движение вперёд — локальная ось Z
        Vector3 forwardMove = _transform.forward * _settings.ForwardSpeed * deltaTime;

        // суммарное движение
        _transform.position += forwardMove + sideMove;
    }
}
