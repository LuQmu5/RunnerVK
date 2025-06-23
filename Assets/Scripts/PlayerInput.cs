using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public event Action<float> OnHorizontalChanged; // -1 .. 1 (влево-вправо)
    public event Action OnJump;

    private Vector2 _touchStart;
    private bool _touchActive = false;
    private float _minSwipeDistance = 50f;

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleKeyboardInput();
#else
        HandleTouchInput();
#endif
    }

    private void HandleKeyboardInput()
    {
        float horizontal = 0f;
        if (Input.GetKey(KeyCode.A))
            horizontal = -1f;
        else if (Input.GetKey(KeyCode.D))
            horizontal = 1f;

        OnHorizontalChanged?.Invoke(horizontal);

        if (Input.GetKeyDown(KeyCode.Space))
            OnJump?.Invoke();
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 0)
        {
            OnHorizontalChanged?.Invoke(0f);
            _touchActive = false;
            return;
        }

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            _touchStart = touch.position;
            _touchActive = true;
        }
        else if (touch.phase == TouchPhase.Moved && _touchActive)
        {
            Vector2 delta = touch.position - _touchStart;

            // Горизонтальное движение - нормируем по ширине экрана (чтобы было -1..1)
            float horizontal = Mathf.Clamp(delta.x / Screen.width, -1f, 1f);
            OnHorizontalChanged?.Invoke(horizontal);

            // Прыжок
            if (delta.y > _minSwipeDistance)
            {
                _touchActive = false;
                OnJump?.Invoke();
            }
        }
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            _touchActive = false;
            OnHorizontalChanged?.Invoke(0f);
        }
    }
}
