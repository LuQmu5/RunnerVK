using UnityEngine;

[CreateAssetMenu(menuName = "Camera/Camera Settings")]
public class CameraSettings : ScriptableObject
{
    public float jumpZoomDuration = 0.2f;           // насколько быстро расширяется
    public AnimationCurve jumpZoomCurve;            // кривая резкого расширения

    public float zoomRecoveryDuration = 0.4f;        // сколько времени на возвращение FOV
    public AnimationCurve zoomRestoreEaseCurve;      // кривая возврата (например, EaseOut)

    public float fallLookHeight = 5f;
    public float fallLookAngle = 85f;

    public float tiltAngle = 10f;
    public float tiltSpeed = 5f;

    public float forkTransitionDuration = 0.5f;
}
