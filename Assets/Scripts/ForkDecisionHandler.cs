using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class ForkDecisionHandler
{
    private readonly IPlayerInput _input;
    private readonly MonoBehaviour _coroutineRunner;
    private readonly float _decisionTime;
    private readonly GameObject _forkUI;
    private readonly TMP_Text _instructionText;

    private Coroutine _decisionCoroutine;
    private float _currentInput = 0f;

    private Coroutine _activeTimerCoroutine;
    private bool _decisionMade = false;

    public ForkDecisionHandler(
        IPlayerInput input,
        MonoBehaviour coroutineRunner,
        float decisionTime,
        GameObject forkUI,
        TMP_Text instructionText)
    {
        _input = input;
        _coroutineRunner = coroutineRunner;
        _decisionTime = decisionTime;
        _forkUI = forkUI;
        _instructionText = instructionText;
    }

    public void StartDecision(Action<string> onDecision, Func<bool> isPlayerDead)
    {
        if (_decisionCoroutine != null)
            return;

        _decisionCoroutine = _coroutineRunner.StartCoroutine(DecisionRoutine(onDecision, isPlayerDead));
    }

    private IEnumerator DecisionRoutine(Action<string> onDecision, Func<bool> isPlayerDead)
    {
        _decisionMade = false;
        Time.timeScale = 0.2f;

        _forkUI.SetActive(true);
        _instructionText.text = Application.isMobilePlatform
            ? "Свайпните влево или вправо, чтобы выбрать путь"
            : "Удерживайте A (влево) или D (вправо), чтобы выбрать путь";

        _input.OnHorizontalChanged += OnHorizontalInput;

        while (!_decisionMade)
        {
            if (isPlayerDead())
                break;

            yield return null;
        }

        Cleanup();

        if (isPlayerDead())
        {
            Debug.Log("Принятие решения отменено — игрок мёртв");
        }

        _forkUI.SetActive(false);
        Time.timeScale = 1f;
    }

    private void OnHorizontalInput(float value)
    {
        if (Mathf.Approximately(value, _currentInput))
            return;

        _currentInput = value;

        if (_activeTimerCoroutine != null)
            _coroutineRunner.StopCoroutine(_activeTimerCoroutine);

        if (value < -0.1f)
        {
            _activeTimerCoroutine = _coroutineRunner.StartCoroutine(DecisionTimer("левый"));
        }
        else if (value > 0.1f)
        {
            _activeTimerCoroutine = _coroutineRunner.StartCoroutine(DecisionTimer("правый"));
        }
    }

    private IEnumerator DecisionTimer(string direction)
    {
        float elapsed = 0f;

        while (elapsed < _decisionTime)
        {
            if (Mathf.Abs(_currentInput) < 0.1f)
                yield break;

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        Debug.Log($"Выбран {direction} путь");
        _decisionMade = true;
    }

    private void Cleanup()
    {
        _input.OnHorizontalChanged -= OnHorizontalInput;

        if (_activeTimerCoroutine != null)
            _coroutineRunner.StopCoroutine(_activeTimerCoroutine);

        if (_decisionCoroutine != null)
        {
            _coroutineRunner.StopCoroutine(_decisionCoroutine);
            _decisionCoroutine = null;
        }
    }
}
