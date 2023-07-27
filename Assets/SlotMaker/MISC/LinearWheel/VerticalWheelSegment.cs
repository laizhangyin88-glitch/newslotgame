using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace SlotMaker
{

public class VerticalWheelSegment : LinearWheelSegment
{
    public float height;
    public float centerPosition;
    public float topPosition;
    public float bottomPosition;

    public override float size { get { return height; } }
    public override float position { get { return centerPosition; } }
    public override float upperBoundPosition { get { return topPosition; } }
    public override float lowerBoundPosition { get { return bottomPosition; } }

    private RectTransform _rectTransform;
    public RectTransform rectTransform
    {
        get
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();
            return _rectTransform;
        }
    }

    private Vector2 initialPosition = Vector2.negativeInfinity;
    public void Awake()
    {
        if (initialPosition.Equals(Vector2.negativeInfinity))
            initialPosition = rectTransform.anchoredPosition;
    }

    public override void UpdateRectInfo()
    {
        height = rectTransform.rect.height;
        centerPosition = rectTransform.anchoredPosition.y;
        topPosition = centerPosition + (height / 2.0f);
        bottomPosition = centerPosition - (height / 2.0f);
    }

    public override void MoveWheelSegment(float displacement, LinearWheelSegment prevSegment = null)
    {
        float nextPosition = centerPosition + displacement;
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, nextPosition);
        UpdateRectInfo();
    }

    public override bool CheckOutOfBound(float boundPosition, bool isMovingUpward)
    {
        return (isMovingUpward) ? (centerPosition > boundPosition) : (centerPosition < boundPosition);
    }

    private bool isSelected = false;
    public override bool CheckWheelPointer(float wheelPointerPosition)
    {
        bool isInSegment = (wheelPointerPosition >= bottomPosition) && (wheelPointerPosition <= topPosition);

        if (!isSelected && isInSegment)
        {
            isSelected = true;
            onEnterSelection.Invoke();
        } 
        else if (isSelected && !isInSegment)
        {
            isSelected = false;
            onExitSelection.Invoke();
        }

        return isInSegment;
    }

    public override void ResetSegmentSelectionState()
    {
        if (isSelected)
            onExitSelection.Invoke();
        isSelected = false;
    }

    public override void ResetToInitialPosition()
    {
        rectTransform.anchoredPosition = initialPosition;
        UpdateRectInfo();
    }

    public override void OnResetWheel()
    {
        onResetWheel.Invoke();
    }
}

}
