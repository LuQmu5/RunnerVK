using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamagable
{
    public event Action Jumped;
    public event Action ForkEntered;
    public event Action ForkExited;

    private const float LeftTurnValue = -45f;
    private const float RightTurnValue = 45f;

    [SerializeField] private JumpSettings _jumpSettings;
    [SerializeField] private PlayerView _view;
    [SerializeField] private MovementSettings _movementSettings;

    private MovementHandler _movementHandler;
    private JumpHandler _jumpHandler;

    private bool _onFork = false;
    private float _currentHorizontal = 0f;
    private float _currentForkHorizontal = 0;

    public IPlayerInput Input { get; private set; } = null;

    public void Init(IPlayerInput input)
    {
        Input = input;
        Input.Enable();

        Input.HorizontalInputChanged += OnHorizontalChanged;
        Input.JumpKeyPressed += OnJumpRequested;

        _jumpHandler = new JumpHandler(this, _jumpSettings);
        _movementHandler = new MovementHandler(transform, _movementSettings);

        _view.SetJumpSpeedMultiplier(_view.GetAnimationClipLength("Jump") / _jumpSettings.JumpTime);
    }

    private void OnDestroy()
    {
        Input.HorizontalInputChanged -= OnHorizontalChanged;
        Input.JumpKeyPressed -= OnJumpRequested;
    }

    private void Update()
    {
        if (_jumpHandler.IsJumping)
            return;

        Input.Update();

        if (_onFork == false)
        {
            _movementHandler.Update(_currentHorizontal, Time.deltaTime);
            _view.UpdateSpeedXParam(_currentHorizontal);
        }
    }

    private void OnHorizontalChanged(float value)
    {
        if (_onFork)
            _currentForkHorizontal = value;
        else
            _currentHorizontal = value;
    }

    private void OnJumpRequested()
    {
        if (_jumpHandler.TryJump())
        {
            _currentHorizontal = 0;
            _view.SetJumpTrigger();
            Jumped.Invoke();
        }
    }

    public void TakeDamage()
    {
        _view.SetHitTrigger();
    }

    public void EnterFork()
    {
        StartCoroutine(ForkRoutine());
    }

    private IEnumerator ForkRoutine()
    {
        _onFork = true;
        _currentHorizontal = 0;
        _currentForkHorizontal = 0;

        _view.SetIdlingState(true);

        ForkEntered?.Invoke();

        yield return new WaitUntil(() => IsHoldDirectionValueCompleted());

        float angleY = _currentForkHorizontal < 0 ? LeftTurnValue : RightTurnValue;
        string rotateDirection = _currentForkHorizontal < 0 ? "Left" : "Right";
        Quaternion targetRotation = Quaternion.Euler(0f, transform.eulerAngles.y + angleY, 0f);

        _view.SetRotateTriggerFor(rotateDirection);
        float rotateTime = _view.GetAnimationClipLength("Rotate" + rotateDirection);

        yield return transform
            .DORotateQuaternion(targetRotation, rotateTime)
            .SetEase(Ease.OutQuad)
            .WaitForCompletion();

        ExitFork();
    }

    private void ExitFork()
    {
        _onFork = false;
        _view.SetIdlingState(false);
        ForkExited?.Invoke();
    }

    private bool IsHoldDirectionValueCompleted()
    {
        return Mathf.Abs(_currentForkHorizontal) == -1 || Mathf.Abs(_currentForkHorizontal) == 1;
    }
}
