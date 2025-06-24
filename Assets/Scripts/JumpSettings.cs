using UnityEngine;

[CreateAssetMenu(menuName = "Player/Jump Settings")]
public class JumpSettings : ScriptableObject
{
    public float totalDuration = 1.8f;      // Вся длительность анимации
    public float jumpStartTime = 0.18f;   // Начало подъема
    public float apexTime = 0.30f;        // Достижение вершины
    public float apexHoldTime = 0.50f;    // Конец "зависания" в воздухе
    public float landTime = 0.60f;        // Приземление
    public float jumpHeight = 1.5f;       // Максимальная высота подъема
    public float forwardSpeed = 3f;       // Скорость вперед во время прыжка
}
