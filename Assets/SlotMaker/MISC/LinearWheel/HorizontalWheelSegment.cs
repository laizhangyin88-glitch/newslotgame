using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace SlotMaker
{

public class HorizontalWheelSegment : LinearWheelSegment
{
    public float width;
    public float centerPosition;
    public float rightPosition;
    public float leftPosition;

    public override float size { get { return width; } }
    public override float position { get { return centerPosition; } }
    public override float upperBoundPosition { get { return rightPosition; } }
    public override float lowerBoundPosition { get { return leftPosition; } }

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
        width = rectTransform.rect.width;
        centerPosition = rectTransform.anchoredPosition.x;
        rightPosition = centerPosition + (width / 2.0f);
        leftPosition = centerPosition - (width / 2.0f);
    }

    public override void MoveWheelSegment(float displacement, LinearWheelSegment prevSegment = null)
    {
        float nextPosition = centerPosition + displacement;
        rectTransform.anchoredPosition = new Vector2(nextPosition, rectTransform.anchoredPosition.y);
        UpdateRectInfo();
    }

    public override bool CheckOutOfBound(float boundPosition, bool isMovingRightward)
    {
        return (isMovingRightward) ? (centerPosition > boundPosition) : (centerPosition < boundPosition);
    }

    private bool isSelected = false;
    public override bool CheckWheelPointer(float wheelPointerPosition)
    {
        bool isInSegment = (wheelPointerPosition >= leftPosition) && (wheelPointerPosition <= rightPosition);

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
