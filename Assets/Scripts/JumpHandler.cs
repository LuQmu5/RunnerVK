using Palmmedia.ReportGenerator.Core;
using System.Collections;
using UnityEngine;

public class JumpHandler
{
    private readonly Transform _transform;
    private readonly MonoBehaviour _coroutineRunner;
    private readonly JumpSettings _settings;

    private bool _isJumping;

    public JumpHandler(Transform transform, MonoBehaviour coroutineRunner, JumpSettings settings)
    {
        _transform = transform;
        _coroutineRunner = coroutineRunner;
        _settings = settings;
    }

    public bool TryJump()
    {
        if (_isJumping)
            return false;

        // _coroutineRunner.StartCoroutine(JumpRoutine());
        return true;
    }

    private IEnumerator JumpRoutine()
    {
        _isJumping = true;

        float time = 0f;
        Vector3 startPos = _transform.position;

        while (time < _settings.jumpDuration)
        {
            float curveY = _settings.jumpCurve.Evaluate(time / _settings.jumpDuration);
            float y = startPos.y + curveY;
            float z = startPos.z + _settings.forwardSpeed * time;
            _transform.position = new Vector3(startPos.x, y, z);

            time += Time.deltaTime;
            yield return null;
        }

        Vector3 endPos = new Vector3(startPos.x, startPos.y, startPos.z + _settings.forwardSpeed * _settings.jumpDuration);
        _transform.position = endPos;

        _isJumping = false;
    }
}
