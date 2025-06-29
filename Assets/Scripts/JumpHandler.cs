using UnityEngine;
using System.Collections;

public class JumpHandler
{
    private readonly MonoBehaviour _actor;
    private readonly JumpSettings _settings;
    private Coroutine _jumpingCoroutine;

    public bool IsJumping => _jumpingCoroutine != null;

    public JumpHandler(MonoBehaviour actor, JumpSettings settings)
    {
        _actor = actor;
        _settings = settings;
    }

    public bool TryJump()
    {
        if (IsJumping) 
            return false;

        if (_jumpingCoroutine != null)
            _actor.StopCoroutine(_jumpingCoroutine);

        _jumpingCoroutine = _actor.StartCoroutine(Jumping());
        
        return true;
    }

    private IEnumerator Jumping()
    {
        float timer = 0f;
        Vector3 startPosition = _actor.transform.position;

        while (timer < _settings.JumpTime)
        {
            float t = timer / _settings.JumpTime;

            float yOffset = _settings.JumpCurveY.Evaluate(t) * _settings.JumpHeight;
            float zOffset = _settings.JumpCurveZ.Evaluate(t) * _settings.JumpLength;

            Vector3 newPosition = startPosition + new Vector3(0f, yOffset, zOffset);

            _actor.transform.position = newPosition;

            timer += Time.deltaTime;
            yield return null;
        }

        _actor.transform.position = startPosition + new Vector3(0f, 0f, _settings.JumpLength);

        _jumpingCoroutine = null;
    }

}
