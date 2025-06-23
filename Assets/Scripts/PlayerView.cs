using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private enum RunDirection { None, Left, Right }
    private RunDirection _lastDirection = RunDirection.None;

    public void PlayRun()
    {
        if (_lastDirection != RunDirection.None)
        {
            _animator.SetTrigger("Run");
            _lastDirection = RunDirection.None;
        }
    }

    public void PlayRunLeft()
    {
        if (_lastDirection != RunDirection.Left)
        {
            _animator.SetTrigger("RunLeft");
            _lastDirection = RunDirection.Left;
        }
    }

    public void PlayRunRight()
    {
        if (_lastDirection != RunDirection.Right)
        {
            _animator.SetTrigger("RunRight");
            _lastDirection = RunDirection.Right;
        }
    }

    public void PlayTakeHit()
    {
        _animator.SetTrigger("TakeHit");
    }

    public void PlayJump()
    {
        _animator.SetTrigger("Jump");
    }
}
