using System;
using UnityEngine;

public class PCPlayerInput : IPlayerInput
{
    public event Action<float> OnHorizontalChanged;
    public event Action OnJump;
    public event Action OnLeftPressed;
    public event Action OnRightPressed;

    private bool _enabled;
    private Vector3 _lastMousePosition;

    public void Enable() => _enabled = true;
    public void Disable() => _enabled = false;


    public void Update()
    {
        if (!_enabled)
            return;

        float horizontal = 0f;

        // Клавиши
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            horizontal = -1f;
            OnLeftPressed?.Invoke();
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            horizontal = 1f;
            OnRightPressed?.Invoke();
        }

        /*
        else
        {
            // Мышь
            Vector3 currentMouse = Input.mousePosition;
            float deltaX = currentMouse.x - _lastMousePosition.x;
            const float sensitivityThreshold = 2f; 

            if (Mathf.Abs(deltaX) > sensitivityThreshold)
            {
                horizontal = deltaX < 0f ? -1f : 1f;

                if (horizontal < 0f)
                    OnLeftPressed?.Invoke();
                else
                    OnRightPressed?.Invoke();
            }
            else
            {
                horizontal = 0f;
            }

            _lastMousePosition = currentMouse;
        }
        */

        OnHorizontalChanged?.Invoke(horizontal);

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            OnJump?.Invoke();
    }


    public int GetForkDirection()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) 
            return -1;
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) 
            return 1;

        return 0;
    }
}
