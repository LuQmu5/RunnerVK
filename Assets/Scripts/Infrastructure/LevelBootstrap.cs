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

    [SerializeField] private LoseDisplay _loseDisplay;
    [SerializeField] private WinDisplay _winDisplay;

    private IPlayerInput _input;
    private bool _isMobile;

    private void Awake()
    {
        _isMobile = PlatformDetector.IsMobile();
        _input = _isMobile ? new MobilePlayerInput() : new PCPlayerInput();

        _tutorialDisplay.Activate(_isMobile ? "Mobila" : "PC");
        _tutorialDisplay.Completed += OnTutorialCompleted;

        GameRestarter gameRestarter = new GameRestarter();
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
        _forkUI.Init(player, _isMobile? 
            "свайп <sprite name=kl>: выбрать левую развилку\nсвайп <sprite name=kr>: выбрать правую развилку" 
            : "<sprite name=a>: выбрать левую развилку\n<sprite name=d>: выбрать правую развилку");

        _loseDisplay.Init(player);
        _winDisplay.Init(player);
    }
}
