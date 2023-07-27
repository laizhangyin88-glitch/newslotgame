using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public class ExpandableAnimationLinearWheel : AnimationLinearWheel
    {
        public float stopAnimationTime = 5.0f;

        public const float MinSegmentWidth = 82f;
        private const float SegmentPositionThreshold = 0.001f;
        private const float SegmentYPosition = -169.1f;
        private const int MaxJackpotSize = 7;
        private const int SegmentValueIncreaseFactor = 2;

        private readonly AnimationCurve _regularSpeedCurve = AnimationCurve.Linear(0f,1f, 1f, 1f);

        [SerializeField] private AnimationCurve _stopCurve;
        [SerializeField] private RectTransform _panelTransform;
        [SerializeField] private ObjectPool _segmentPool;
        [SerializeField] private float _wheelSpeed = 1500f;

        private int _stopSegmentIndex;
        private int _jackpotOffsetPosition;
        private int _currentJackpotSize;

        private bool _isStopping;
        private bool _isLooping;

        private List<int> _segmentValuesList;
        private List<int> _jackpotIndexesList;
        private List<ExpandableHorizontalWheelSegment> _jackpotSegmentList;
        private List<ExpandableHorizontalWheelSegment> _originalSegmentIndexesList;
        private ExpandableHorizontalWheelSegment _targetSegment;

        private float _curvePercent = 1f;
        private float _velocityMultiplier;
        private float _distanceTraveled;

        private Coroutine _spinToTargetSegment;

        public void Initialize(ExpandableWheelData wheelData)
        {
            _segmentValuesList = wheelData.SegmentValuesList;
            _jackpotIndexesList = wheelData.JackpotIndexesList;
            _stopSegmentIndex = wheelData.StopSegmentIndex;
            _jackpotOffsetPosition = wheelData.JackpotOffsetPosition;

            _jackpotSegmentList = new List<ExpandableHorizontalWheelSegment>();
            _originalSegmentIndexesList = new List<ExpandableHorizontalWheelSegment>();

            ExpandableHorizontalWheelSegment prevSegment = null;

            if (segmentList.Count > _segmentValuesList.Count)
            {
                for (int i = 0, count = segmentList.Count - _segmentValuesList.Count; i < count; i++)
                {
                    var pooledObject = segmentList[0].GetComponent<PooledObject>();
                    _segmentPool.AddObject(pooledObject);
                    segmentList.RemoveAt(0);
                }
            }

            for (int i = 0, count = _segmentValuesList.Count; i < count; i++)
            {
                ExpandableHorizontalWheelSegment currentSegment;

                if (i < segmentList.Count)
                {
                    currentSegment = (ExpandableHorizontalWheelSegment) segmentList[i];
                }
                else
                {
                    currentSegment = _segmentPool.GetObject().GetComponent<ExpandableHorizontalWheelSegment>();
                    currentSegment.rectTransform.SetParent(_panelTransform);
                    segmentList.Add(currentSegment);
                }

                SetSegmentPosition(currentSegment.rectTransform, prevSegment);

                currentSegment.additionalSegmentValue = 0;
                currentSegment.segmentValue = _segmentValuesList[i];

                if (_jackpotIndexesList.Contains(i))
                {
                    _jackpotSegmentList.Add(currentSegment);
                }

                _originalSegmentIndexesList.Add(currentSegment);
                prevSegment = currentSegment;
                currentSegment.Expand(1);
            }

            _currentJackpotSize = 0;

            UpdateWheelRectInfo();
            StartSpin();
        }

        public void StartSpin()
        {
            _isLooping = true;
            _isStopping = false;

            SetWheelPath(_wheelSpeed, _regularSpeedCurve);
            Simulation();
        }

        public void StopSpin()
        {
            if (_isLooping)
            {
                _isLooping = false;
                SetSimulationState(false);

                if (_spinToTargetSegment != null)
                {
                    StopCoroutine(_spinToTargetSegment);
                    _spinToTargetSegment = null;
                }

                _spinToTargetSegment = StartCoroutine(SpinToTargetSector());
            }
        }

        public void ExpandJackpotWidth(bool shouldReset = false)
        {
            if (_currentJackpotSize < MaxJackpotSize)
            {
                _currentJackpotSize++;

                for (int i = 0, count = _jackpotSegmentList.Count; i < count; i++)
                {
                    _jackpotSegmentList[i].Expand(shouldReset ? 1 : _currentJackpotSize + 1);
                }

                UpdateWheelRectInfo();
            }
        }

        public void IncreaseSectorValues()
        {
            for (int i = 0, count = segmentList.Count; i < count; i++)
            {
                var currentSegment = segmentList[i] as ExpandableHorizontalWheelSegment;

                if (currentSegment != null)
                {
                    currentSegment.additionalSegmentValue += SegmentValueIncreaseFactor;
                }
            }
        }

        public override GameObject GetSegmentGameObject(float position)
        {
            for (int i = 0, count = segmentList.Count; i < count; i++)
            {
                var currentSegment = (ExpandableHorizontalWheelSegment) segmentList[i];

                if (currentSegment.CheckWheelPointer(position))
                {
                    return currentSegment.gameObject;
                }
            }

            return null;
        }

        public void ActivateFeature(ExpandableWheelFeatureType featureType)
        {
            switch (featureType)
            {
                case ExpandableWheelFeatureType.ExpandJackpot:
                    ExpandJackpotWidth();
                    break;
                case ExpandableWheelFeatureType.IncreaseValue:
                    IncreaseSectorValues();
                    break;
            }
        }

        protected override void MoveWheelSegments(float displacement)
        {
            float boundPosition = (upward) ? wheelTopBorder : wheelBottomBorder;
            int elementOutOfBoundCount = 0;

            for (int index = 0, count = segmentList.Count; index < count; index++)
            {
                var currentSegment = segmentList[index];
                var prevIndex = index - 1;
                var prevSegment = prevIndex >= 0 ? segmentList[prevIndex] : null;

                currentSegment.MoveWheelSegment(displacement, prevSegment);

                if (currentSegment.CheckOutOfBound(boundPosition, upward))
                {
                    elementOutOfBoundCount++;
                }
            }

            for (int i = 0; i < elementOutOfBoundCount; i++)
            {
                LinearWheelSegment borderElement;

                if (upward)
                {
                    borderElement = segmentList[segmentList.Count - 1];
                    segmentList.RemoveAt(segmentList.Count - 1);
                    segmentList.Insert(0, borderElement);
                }
                else
                {
                    borderElement = segmentList[0];
                    segmentList.RemoveAt(0);
                    segmentList.Add(borderElement);
                }

                borderElement.MoveWheelSegment(-1.0f * direction * totalWheelLength);
            }
        }

        protected override void CustomUpdate()
        {
            if (_isStopping && _curvePercent > SegmentPositionThreshold)
            {
                var clampedSpeed = Mathf.Clamp01(_distanceTraveled / desiredDistance);
                _curvePercent = curve.Evaluate(clampedSpeed);

                var remainingDistance = desiredDistance - _distanceTraveled;
                var displacement = direction * _velocityMultiplier * _curvePercent * Time.deltaTime;

                displacement = remainingDistance < Mathf.Abs(displacement)
                    ? direction * remainingDistance
                    : displacement;

                MoveWheelSegments(displacement);
                _distanceTraveled += Mathf.Abs(displacement);

                if (_curvePercent <= SegmentPositionThreshold)
                {
                    _isStopping = false;
                    base.StopWheel();
                }
            }
            else
            {
                base.CustomUpdate();
            }
        }

        protected override void StopWheel()
        {
            if (_isLooping)
            {
                SetWheelMovement();
            }
        }

        private IEnumerator SpinToTargetSector()
        {
            yield return new WaitForEndOfFrame();

            _targetSegment = _originalSegmentIndexesList[_stopSegmentIndex];
            var stopPosition = _targetSegment.GetStopPosition(_jackpotOffsetPosition);
            float targetDistance = (-direction) * (stopPosition - 0f) + 2f * totalWheelLength;

            SetWheelPath(targetDistance, _stopCurve);
            _velocityMultiplier = desiredDistance / stopAnimationTime;
            _distanceTraveled = 0f;
            _curvePercent = 1f;
            _isStopping = true;
        }

        private void SetWheelPath(float distance, AnimationCurve motionCurve)
        {
            desiredDistance = distance;
            curve = motionCurve;
        }

        private void SetSegmentPosition(RectTransform currentSegmentRect, HorizontalWheelSegment previousSegment)
        {
            if (previousSegment != null)
            {
                var prevSegmentPosition = previousSegment.rectTransform.anchoredPosition;
                currentSegmentRect.anchoredPosition =
                    new Vector2(prevSegmentPosition.x + MinSegmentWidth, prevSegmentPosition.y);
            }
            else
            {
                currentSegmentRect.anchoredPosition = new Vector2(MinSegmentWidth / 2f, SegmentYPosition);
            }
        }
    }
}
