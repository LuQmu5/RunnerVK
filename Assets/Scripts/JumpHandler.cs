using UnityEngine;
using DG.Tweening;

public class JumpHandler
{
    private readonly Transform _transform;
    private readonly JumpSettings _settings;
    private Tween _jumpTween;

    public bool IsJumping { get; private set; }

    public JumpHandler(Transform transform, JumpSettings settings)
    {
        _transform = transform;
        _settings = settings;
    }

    public bool TryJump()
    {
        if (IsJumping) return false;

        DoJump();
        return true;
    }

    private void DoJump()
    {
        IsJumping = true;

        Vector3 startPos = _transform.position;
        float duration = _settings.totalDuration;
        float apex = _settings.jumpHeight;
        float forwardSpeed = _settings.forwardSpeed;

        Sequence seq = DOTween.Sequence();

        seq.AppendInterval(duration * 0.18f);

        seq.Append(_transform.DOMoveY(startPos.y + apex, duration * 0.12f)
            .SetEase(Ease.OutQuad));

        seq.AppendInterval(duration * 0.20f);

        seq.Append(_transform.DOMoveY(startPos.y, duration * 0.10f)
            .SetEase(Ease.InQuad));

        seq.AppendInterval(duration * 0.40f);

        seq.Insert(duration * 0.18f, _transform.DOMoveZ(startPos.z + _settings.forwardSpeed, duration * 0.18f)
            .SetEase(Ease.Linear));

        seq.OnComplete(() => IsJumping = false);


        _jumpTween = seq;
    }
}
