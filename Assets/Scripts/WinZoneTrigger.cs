using Unity.Cinemachine;
using UnityEngine;

public class WinZoneTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _camera;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController playerController))
        {
            _camera.gameObject.SetActive(true);
            playerController.HandleWin();
        }
    }
}
