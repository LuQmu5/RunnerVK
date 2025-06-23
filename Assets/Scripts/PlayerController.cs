using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _forwardSpeed = 5f;
    [SerializeField] private float _sideSpeed = 10f;
    [SerializeField] private float _laneLimit = 3f;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private PlayerView _view;


    private float _targetX;

    private void Start()
    {
        if (_mainCamera == null)
            _mainCamera = Camera.main;

        _targetX = transform.position.x;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                _targetX = Mathf.Clamp(hit.point.x, -_laneLimit, _laneLimit);
            }
        }

        Vector3 currentPos = transform.position;
        float newX = Mathf.MoveTowards(currentPos.x, _targetX, _sideSpeed * Time.deltaTime);
        float newZ = currentPos.z + _forwardSpeed * Time.deltaTime;
        transform.position = new Vector3(newX, currentPos.y, newZ);


        if (_targetX > transform.position.x + 0.1f)
        {
            _view.PlayRunRight();
        }
        else if (_targetX < transform.position.x - 0.1f)
        {
            _view.PlayRunLeft();
        }
        else
        {
            _view.PlayRun();
        }

        // TESTS
        if (Input.GetKeyDown(KeyCode.R))
        {
            _view.PlayTakeHit();
        }
    }
}
