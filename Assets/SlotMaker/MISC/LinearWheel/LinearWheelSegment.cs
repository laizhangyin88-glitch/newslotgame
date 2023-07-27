using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SlotMaker
{
    public abstract class LinearWheelSegment : MonoBehaviour
    {
        public abstract float size { get; }
        public abstract float position { get; }
        public abstract float upperBoundPosition { get; }
        public abstract float lowerBoundPosition { get; }

        public UnityEvent onEnterSelection;
        public UnityEvent onExitSelection;
        public UnityEvent onResetWheel;

        public abstract void UpdateRectInfo();
        public abstract void MoveWheelSegment(float displacement, LinearWheelSegment prevSegment = null);
        public abstract bool CheckOutOfBound(float boundPosition, bool isMovingPositive);
        public abstract bool CheckWheelPointer(float wheelPointerPosition);

        public abstract void ResetSegmentSelectionState();
        public abstract void ResetToInitialPosition();
        public abstract void OnResetWheel();
    }
}