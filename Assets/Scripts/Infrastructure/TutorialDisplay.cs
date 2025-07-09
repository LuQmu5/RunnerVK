using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private CanvasGroup _canvasGroup;

    public event Action Completed;

    public void Activate(string platformLabel)
    {
        _canvasGroup.alpha = 1;

        if (platformLabel == "PC")
        {
            _text.text =
                "<sprite name=a>: движение влево\n" +
                "<sprite name=d>: движение вправо\n" +
                "<sprite name=space>: прыжок\n";
        }
        else
        {
            _text.text =
                "свайп <sprite name=kl>: движение влево\n" +
                "свайп <sprite name=kr>: движение вправо\n" +
                "свайп <sprite name=ku>: прыжок\n";
        }

        StartCoroutine(CompletingRoutine());
    }


    private IEnumerator CompletingRoutine()
    {
        yield return new WaitUntil(() => Input.anyKeyDown);

        _canvasGroup.alpha = 0;

        Completed?.Invoke();
    }
}