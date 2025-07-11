using System;
using UnityEngine;

public class LoseDisplay : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;

    private PlayerController _playerController;

    public void Init(PlayerController player)
    {
        _playerController = player;

        _playerController.Lose += Show;
    }

    private void OnDestroy()
    {
        if (_playerController == null)
            return;

        _playerController.Lose -= Show;
    }

    private void Show()
    {
        _canvasGroup.alpha = 1;
    }
}
