using TMPro;
using UnityEngine;

public class RoadForkTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _forkUI;
    [SerializeField] private TMP_Text _instructionText;
    [SerializeField] private float _decisionTime = 0.5f;

    private ForkDecisionHandler _decisionHandler;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            var input = player.Input; // предполагается, что игроку передан IPlayerInput
            _decisionHandler = new ForkDecisionHandler(
                input,
                this, // MonoBehaviour
                _decisionTime,
                _forkUI,
                _instructionText
            );

            _decisionHandler.StartDecision(
                direction => Debug.Log($"Выбран путь: {direction}"),
                () => player.IsDead
            );
        }
    }
}
