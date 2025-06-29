using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IDamagable
{
    public event Action Jumped;

    [SerializeField] private JumpSettings _jumpSettings;
    [SerializeField] private PlayerView _view;
    [SerializeField] private MovementSettings _movementSettings;

    private MovementHandler _movementHandler;
    private JumpHandler _jumpHandler;

    private float _currentHorizontal = 0f;

    public IPlayerInput Input { get; private set; } = null;

    public void Init(IPlayerInput input)
    {
        Input = input;
        Input.Enable();

        Input.OnHorizontalChanged += OnHorizontalChanged;
        Input.OnJump += OnJumpRequested;

        _jumpHandler = new JumpHandler(this, _jumpSettings);
        _movementHandler = new MovementHandler(transform, _movementSettings);

        _view.SetJumpSpeedMultiplier(_view.GetAnimationClipLength("Jump") / _jumpSettings.JumpTime);
    }

    private void OnDestroy()
    {
        Input.OnHorizontalChanged -= OnHorizontalChanged;
        Input.OnJump -= OnJumpRequested;
    }

    private void Update()
    {
        Input.Update();

        if (_jumpHandler.IsJumping == false)
        {
            _movementHandler.Update(_currentHorizontal, Time.deltaTime);
            _view.UpdateSpeedXParam(_currentHorizontal);
        }
    }

    private void OnHorizontalChanged(float value)
    {
        _currentHorizontal = value;
    }

    private void OnJumpRequested()
    {
        if (_jumpHandler.TryJump())
        {
            _view.SetJumpTrigger();
            Jumped.Invoke();
        }
    }

    public void TakeDamage()
    {
        _view.SetHitTrigger();
    }
}
