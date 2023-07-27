using TMPro;
using UnityEngine;

namespace SlotMaker
{
    [RequireComponent(typeof(Animator))]
    public class ExpandableHorizontalWheelSegment : HorizontalWheelSegment
    {
        public int segmentValue = 0;

        /// <summary>
        /// Contains additional sector value (f.e. multiplier)
        /// </summary>
        public int additionalSegmentValue = 0;

        /// <summary>
        /// Increase factor for sector width/height
        /// </summary>
        public int jackpotSizeValue = 1;

        private float _expandedWidth;

        public override float size
        {
            get
            {
                if (_expandedWidth <= 0f)
                {
                    _expandedWidth = width;
                }

                return _expandedWidth;
            }
        }

        public void Expand(int sizeToExpand)
        {
            jackpotSizeValue = sizeToExpand;
            _expandedWidth = ExpandableAnimationLinearWheel.MinSegmentWidth * jackpotSizeValue;

            rightPosition = centerPosition + (_expandedWidth / 2.0f);
            leftPosition = centerPosition - (_expandedWidth / 2.0f);
        }

        public float GetStopPosition(int positionPart)
        {
            return lowerBoundPosition + ExpandableAnimationLinearWheel.MinSegmentWidth * positionPart;
        }

        public override void MoveWheelSegment(float displacement, LinearWheelSegment prevSegment = null)
        {
            if (prevSegment != null)
            {
                centerPosition = (prevSegment.upperBoundPosition - displacement) + width / 2.0f;
            }

            base.MoveWheelSegment(displacement, prevSegment);
        }

        public override bool CheckOutOfBound(float boundPosition, bool isMovingRightward)
        {
            return (isMovingRightward) ? (upperBoundPosition > boundPosition) : (upperBoundPosition < boundPosition);
        }
        
        public override bool CheckWheelPointer(float wheelPointerPosition)
        {
            return wheelPointerPosition >= leftPosition && wheelPointerPosition <= rightPosition;
        }
    }
}