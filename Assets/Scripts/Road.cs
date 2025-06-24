using UnityEngine;

public class Road : MonoBehaviour
{
    public Transform StartPoint => transform;
    public Transform EndPoint; // Можно не использовать напрямую, но пригодится

    public Vector3 Forward => (EndPoint.position - transform.position).normalized;
    public Vector3 Right => Vector3.Cross(Vector3.up, Forward); // всегда вбок
}
