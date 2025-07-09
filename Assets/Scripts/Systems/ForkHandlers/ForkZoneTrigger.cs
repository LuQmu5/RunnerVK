using UnityEngine;
using UnityEngine.UI;

public class ForkZoneTrigger : MonoBehaviour
{
    [SerializeField] private Transform _leftPoint;
    [SerializeField] private Transform _rightPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out var player))
        {
            player.EnterFork();
            gameObject.SetActive(false);
        }
    }
}
