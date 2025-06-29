using DG.Tweening;
using UnityEngine;

public class ForkDesicionView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _panel;

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
