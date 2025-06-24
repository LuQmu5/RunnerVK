using TMPro;
using UnityEngine;

public class RoadForkTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _forkUI;
    [SerializeField] private TMP_Text _instructionText;
    [SerializeField] private float _decisionTime = 0.5f;
    [SerializeField] private Road _leftRoad;
    [SerializeField] private Road _rightRoad;

    private bool _triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered)
            return;

        if (other.TryGetComponent(out PlayerController player))
        {
            _triggered = true;

            var handler = new ForkDecisionHandler(
                player.Input,
                this,
                _decisionTime,
                _forkUI,
                _instructionText,
                player,
                _leftRoad,
                _rightRoad
            );

            handler.StartDecision(
                () => player.IsDead
            );
        }
    }
}