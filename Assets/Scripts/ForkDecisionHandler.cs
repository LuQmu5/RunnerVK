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
    private readonly PlayerController _player;
    private readonly Road _leftRoad;
    private readonly Road _rightRoad;

    private Coroutine _decisionCoroutine;
    private Coroutine _activeTimerCoroutine;
    private Coroutine _rotateCoroutine;
    private float _currentInput;
    private bool _decisionMade;

    public ForkDecisionHandler(
        IPlayerInput input,
        MonoBehaviour coroutineRunner,
        float decisionTime,
        GameObject forkUI,
        TMP_Text instructionText,
        PlayerController player,
        Road leftRoad,
        Road rightRoad)
    {
        _input = input;
        _coroutineRunner = coroutineRunner;
        _decisionTime = decisionTime;
        _forkUI = forkUI;
        _instructionText = instructionText;
        _player = player;
        _leftRoad = leftRoad;
        _rightRoad = rightRoad;
    }

    public void StartDecision(Func<bool> isPlayerDead)
    {
        if (_decisionCoroutine != null)
            return;

        _decisionCoroutine = _coroutineRunner.StartCoroutine(DecisionRoutine(isPlayerDead));
    }

    private IEnumerator DecisionRoutine(Func<bool> isPlayerDead)
    {
        _decisionMade = false;
        Time.timeScale = 0.4f;

        _forkUI.SetActive(true);
        _instructionText.text = Application.isMobilePlatform
            ? "Свайпните влево или вправо, чтобы выбрать путь"
            : "Удерживайте A (влево) или D (вправо), чтобы выбрать путь";

        _input.OnHorizontalChanged += OnHorizontalInput;

        float timer = _decisionTime / Time.timeScale;

        while (!_decisionMade)
        {
            timer -= Time.deltaTime;

            if (isPlayerDead() || timer <= 0)
            {
                break;
            }

            yield return null;
        }

        Time.timeScale = 1f;

        Cleanup();
        _forkUI.SetActive(false);
    }

    private void OnHorizontalInput(float value)
    {
        if (Mathf.Approximately(value, _currentInput))
            return;

        _currentInput = value;

        if (_activeTimerCoroutine != null)
            _coroutineRunner.StopCoroutine(_activeTimerCoroutine);
        if (_rotateCoroutine != null)
            _coroutineRunner.StopCoroutine(_rotateCoroutine);

        if (value < -0.1f)
        {
            _activeTimerCoroutine = _coroutineRunner.StartCoroutine(DecisionTimer(_leftRoad));
            _rotateCoroutine = _coroutineRunner.StartCoroutine(RotateTowards(_leftRoad.transform));
        }
        else if (value > 0.1f)
        {
            _activeTimerCoroutine = _coroutineRunner.StartCoroutine(DecisionTimer(_rightRoad));
            _rotateCoroutine = _coroutineRunner.StartCoroutine(RotateTowards(_rightRoad.transform));
        }
    }

    private IEnumerator RotateTowards(Transform targetTransform)
    {
        Quaternion startRot = _player.transform.rotation;
        Quaternion targetRot = Quaternion.LookRotation(targetTransform.forward, Vector3.up);

        float elapsed = 0f;

        while (elapsed < _decisionTime)
        {
            if (_decisionMade || Mathf.Abs(_currentInput) < 0.1f)
                yield break;

            float t = elapsed / _decisionTime;
            _player.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        _player.transform.rotation = targetRot;
    }

    private IEnumerator DecisionTimer(Road selectedRoad)
    {
        float elapsed = 0f;

        while (elapsed < _decisionTime)
        {
            if (Mathf.Abs(_currentInput) < 0.1f)
                yield break;

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        _player.SetRoad(selectedRoad);
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
