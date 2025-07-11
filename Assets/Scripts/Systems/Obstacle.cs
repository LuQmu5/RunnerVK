using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float _damagePerHit = 2;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamagable damagableActor))
        {
            damagableActor.TakeDamage(_damagePerHit);
            gameObject.SetActive(false);
        }
    }
}