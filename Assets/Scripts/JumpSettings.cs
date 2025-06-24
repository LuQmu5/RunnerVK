using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/JumpSettings")]
public class JumpSettings : ScriptableObject
{
    [Header("Основные параметры прыжка")]
    [Range(0.1f, 5f)] public float jumpDuration = 0.6f;

    [Header("Кривая прыжка (Y: 0-1)")]
    public AnimationCurve jumpCurve;

    public float forwardSpeed = 5;
}
