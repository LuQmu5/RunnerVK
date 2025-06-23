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
    [SerializeField] private float _minSwipeDistance = 50f;

    [Header("References")]
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private PlayerView _view;

    private float _targetX;
    private bool _isJumping = false;

    private Vector2 _swipeStart;
    private bool _inputHeld = false;
    private bool _jumpTriggered = false;

    private void Start()
    {
        if (_mainCamera == null)
            _mainCamera = Camera.main;

        _targetX = transform.position.x;
    }

    private void Update()
    {
        HandleInput();

        if (!_isJumping)
            UpdateMovement();
        else
            UpdateForwardOnlyMovement();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _swipeStart = Input.mousePosition;
            _inputHeld = true;
            _jumpTriggered = false;
        }

        if (Input.GetMouseButton(0) && _inputHeld && !_isJumping)
        {
            Vector2 current = Input.mousePosition;
            Vector2 delta = current - _swipeStart;

            // Проверка на прыжок
            if (!_jumpTriggered && delta.y > _minSwipeDistance)
            {
                _jumpTriggered = true;
                _inputHeld = true; // продолжает удерживаться после прыжка
                StartCoroutine(JumpRoutine());
                return;
            }

            // Горизонтальное движение
            float direction = Mathf.Sign(delta.x);
            _targetX += direction * _sideSpeed * Time.deltaTime;
            _targetX = Mathf.Clamp(_targetX, -_laneLimit, _laneLimit);
        }

        if (Input.GetMouseButtonUp(0))
        {
            _inputHeld = false;
        }
    }

    private void UpdateMovement()
    {
        Vector3 pos = transform.position;
        float newX = Mathf.MoveTowards(pos.x, _targetX, _sideSpeed * Time.deltaTime);
        float newZ = pos.z + _forwardSpeed * Time.deltaTime;
        transform.position = new Vector3(newX, pos.y, newZ);

        float delta = _targetX - pos.x;
        _view.SetXSpeed(delta);
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
