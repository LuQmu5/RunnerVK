using UnityEngine;

public class ForkData
{
    private Transform _cameraPivotPoint;

    public Transform CameraLookPoint => _cameraPivotPoint;

    public ForkData(Transform cameraPivotPoint)
    {
        _cameraPivotPoint = cameraPivotPoint;
    }
}