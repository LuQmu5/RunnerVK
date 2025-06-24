using UnityEngine;

[CreateAssetMenu(menuName = "Camera/Camera Settings")]
public class CameraSettings : ScriptableObject
{
    public float jumpZoomDuration = 1f;
    public AnimationCurve jumpZoomCurve;
    public float zoomRecoveryDuration = 0.5f;
    public AnimationCurve zoomRestoreEaseCurve;

    public float fallLookHeight = 2f;
    public float fallLookAngle = 45f;

    public float tiltAngle = 10f;               // Макс угол наклона камеры при страйфе
    public float tiltTransitionDuration = 0.3f; // Время плавного перехода наклона

    public float strafeScreenXOffset = 0.15f;    // Смещение камеры по горизонтали (ScreenX) при страйфе
    public float strafeTransitionDuration = 0.3f; // Время плавного перехода смещения

    public float forkTransitionDuration = 1f;
}
