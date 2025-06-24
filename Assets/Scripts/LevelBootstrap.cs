using UnityEngine;

public class LevelBootstrap : MonoBehaviour
{
    [SerializeField] private Transform _playerSpawnPoint;
    [SerializeField] private PlayerController _playerPrefab;
    [SerializeField] private MainCameraController _mainCameraControllerPrefab;

    private void Awake()
    {
        PlayerController player = Instantiate(_playerPrefab, _playerSpawnPoint.position, Quaternion.identity);

        RuntimePlatform platform = Application.platform;
        IPlayerInput input = platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer ? new MobilePlayerInput() : new PCPlayerInput();
        player.Init(input);

        MainCameraController camera = Instantiate(_mainCameraControllerPrefab);
        camera.Init(player);
    }
}