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

    private void Start()
    {
        if (_mainCamera == null)
            _mainCamera = Camera.main;

        _targetX = transform.position.x;
    }

    private void Update()
    {
        if (_isJumping)
            return;

        HandleSwipeInput();

        Vector3 pos = transform.position;
        float newX = Mathf.MoveTowards(pos.x, _targetX, _sideSpeed * Time.deltaTime);
        float newZ = pos.z + _forwardSpeed * Time.deltaTime;
        transform.position = new Vector3(newX, pos.y, newZ);

        if (Mathf.Abs(_targetX - pos.x) > 0.1f)
        {
            if (_targetX > pos.x)
                _view.PlayRunRight();
            else
                _view.PlayRunLeft();
        }
        else
        {
            _view.PlayRun();
        }
    }

    private void HandleSwipeInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _swipeStart = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector2 swipeEnd = Input.mousePosition;
            Vector2 swipeDelta = swipeEnd - _swipeStart;

            if (Mathf.Abs(swipeDelta.y) > _minSwipeDistance && swipeDelta.y > 0)
            {
                StartCoroutine(JumpRoutine());
                return;
            }

            if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
            {
                Ray ray = _mainCamera.ScreenPointToRay(swipeEnd);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    _targetX = Mathf.Clamp(hit.point.x, -_laneLimit, _laneLimit);
                }
            }
        }
    }

    private IEnumerator JumpRoutine()
    {
        _isJumping = true;
        _view.PlayJump();

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
        _view.PlayRun();
    }
}
