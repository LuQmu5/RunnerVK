using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    public void SetXSpeed(float value)
    {
        if (value < 0)
            value = -1;
        else if (value > 0)
            value = 1;

        _animator.SetFloat("XSpeed", value);
    }

    public void SetJump(bool value)
    {
        _animator.SetBool("IsJumping", value);
    }

    public void PlayHit()
    {
        _animator.SetTrigger("Hit");
    }
}
