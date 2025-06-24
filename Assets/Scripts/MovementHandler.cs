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

    public void Update(float horizontalInput)
    {
        _sideOffset += horizontalInput * _settings.sideSpeed * Time.deltaTime;
        _sideOffset = Mathf.Clamp(_sideOffset, -_settings.sideClampDistance, _settings.sideClampDistance);

        Vector3 right = _transform.right;
        Vector3 rightOffset = right * (_sideOffset - Vector3.Dot(_transform.position, right));

        Vector3 forwardMove = _transform.forward * _settings.forwardSpeed * Time.deltaTime;

        _transform.position += forwardMove + rightOffset;
    }
}
