using UnityEngine;

[CreateAssetMenu(menuName = "Player/Movement Settings")]
public class MovementSettings : ScriptableObject
{
    public float forwardSpeed = 5f;
    public float sideSpeed = 5f;
    public float sideClampDistance = 3f; // ограничение по X (влево-вправо)
}
