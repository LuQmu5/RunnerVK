using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using System;

public class MainCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private CameraSettings _settings;

    private PlayerController _player;
    private CinemachineFollow _composer;

    private float _defaultFov;
    private Vector3 _defaultOffset;
    private Quaternion _defaultRotation;

    private Coroutine _zoomCoroutine;
    private Coroutine _tiltCoroutine;

    private void Awake()
    {
        _composer = _camera.GetComponent<CinemachineFollow>();
        _defaultFov = _camera.Lens.FieldOfView;
        _defaultOffset = _composer.FollowOffset;
        _defaultRotation = transform.rotation;
    }

    public void Init(PlayerController player)
    {
        _player = player;
        _camera.Target.TrackingTarget = player.transform;

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

        print("Handle Jump");
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

        // Сохранили последнее значение
        float peakFov = _camera.Lens.FieldOfView;

        // Плавно возвращаем обратно
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
        _camera.Target.TrackingTarget = null;

        transform.position = fallPoint + Vector3.up * _settings.fallLookHeight;
        transform.rotation = Quaternion.Euler(_settings.fallLookAngle, _player.transform.eulerAngles.y, 0f);
    }

    private void HandleStrafe(float direction)
    {
        if (_tiltCoroutine != null)
            StopCoroutine(_tiltCoroutine);

        _tiltCoroutine = StartCoroutine(TiltCamera(direction));
    }

    private IEnumerator TiltCamera(float direction)
    {
        float targetZ = direction * _settings.tiltAngle;
        float currentZ = transform.localEulerAngles.z;

        if (currentZ > 180f)
            currentZ -= 360f;

        float t = 0f;

        while (t < 1f)
        {
            float z = Mathf.Lerp(currentZ, targetZ, t);
            transform.localEulerAngles = new Vector3(
                transform.localEulerAngles.x,
                transform.localEulerAngles.y,
                z
            );

            t += Time.deltaTime * _settings.tiltSpeed;
            yield return null;
        }
    }

    private void HandleForkEnter(ForkData data)
    {
        _camera.Target.TrackingTarget = null;
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

        _camera.Target.TrackingTarget = _player.transform;

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
