using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class ForkZoneTrigger : MonoBehaviour
{
    [SerializeField] private Transform _leftPoint;
    [SerializeField] private Transform _rightPoint;
    [SerializeField] private CinemachineCamera _camera;

    private PlayerController _playerController;
    private bool _triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered)
            return;

        if (other.TryGetComponent<PlayerController>(out var player))
        {
            _triggered = true;
            _playerController = player;
            _camera.gameObject.SetActive(true);
            player.ForkExited += OnPlayerExitedFork;
            player.EnterFork();
        }
    }

    private void OnPlayerExitedFork()
    {
        _playerController.ForkExited -= OnPlayerExitedFork;
        _camera.gameObject.SetActive(false);
    }
}
