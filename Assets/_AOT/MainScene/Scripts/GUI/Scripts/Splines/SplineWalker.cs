using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    [ExecuteInEditMode]
    public class SplineWalker : MonoBehaviour 
    {
        public BezierSpline spline;
        public float position;
        public enum PositionMode
        {
            Local,
            World
        };
        public PositionMode positionMode = PositionMode.World;

        protected Coroutine coroutine;

        private void Update()
        {
            if (spline == null)
                return;

            UpdateWalker(GetClampedProgress());
        }

        protected virtual float GetClampedProgress()
        {
            float progress = position - (int)position;
            if (spline.Loop)
            {
                if (progress < 0f)
                    progress += 1f;
                else if (progress > 1f)
                    progress -= 1f;
            }
            else 
            {
                progress = Mathf.Clamp01(progress);
            }
            return progress;
        }

        protected virtual void UpdateWalker(float progress)
        {
            Vector3 point = spline.GetPoint(progress);
            if (positionMode == PositionMode.Local)
                transform.localPosition = point;
            else if (positionMode == PositionMode.World)
                transform.position = point;
            transform.LookAt(point + spline.GetDirection(progress));
        }

        public void MoveTo(float target, float velocity, int accuracy)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);

            coroutine = StartCoroutine(MoveToCoroutine(Mathf.Clamp01(target), velocity, accuracy));
        }

        protected IEnumerator MoveToCoroutine(float target, float velocity, int accuracy)
        {
            position = GetClampedProgress();
            float displacement = target - position;
            if (velocity > 0f && position > target)
                displacement = 1f - displacement;
            else if (velocity < 0f && position < target)
                displacement -= 1f;

            while (true)
            {
                float newPosition = spline.MoveAlong(position, velocity * Time.deltaTime, accuracy);
                float delta = newPosition - position;
                displacement -= delta;
                if (displacement * velocity > 0f)
                {
                    position = newPosition;
                    position = GetClampedProgress();
                    yield return null;
                }
                else
                {
                    position = target;
                    yield break;
                }
            }
        }
    }
}
