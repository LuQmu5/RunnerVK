using System.Collections;
using System.Collections.Generic;
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

        _coroutineRunner.StartCoroutine(JumpRoutine());
        _isJumping = true;

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
            else
            {
                yOffset = 0f;
            }

            float zOffset = _settings.forwardSpeed * Mathf.Clamp01(Mathf.InverseLerp(_settings.jumpStartTime, _settings.landTime, tNorm)) * time;

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
            _transform.position.z //startPos.z + _settings.forwardSpeed * (_settings.landTime - _settings.jumpStartTime)
        );

        yield return new WaitForSeconds(_settings.totalDuration - _settings.landTime);

        _isJumping = false;
    }
}
