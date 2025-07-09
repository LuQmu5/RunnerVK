using System;
using UnityEngine;

public class MobilePlayerInput : IPlayerInput
{
    private const float MaxSwipeDistance = 300f; // максимальная длина, при которой достигается horizontal = 1
    private const float MinSwipeUpDistance = 50f;

    private const float MaxHorizontalValue = 1f;
    private const float MinHorizontalValue = -1f;

    public event Action<float> HorizontalInputChanged;
    public event Action JumpKeyPressed;

    private bool _enabled;

    private Vector2 _startPos;
    private float _startTime;
    private int _activeTouchId = -1;

    public void Enable() => _enabled = true;
    public void Disable()
    {
        _enabled = false;
        _activeTouchId = -1;
        HorizontalInputChanged?.Invoke(0f);
    }

    public void Update()
    {
        if (!_enabled || Input.touchCount == 0)
            return;

        foreach (Touch touch in Input.touches)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _startPos = touch.position;
                    _startTime = Time.time;
                    _activeTouchId = touch.fingerId;
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (touch.fingerId != _activeTouchId)
                        continue;

                    Vector2 delta = touch.position - _startPos;

                    if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                    {
                        float horizontal = Mathf.Clamp(delta.x / MaxSwipeDistance, MinHorizontalValue, MaxHorizontalValue);
                        HorizontalInputChanged?.Invoke(horizontal);
                    }
                    else if (delta.y > MinSwipeUpDistance)
                    {
                        JumpKeyPressed?.Invoke();
                        _activeTouchId = -1;
                        HorizontalInputChanged?.Invoke(0f);
                    }

                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (touch.fingerId == _activeTouchId)
                    {
                        _activeTouchId = -1;
                        HorizontalInputChanged?.Invoke(0f);
                    }
                    break;
            }
        }
    }
}
