using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _forwardSpeed = 5f;
    [SerializeField] private float _sideSpeed = 10f;
    [SerializeField] private float _laneLimit = 3f;

    [Header("Jump")]
    [SerializeField] private float _jumpDuration = 1f;
    [SerializeField] private AnimationCurve _jumpCurve;
    [SerializeField] private float _jumpForwardMultiplier = 2f;

    [Header("Camera Effects")]
    [SerializeField] private CinemachineCamera _cinemachineCamera;
    [SerializeField] private float _jumpFovBoost = 10f;
    [SerializeField] private Vector3 _jumpOffsetBoost = new Vector3(0f, 0.5f, -1f);

    [Header("References")]
    [SerializeField] private PlayerInput _input;
    [SerializeField] private PlayerView _view;

    private float _targetX;
    private bool _isJumping = false;
    private float _horizontalInput = 0f;

    private float _defaultFov;
    private Vector3 _defaultOffset;
    private CinemachineFollow _composer;

    private void OnEnable()
    {
        _input.OnHorizontalChanged += OnHorizontalChanged;
        _input.OnJump += OnJumpRequested;
    }

    private void OnDisable()
    {
        _input.OnHorizontalChanged -= OnHorizontalChanged;
        _input.OnJump -= OnJumpRequested;
    }

    private void Start()
    {
        _targetX = transform.position.x;

        if (_cinemachineCamera != null)
        {
            _defaultFov = _cinemachineCamera.Lens.FieldOfView;
            _composer = _cinemachineCamera.GetComponent<CinemachineFollow>();

            if (_composer != null)
                _defaultOffset = _composer.FollowOffset;
        }
    }

    private void Update()
    {
        if (!_isJumping)
        {
            _targetX += _horizontalInput * _sideSpeed * Time.deltaTime;
            _targetX = Mathf.Clamp(_targetX, -_laneLimit, _laneLimit);
            UpdateMovement();
        }
        else
        {
            UpdateForwardOnlyMovement();
        }
    }

    private void OnHorizontalChanged(float value)
    {
        _horizontalInput = value;
    }

    private void OnJumpRequested()
    {
        if (!_isJumping)
            StartCoroutine(JumpRoutine());
    }

    private void UpdateMovement()
    {
        Vector3 pos = transform.position;
        float newX = Mathf.MoveTowards(pos.x, _targetX, _sideSpeed * Time.deltaTime);
        float newZ = pos.z + _forwardSpeed * Time.deltaTime;
        transform.position = new Vector3(newX, pos.y, newZ);

        _view.SetXSpeed(newX - pos.x);
    }

    private void UpdateForwardOnlyMovement()
    {
        Vector3 pos = transform.position;
        float newZ = pos.z + _forwardSpeed * Time.deltaTime;
        transform.position = new Vector3(pos.x, pos.y, newZ);
    }

    private IEnumerator JumpRoutine()
    {
        _isJumping = true;
        _view.SetJump(true);

        float camTransitionTime = 0.2f;
        float elapsed = 0f;

        float startFov = _cinemachineCamera.Lens.FieldOfView;
        float targetFov = _defaultFov + _jumpFovBoost;

        Vector3 startOffset = _composer.FollowOffset;
        Vector3 targetOffset = _defaultOffset + _jumpOffsetBoost;

        while (elapsed < camTransitionTime)
        {
            float t = elapsed / camTransitionTime;
            _cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(startFov, targetFov, t);
            _composer.FollowOffset = Vector3.Lerp(startOffset, targetOffset, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _cinemachineCamera.Lens.FieldOfView = targetFov;
        _composer.FollowOffset = targetOffset;

        // Ïðûæîê
        float time = 0f;
        Vector3 startPos = transform.position;

        while (time < _jumpDuration)
        {
            float normalizedTime = time / _jumpDuration;
            float curveY = _jumpCurve.Evaluate(normalizedTime);
            float y = startPos.y + curveY;
            float z = startPos.z + _forwardSpeed * _jumpForwardMultiplier * normalizedTime;
            transform.position = new Vector3(startPos.x, y, z);
            time += Time.deltaTime;
            yield return null;
        }

        float finalZ = startPos.z + _forwardSpeed * _jumpForwardMultiplier;
        transform.position = new Vector3(startPos.x, startPos.y, finalZ);

        elapsed = 0f;

        while (elapsed < camTransitionTime)
        {
            float t = elapsed / camTransitionTime;
            _cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(targetFov, _defaultFov, t);
            _composer.FollowOffset = Vector3.Lerp(targetOffset, _defaultOffset, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _cinemachineCamera.Lens.FieldOfView = _defaultFov;
        _composer.FollowOffset = _defaultOffset;

        _isJumping = false;
        _view.SetJump(false);
    }
}
