using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private CanvasGroup _canvasGroup;

    public event Action Completed;

    public void Activate(string startText)
    {
        _canvasGroup.alpha = 1;
        _text.text = startText;

        StartCoroutine(CompletingRoutine());
    }

    private IEnumerator CompletingRoutine()
    {
        yield return new WaitUntil(() => Input.anyKeyDown);

        _canvasGroup.alpha = 0;

        Completed?.Invoke();
    }
}