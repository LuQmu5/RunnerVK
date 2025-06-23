using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _forwardSpeed = 5f;
    [SerializeField] private float _sideSpeed = 10f;
    [SerializeField] private float _laneLimit = 3f;

    [Header("Jump")]
    [SerializeField] private float _jumpDuration = 1f;
    [SerializeField] private AnimationCurve _jumpCurve;

    [Header("References")]
    [SerializeField] private PlayerInput _input;
    [SerializeField] private PlayerView _view;

    private float _targetX;
    private bool _isJumping = false;

    private float _horizontalInput = 0f;

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

        float time = 0f;
        Vector3 startPos = transform.position;

        while (time < _jumpDuration)
        {
            float curveY = _jumpCurve.Evaluate(time / _jumpDuration);
            float y = startPos.y + curveY;
            float z = startPos.z + _forwardSpeed * time;
            transform.position = new Vector3(startPos.x, y, z);

            time += Time.deltaTime;
            yield return null;
        }

        Vector3 endPos = new Vector3(startPos.x, startPos.y, startPos.z + _forwardSpeed * _jumpDuration);
        transform.position = endPos;

        _isJumping = false;
        _view.SetJump(false);
    }
}
