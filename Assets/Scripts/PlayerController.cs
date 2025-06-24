using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Road Movement")]
    [SerializeField] private Road _currentRoad;
    [SerializeField] private float _laneLimit = 3f;
    [SerializeField] private float _forwardSpeed = 5f;
    [SerializeField] private float _sideSpeed = 10f;

    [Header("Jump")]
    [SerializeField] private AnimationCurve _jumpHeightCurve;
    [SerializeField] private float _jumpForwardMultiplier = 2f;

    [Header("Camera Effects")]
    [SerializeField] private CinemachineCamera _cinemachineCamera;
    [SerializeField] private float _jumpFovBoost = 10f;
    [SerializeField] private Vector3 _jumpOffsetBoost = new Vector3(0f, 0.5f, -1f);

    [Header("References")]
    [SerializeField] private PCPlayerInput _input;
    [SerializeField] private PlayerView _view;
    [SerializeField] private Rigidbody _rigidbody;

    public bool IsJumping { get; private set; } = false;
    public bool IsDead { get; private set; } = false;

    public IPlayerInput Input => _input;

    private float _laneOffset = 0f;
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
        _input.Enable();

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
        if (IsJumping || IsDead || _currentRoad == null)
            return;

        _laneOffset += _horizontalInput * _sideSpeed * Time.deltaTime;
        _laneOffset = Mathf.Clamp(_laneOffset, -_laneLimit, _laneLimit);

        Vector3 move = _currentRoad.Forward * _forwardSpeed * Time.deltaTime;
        Vector3 lateral = _currentRoad.Right * (_laneOffset - Vector3.Dot(transform.position - _currentRoad.transform.position, _currentRoad.Right));

        transform.position += move + lateral;

        _view.SetXSpeed(_horizontalInput);

        if (_rigidbody.linearVelocity.y < -5f)
        {
            _cinemachineCamera.Target.TrackingTarget = null;
            _view.gameObject.SetActive(false);
            _input.Disable();
            IsDead = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out Obstacle obstacle))
        {
            Destroy(collision.gameObject);
            _view.PlayHit();
        }
    }

    private void OnHorizontalChanged(float value)
    {
        _horizontalInput = value;
    }

    private void OnJumpRequested()
    {
        if (!IsJumping)
            StartCoroutine(JumpRoutine());
    }

    public void SetRoad(Road newRoad)
    {
        _currentRoad = newRoad;
        _laneOffset = 0f;
        transform.rotation = Quaternion.LookRotation(_currentRoad.Forward, Vector3.up);
    }

    private IEnumerator JumpRoutine()
    {
        IsJumping = true;
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

        float time = 0f;
        Vector3 startPos = transform.position;
        float totalJumpTime = _jumpHeightCurve.keys[^1].time;

        while (time < totalJumpTime)
        {
            float y = startPos.y + _jumpHeightCurve.Evaluate(time);
            Vector3 jumpOffset = _currentRoad.Forward * _forwardSpeed * _jumpForwardMultiplier * time;
            transform.position = new Vector3(startPos.x + jumpOffset.x, y, startPos.z + jumpOffset.z);
            time += Time.deltaTime;
            yield return null;
        }

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

        IsJumping = false;
        _view.SetJump(false);
    }
}
