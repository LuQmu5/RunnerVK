using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class LevelBootstrap : MonoBehaviour
{
    [SerializeField] private TutorialDisplay _tutorialDisplay;
    [SerializeField] private Transform _playerSpawnPoint;
    [SerializeField] private PlayerController _playerPrefab;
    [SerializeField] private PlayerCameraController _mainCameraControllerPrefab;
    [SerializeField] private ForkDesicionView _forkUI;
    [SerializeField] private CinemachineCamera _introCamera;
    [SerializeField] private float _delayBeforeStart = 1.5f;

    private IPlayerInput _input;

    private void Awake()
    {
        bool isMobile = PlatformDetector.IsMobile();
        _input = isMobile ? new MobilePlayerInput() : new PCPlayerInput();

        _tutorialDisplay.Activate(isMobile ? "Mobila" : "PC");
        _tutorialDisplay.Completed += OnTutorialCompleted;
    }

    private void OnTutorialCompleted()
    {
        _tutorialDisplay.Completed -= OnTutorialCompleted;
        StartCoroutine(StartGameSequence());
    }

    private IEnumerator StartGameSequence()
    {
        PlayerController player = Instantiate(_playerPrefab, _playerSpawnPoint.position, Quaternion.identity);

        PlayerCameraController camera = Instantiate(_mainCameraControllerPrefab);
        camera.Init(player);
        camera.GetComponent<CinemachineCamera>().Priority = 30;

        yield return new WaitForSeconds(_delayBeforeStart);

        player.Init(_input);
        _forkUI.Init(player);
    }
}
