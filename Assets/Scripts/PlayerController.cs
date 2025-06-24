using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public event Action OnJump;
    public event Action<Vector3> OnFall;
    public event Action<float> OnStrafe;
    public event Action<ForkData> OnEnterFork;
    public event Action OnExitFork;

    [SerializeField] private JumpSettings _jumpSettings;
    [SerializeField] private PlayerView _view;

    private JumpHandler _jumpHandler;

    public IPlayerInput Input { get; private set; } = null;


    public void Init(IPlayerInput input)
    {
        Input = input;
        Input.Enable();

        Input.OnHorizontalChanged += OnHorizontalChanged;
        Input.OnJump += OnJumpRequested;

        _jumpHandler = new JumpHandler(transform, this, _jumpSettings);
    }

    private void OnDestroy()
    {
        Input.OnHorizontalChanged -= OnHorizontalChanged;
        Input.OnJump -= OnJumpRequested;
    }

    private void Update()
    {
        Input.Update();
    }

    private void OnHorizontalChanged(float value)
    {
        
    }

    private void OnJumpRequested()
    {
        if (_jumpHandler.TryJump())
        {
            _view.SetJumpTrigger();
            OnJump.Invoke();
        }
    }
}

