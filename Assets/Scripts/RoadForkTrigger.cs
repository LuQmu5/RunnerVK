using TMPro;
using UnityEngine;

public class RoadForkTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _forkUI;
    [SerializeField] private TMP_Text _instructionText;
    [SerializeField] private float _decisionTime = 0.5f;
    [SerializeField] private Road _leftRoad;
    [SerializeField] private Road _rightRoad;

    [SerializeField] private Transform _cameraPivotPoint;

    private bool _triggered = false;
    private ForkData _data;

    private void Awake()
    {
        _data = new ForkData(_cameraPivotPoint);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered)
            return;
    }
}