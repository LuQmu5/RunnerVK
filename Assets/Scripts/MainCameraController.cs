using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class MainCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private CameraSettings _settings;
    [SerializeField] private CinemachineFramingTransposer _framingTransposer;

    private PlayerController _player;

    private float _defaultFov;
    private Vector3 _defaultOffset;
    private Quaternion _defaultRotation;

    private Coroutine _zoomCoroutine;
    private Coroutine _tiltCoroutine;
    private Coroutine _strafeCoroutine;

    private void Awake()
    {
        _defaultFov = _camera.Lens.FieldOfView;
        _defaultOffset = _framingTransposer.m_TrackedObjectOffset;
        _defaultRotation = transform.localRotation;
    }

    public void Init(PlayerController player)
    {
        _player = player;
        _camera.Follow = player.transform;
        _camera.LookAt = player.transform;

        _player.OnJump += HandleJump;
        _player.OnFall += HandleFall;
        _player.OnStrafe += HandleStrafe;
        _player.OnEnterFork += HandleForkEnter;
        _player.OnExitFork += HandleForkExit;
    }

    private void OnDestroy()
    {
        if (_player == null) 
            return;

        _player.OnJump -= HandleJump;
        _player.OnFall -= HandleFall;
        _player.OnStrafe -= HandleStrafe;
        _player.OnEnterFork -= HandleForkEnter;
        _player.OnExitFork -= HandleForkExit;
    }

    private void HandleJump()
    {
        if (_zoomCoroutine != null)
            StopCoroutine(_zoomCoroutine);

        _zoomCoroutine = StartCoroutine(ZoomDuringJump());
    }

    private IEnumerator ZoomDuringJump()
    {
        float duration = _settings.jumpZoomDuration;
        AnimationCurve curve = _settings.jumpZoomCurve;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float zoomOffset = curve.Evaluate(t);
            _camera.Lens.FieldOfView = _defaultFov + zoomOffset;
            elapsed += Time.deltaTime;
            yield return null;
        }

        float peakFov = _camera.Lens.FieldOfView;

        float recoveryDuration = _settings.zoomRecoveryDuration;
        elapsed = 0f;

        while (elapsed < recoveryDuration)
        {
            float t = elapsed / recoveryDuration;
            float eased = _settings.zoomRestoreEaseCurve.Evaluate(t);
            _camera.Lens.FieldOfView = Mathf.Lerp(peakFov, _defaultFov, eased);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _camera.Lens.FieldOfView = _defaultFov;
    }

    private void HandleFall(Vector3 fallPoint)
    {
        _camera.Follow = null;
        _camera.LookAt = null;

        transform.position = fallPoint + Vector3.up * _settings.fallLookHeight;
        transform.rotation = Quaternion.Euler(_settings.fallLookAngle, _player.transform.eulerAngles.y, 0f);
    }

    private void HandleStrafe(float direction)
    {
        if (_strafeCoroutine != null)
            StopCoroutine(_strafeCoroutine);

        if (_tiltCoroutine != null)
            StopCoroutine(_tiltCoroutine);

        _strafeCoroutine = StartCoroutine(StrafeRoutine(direction));
        _tiltCoroutine = StartCoroutine(TiltCamera(direction));
    }

    private IEnumerator StrafeRoutine(float direction)
    {
        float targetScreenX = 0.5f + direction * _settings.strafeScreenXOffset; // 0.5 - центр экрана
        float startScreenX = _framingTransposer.m_ScreenX;
        float duration = _settings.strafeTransitionDuration;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            _framingTransposer.m_ScreenX = Mathf.Lerp(startScreenX, targetScreenX, time / duration);
            yield return null;
        }

        _framingTransposer.m_ScreenX = targetScreenX;
    }


    private IEnumerator TiltCamera(float direction)
    {
        float targetAngle = direction * _settings.tiltAngle; // угол наклона камеры
        float duration = _settings.tiltTransitionDuration;

        Quaternion startRotation = transform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(startRotation.eulerAngles.x, startRotation.eulerAngles.y, targetAngle);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / duration);
            yield return null;
        }

        transform.localRotation = targetRotation;
    }

    private void HandleForkEnter(ForkData data)
    {
        _camera.Follow = null;
        _camera.LookAt = null;
        StartCoroutine(MoveToForkView(data.CameraLookPoint));
    }

    private IEnumerator MoveToForkView(Transform lookPoint)
    {
        float duration = _settings.forkTransitionDuration;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        Vector3 targetPos = lookPoint.position;
        Quaternion targetRot = lookPoint.rotation;

        float t = 0f;
        while (t < 1f)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            t += Time.unscaledDeltaTime / duration;
            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;
    }

    private void HandleForkExit()
    {
        StartCoroutine(ReturnToPlayer());
    }

    private IEnumerator ReturnToPlayer()
    {
        float duration = _settings.forkTransitionDuration;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        _camera.Follow = _player.transform;
        _camera.LookAt = _player.transform;

        Vector3 targetPos = _player.transform.position + _player.transform.TransformDirection(_defaultOffset);
        Quaternion targetRot = Quaternion.LookRotation(_player.transform.forward, Vector3.up);

        float t = 0f;
        while (t < 1f)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            t += Time.unscaledDeltaTime / duration;
            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;
    }
}
