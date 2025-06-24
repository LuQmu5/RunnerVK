using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpHandler
{
    private readonly Transform _transform;
    private readonly MonoBehaviour _coroutineRunner;
    private readonly JumpSettings _settings;

    public bool IsJumping { get; private set; } = false;

    public JumpHandler(Transform transform, MonoBehaviour coroutineRunner, JumpSettings settings)
    {
        _transform = transform;
        _coroutineRunner = coroutineRunner;
        _settings = settings;
    }

    public bool TryJump()
    {
        if (IsJumping) 
            return false;

        _coroutineRunner.StartCoroutine(JumpRoutine());
        IsJumping = true;

        return true;
    }

    private IEnumerator JumpRoutine()
    {
        float time = 0f;
        float duration = _settings.totalDuration;
        float apex = _settings.jumpHeight;
        Vector3 startPos = _transform.position;

        while (time < duration)
        {
            float tNorm = time / duration;

            float yOffset = 0f;

            if (tNorm < _settings.jumpStartTime)
            {
                yOffset = 0f;
            }
            else if (tNorm < _settings.apexTime)
            {
                float phase = Mathf.InverseLerp(_settings.jumpStartTime, _settings.apexTime, tNorm);
                yOffset = Mathf.SmoothStep(0, apex, phase);
            }
            else if (tNorm < _settings.apexHoldTime)
            {
                yOffset = apex;
            }
            else if (tNorm < _settings.landTime)
            {
                float phase = Mathf.InverseLerp(_settings.apexHoldTime, _settings.landTime, tNorm);
                yOffset = Mathf.SmoothStep(apex, 0, phase);
            }

            float forwardPhase = Mathf.InverseLerp(_settings.jumpStartTime, _settings.landTime, tNorm);
            float forwardEaseOut = 1f - Mathf.Pow(1f - forwardPhase, 2f);
            float zOffset = _settings.forwardSpeed * forwardEaseOut * duration;

            _transform.position = new Vector3(
                startPos.x,
                startPos.y + yOffset,
                startPos.z + zOffset
            );

            time += Time.deltaTime;
            yield return null;
        }

        _transform.position = new Vector3(
            _transform.position.x,
            startPos.y,
            _transform.position.z
        );

        IsJumping = false;
    }

}
