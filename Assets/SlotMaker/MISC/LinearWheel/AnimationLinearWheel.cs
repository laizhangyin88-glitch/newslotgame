using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SlotMaker
{
    public class AnimationLinearWheel : MonoBehaviour
    {
        public AnimationCurve curve = new AnimationCurve(
            new Keyframe(0.0f, 0.0f, 0.0f, 25.0f), 
            new Keyframe(0.04f, 1.0f, 25.0f, -1.041666f), 
            new Keyframe(1.0f, 0.0f, -1.041666f, 0f)
        );
        public float animationTime = 5.0f;
        public bool upward = true;
        public float direction { get { return upward ? 1f : -1f; } }

        public float desiredDistance;
        public float currentVelocity;
        public int lastSegmentIndex
        {
            get; private set;
        }

        public UnityEvent onSpinLinearWheel;
        public UnityEvent onStoppedLinearWheel;
        public UnityIntEvent onChangedSegment;

        public List<LinearWheelSegment> segmentList;
        protected float totalWheelLength { get; private set; }
        protected float wheelTopBorder { get; private set; }
        protected float wheelBottomBorder { get; private set; }

        public float wheelPointerPosition;

        // This wheel will spin more than minimumSpinCount times and less than (minimumSpinCount + 1) times
        public float minimumSpinCount;
        private int precisionStep;

        private float simulationTime;

        private float velocityMultiplier;
        private bool simulation = false;
        
        public void Awake()
        {
            UpdateWheelRectInfo();
        }

        public void UpdateWheelRectInfo()
        {
            totalWheelLength = 0f;
            wheelTopBorder = float.NegativeInfinity;
            wheelBottomBorder = float.PositiveInfinity;

            foreach (LinearWheelSegment segment in segmentList)
            {
                segment.UpdateRectInfo();

                totalWheelLength += segment.size;
                if (segment.upperBoundPosition > wheelTopBorder)
                    wheelTopBorder = segment.upperBoundPosition;
                if (segment.lowerBoundPosition < wheelBottomBorder)
                    wheelBottomBorder = segment.lowerBoundPosition;
            }
        }

        public void Simulation()
        {
            SetWheelMovement();
            simulation = true;
            onSpinLinearWheel.Invoke();
        }

        protected void SetSimulationState(bool isActive)
        {
            simulation = isActive;
        }
        
        protected void SetWheelMovement()
        {
            // Integration result is the most precise when precisionStep is 50 per second.
            // This is because the fixed timestep is 0.02 second.
            precisionStep = Mathf.RoundToInt(50.0f * animationTime);

            float result = IntegrateAnimationCurve(curve, 0.0f, 1.0f);
            float timeScaledResult = result * animationTime;

            velocityMultiplier = desiredDistance / timeScaledResult;
            simulationTime = 0f;
            
            lastUpdateTime = Time.time;
            timeToNextFixedUpdate = float.PositiveInfinity;
            lastSegmentIndex = GetCurrentSegment();
        }
        
        private int fixedCount;
        private float lastUpdateTime;
        private float timeToNextFixedUpdate;
        private float nextDisplacement;
        private void FixedUpdate()
        {
            CustomFixedUpdate();
        }

        protected virtual void CustomFixedUpdate()
        {
            if (simulation)
            {
                lastUpdateTime = Time.time;
                timeToNextFixedUpdate = Time.fixedDeltaTime;

                if (++fixedCount > 2)
                    return;

                if (nextDisplacement != 0.0f)
                {
                    MoveWheelSegments(nextDisplacement);
                    nextDisplacement = 0.0f;
                }

                if (simulationTime < animationTime)
                {
                    currentVelocity = velocityMultiplier * curve.Evaluate(simulationTime / animationTime);
                    nextDisplacement = direction * currentVelocity * Time.fixedDeltaTime;
                    
                    simulationTime += Time.fixedDeltaTime;
                }
                else
                {
                    StopWheel();
                }
            }
        }

        private void Update()
        {
            CustomUpdate();
        }

        protected virtual void CustomUpdate()
        {
            if (simulation)
            {
                fixedCount = 0;

                float elapsedTime = Time.time - lastUpdateTime;
                float interpolatedDisplacement = (elapsedTime / timeToNextFixedUpdate) * nextDisplacement;
                MoveWheelSegments(interpolatedDisplacement);
                UpdateCurrentSegment();
                nextDisplacement -= interpolatedDisplacement;
                timeToNextFixedUpdate -= elapsedTime;

                // For removing floating point error when nextDisplacement should be zero.
                if (Mathf.Approximately(0.0f, nextDisplacement))
                {
                    nextDisplacement = 0.0f;
                }

                lastUpdateTime = Time.time;
            }
        }

        protected virtual void StopWheel()
        {
            currentVelocity = 0.0f;
            simulation = false;
            onStoppedLinearWheel.Invoke();
        }
        
        protected virtual void MoveWheelSegments(float displacement) 
        {
            float boundPosition = (upward) ? wheelTopBorder : wheelBottomBorder;
            foreach (LinearWheelSegment segment in segmentList)
            {
                segment.MoveWheelSegment(displacement);

                if (segment.CheckOutOfBound(boundPosition, upward))
                {
                    segment.MoveWheelSegment(-1.0f * direction * totalWheelLength);
                }
            }
        }

        public void SetDesiredSegment(int segmentIndex)
        {
            float minimumDisatnce = (minimumSpinCount > 0f) ? minimumSpinCount * totalWheelLength : 0f;
            
            float segmentPos = segmentList[segmentIndex].position;
            float targetDistance = direction * (wheelPointerPosition - segmentPos);

            while (targetDistance < minimumDisatnce)
            {
                targetDistance += totalWheelLength;
            }

            desiredDistance = targetDistance;
        }

        // Calculate the Right Riemman Sum for approximating the definitive integral
        private float IntegrateAnimationCurve(AnimationCurve curve, float startTime, float endTime)
        {
            float result = 0f;
            float stepSize = (endTime - startTime) / Convert.ToSingle(precisionStep);

            float currTime = startTime;
            for (int i = 0; i < precisionStep; i++) 
            {
                currTime += stepSize;
                result += curve.Evaluate(currTime);
            }
            result *= stepSize;
            return result;
        }

        private void UpdateCurrentSegment()
        {
            int currentSegmentIndex = GetCurrentSegment();
            if (currentSegmentIndex != lastSegmentIndex)
            {
                onChangedSegment.Invoke(currentSegmentIndex);
                lastSegmentIndex = currentSegmentIndex;
            }
        }

        private int GetCurrentSegment()
        {
            int currentSegmentIndex = -1;
            for (int i = 0; i < segmentList.Count; i++)
            {
                var segment = segmentList[i];
                if (segment.CheckWheelPointer(wheelPointerPosition))
                {
                    currentSegmentIndex = i;
                }
            }

            return (currentSegmentIndex >= 0) ? currentSegmentIndex : lastSegmentIndex;
        }

        public GameObject GetSegmentGameObject(int segmentIndex)
        {
            return segmentList[segmentIndex].gameObject;
        }

        public virtual GameObject GetSegmentGameObject(float position)
        {
            return null;
        }
        
        public void ResetWheel(bool resetPosition = true, bool resetSelection = true, bool invokeOnResetWheel = true)
        {
            foreach (LinearWheelSegment segment in segmentList)
            {
                if (resetSelection)
                    segment.ResetSegmentSelectionState();
                if (resetPosition)
                    segment.ResetToInitialPosition();
                if (invokeOnResetWheel)
                    segment.OnResetWheel();
            }
        }
    }
}
