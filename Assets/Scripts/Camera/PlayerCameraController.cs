using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using DG.Tweening;
using System;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private CinemachineThirdPersonFollow _thirdPersonFollow;
    [SerializeField] private PlayerCameraSettings _settings;
    [SerializeField] private CameraOccluderFader _occluderFader;

    private PlayerController _player;
    private Coroutine _jumpCameraRoutine;
    private float _defaultDistance;

    public event Action OnIntroComplete;

    public void Init(PlayerController player)
    {
        _player = player;
        _defaultDistance = _thirdPersonFollow.CameraDistance;

        // Прежде чем прикрепиться к игроку — делаем пролет камеры
        StartCoroutine(IntroCameraRoutine());
    }

    private IEnumerator IntroCameraRoutine()
    {
        Vector3 introStartOffset = new Vector3(0f, _settings.IntroHeight, -_settings.IntroDistance);
        Vector3 targetOffset = new Vector3(0f, _thirdPersonFollow.VerticalArmLength, -_defaultDistance);

        Transform cameraTransform = _camera.transform;
        Transform playerTransform = _player.transform;

        Vector3 startPosition = playerTransform.position + introStartOffset;
        Vector3 targetPosition = playerTransform.position + targetOffset;

        cameraTransform.position = startPosition;
        cameraTransform.LookAt(playerTransform.position + Vector3.up * 1.2f);

        float duration = _settings.IntroDuration;

        // Движение по дуге (вниз)
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float curvedT = EaseOutCubic(t);

            Vector3 currentPosition = Vector3.Slerp(startPosition - playerTransform.position, targetPosition - playerTransform.position, curvedT) + playerTransform.position;
            cameraTransform.position = currentPosition;
            cameraTransform.LookAt(playerTransform.position + Vector3.up * 1.2f);

            yield return null;
        }

        // Только теперь прикрепляем камеру к игроку
        _camera.Target.TrackingTarget = playerTransform;

        // Камера готова, можно инициировать игрока
        _occluderFader.SetTarget(playerTransform);
        _player.Jumped += HandleJump;

        OnIntroComplete?.Invoke();
    }

    private float EaseOutCubic(float t)
    {
        return 1 - Mathf.Pow(1 - t, 3);
    }

    private void OnDestroy()
    {
        if (_player != null)
            _player.Jumped -= HandleJump;
    }

    private void HandleJump()
    {
        if (_jumpCameraRoutine != null)
            StopCoroutine(_jumpCameraRoutine);

        _jumpCameraRoutine = StartCoroutine(JumpCameraRoutine());
    }

    private IEnumerator JumpCameraRoutine()
    {
        float targetDistance = _defaultDistance + _settings.JumpDistanceIncrease;
        float zoomOutDuration = _settings.JumpZoomOutDuration;
        float returnDuration = _settings.JumpReturnDuration;
        float holdDuration = _settings.JumpHoldDuration;

        float defaultArm = _thirdPersonFollow.VerticalArmLength;
        float targetArm = defaultArm + _settings.JumpVerticalOffset;

        // Параллельный tween: отдаление и подъем камеры
        Sequence jumpSequence = DOTween.Sequence();

        jumpSequence.Join(DOTween.To(
            () => _thirdPersonFollow.CameraDistance,
            v => _thirdPersonFollow.CameraDistance = v,
            targetDistance,
            zoomOutDuration
        ).SetEase(Ease.OutQuad));

        jumpSequence.Join(DOTween.To(
            () => _thirdPersonFollow.VerticalArmLength,
            v => _thirdPersonFollow.VerticalArmLength = v,
            targetArm,
            zoomOutDuration
        ).SetEase(Ease.OutQuad));

        yield return jumpSequence.WaitForCompletion();

        // Пауза в максимальной точке
        yield return new WaitForSeconds(holdDuration);

        // Плавный возврат к исходному положению
        Sequence returnSequence = DOTween.Sequence();

        returnSequence.Join(DOTween.To(
            () => _thirdPersonFollow.CameraDistance,
            v => _thirdPersonFollow.CameraDistance = v,
            _defaultDistance,
            returnDuration
        ).SetEase(Ease.OutQuad));

        returnSequence.Join(DOTween.To(
            () => _thirdPersonFollow.VerticalArmLength,
            v => _thirdPersonFollow.VerticalArmLength = v,
            defaultArm,
            returnDuration
        ).SetEase(Ease.OutQuad));

        yield return returnSequence.WaitForCompletion();

        _jumpCameraRoutine = null;
    }
}
