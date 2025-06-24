using UnityEngine;

[CreateAssetMenu(fileName = "CameraSettings", menuName = "Settings/Camera Settings")]
public class CameraSettings : ScriptableObject
{
    [Header("Jump Zoom")]
    public AnimationCurve jumpZoomCurve = AnimationCurve.EaseInOut(0, 0, 1, 10);
    public float jumpZoomDuration = 0.5f;

    [Header("Fall View")]
    public float fallLookHeight = 5f;
    public float fallLookAngle = 85f;

    [Header("Tilt Effects")]
    public float tiltAngle = 10f;
    public float tiltSpeed = 5f;

    [Header("Fork View Transition")]
    public float forkTransitionDuration = 0.5f;
}
