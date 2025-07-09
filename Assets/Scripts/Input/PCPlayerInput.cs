using System;
using UnityEngine;

public class PCPlayerInput : IPlayerInput
{
    private const string HorizontalAxisName = "Horizontal";
    private const float MaxRightHorizontalValue = 1f;
    private const float MaxLeftHorizontalValue = -1f;
    private const KeyCode JumpKey = KeyCode.Space;

    public event Action<float> HorizontalInputChanged;
    public event Action JumpKeyPressed;

    private bool _enabled;
    private float _horizontalInputSensetivity = 2;

    public void Enable() => _enabled = true;
    public void Disable() => _enabled = false;

    public void Update()
    {
        if (_enabled == false)
            return;

        float horizontal = Input.GetAxis(HorizontalAxisName) * _horizontalInputSensetivity;
        HorizontalInputChanged?.Invoke(Mathf.Clamp(horizontal, MaxLeftHorizontalValue, MaxRightHorizontalValue));

        if (Input.GetKeyDown(JumpKey))
            JumpKeyPressed?.Invoke();
    }
}
