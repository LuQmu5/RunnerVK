using DG.Tweening;
using TMPro;
using UnityEngine;

public class ForkDesicionView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _panel;
    [SerializeField] private TMP_Text _text;

    private PlayerController _player;

    public void Init(PlayerController player, string hintText)
    {
        _player = player;

        _player.ForkEntered += Show;
        _player.ForkExited += Hide;

        _text.text = hintText;
    }

    private void OnDestroy()
    {
        _player.ForkEntered -= Show;
        _player.ForkExited -= Hide;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        _panel.DOFade(1f, 0.3f).SetEase(Ease.OutQuad);
    }

    public void Hide()
    {
        _panel.DOFade(0f, 0.3f).SetEase(Ease.InQuad).OnComplete(() => gameObject.SetActive(false));
    }
}
