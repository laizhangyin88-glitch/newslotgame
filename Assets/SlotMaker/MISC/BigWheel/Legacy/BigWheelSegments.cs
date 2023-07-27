using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SlotMaker
{

public class BigWheelSegments : MonoBehaviour
{
    public int segmentCount;
    public bool reverse;
    private int lastSegment;
    public float anchorAngle;
    public Vector3 rotationAxis = Vector3.forward;

    protected bool simulation;

    public UnityIntEvent onChangedSegment;

    public void OnSpinBigWheel()
    {
        lastSegment = CalcCurrentSegment();
        simulation = true;
    }

    public void OnStoppedBigWheel()
    {
        simulation = false;
    }

    private int CalcCurrentSegment()
    {
        float segmentAngle = 360f / segmentCount;
        float currentAngle;
        Vector3 currentAxis;
        transform.localRotation.ToAngleAxis(out currentAngle, out currentAxis);
        if (Vector3.Dot(rotationAxis, currentAxis) < 0) {
          // If localRotation's axis of axis-angle representation is negative direction of given rotation axis
          // (this can happen because range of angle in axis-angle is [0, pi])
          currentAngle = 360f - currentAngle;
        }

        // Arrangement segment space
        currentAngle += segmentAngle * 0.5f;
        if (currentAngle >= 360f)
            currentAngle -= 360f;

        // Local to anchor space
        currentAngle += anchorAngle;
        if (currentAngle >= 360f)
            currentAngle -= 360f;

        int ret = (int)((currentAngle / 360f) * segmentCount);
        if (reverse && ret != 0)
            ret = segmentCount - ret;

        return ret;
    }

    private void Update()
    {
        if (simulation)
        {
            int currentSegment = CalcCurrentSegment();
            if (currentSegment != lastSegment)
            {
                onChangedSegment.Invoke(currentSegment);
                lastSegment = currentSegment;
            }
        }
    }
}

}
