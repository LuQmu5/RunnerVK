using UnityEngine;

public class LevelBootstrap : MonoBehaviour
{
    [SerializeField] private TutorialDisplay _tutorialDisplay;
    [SerializeField] private Transform _playerSpawnPoint;
    [SerializeField] private PlayerController _playerPrefab;
    [SerializeField] private PlayerCameraController _mainCameraControllerPrefab;
    [SerializeField] private ForkDesicionView _forkUI;

    private IPlayerInput _input;

    private void Awake()
    {
        bool isMobile = PlatformDetector.IsMobile();
        _input = isMobile ? new MobilePlayerInput() : new PCPlayerInput();

        _tutorialDisplay.Activate(isMobile? "Mobila" : "PC");
        _tutorialDisplay.Completed += InitPlayer;
    }

    private void InitPlayer()
    {
        _tutorialDisplay.Completed -= InitPlayer;

        PlayerController player = Instantiate(_playerPrefab, _playerSpawnPoint.position, Quaternion.identity);

        player.Init(_input);
        PlayerCameraController camera = Instantiate(_mainCameraControllerPrefab);
        camera.Init(player);

        _forkUI.Init(player);
    }
}

