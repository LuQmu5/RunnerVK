using Lean.Touch;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MobilePlayerInput : IPlayerInput
{
    private const float MaxSwipeDistance = 300f; 
    private const float MaxRightHorizontalValue = 1f;
    private const float MaxLeftHorizontalValue = -1f;

    public event Action<float> HorizontalInputChanged;
    public event Action JumpKeyPressed;

    private bool _enabled;

    public void Enable()
    {
        LeanTouch.OnFingerTap += HandleFingerTap;
        LeanTouch.OnGesture += HandleGesture;
        _enabled = true;
    }

    public void Disable()
    {
        LeanTouch.OnFingerTap -= HandleFingerTap;
        LeanTouch.OnGesture -= HandleGesture;
        _enabled = false;
    }

    public void Update() { }

    private void HandleFingerTap(LeanFinger finger)
    {
        if (_enabled == false)
            return;

        if (finger != null && finger.Age <= 0.2f)
        {
            JumpKeyPressed?.Invoke();
        }
    }

    private void HandleGesture(List<LeanFinger> fingers)
    {
        if (_enabled == false || fingers.Count != 1)
            return;

        Vector2 delta = fingers[0].SwipeScreenDelta;

        if (IsHorizontalSwipe(delta))
        {
            float horizontal = Mathf.Clamp(delta.x / MaxSwipeDistance, MaxLeftHorizontalValue, MaxRightHorizontalValue);
            HorizontalInputChanged?.Invoke(horizontal);
        }
    }

    private static bool IsHorizontalSwipe(Vector2 delta)
    {
        return Mathf.Abs(delta.x) > Mathf.Abs(delta.y);
    }
}
