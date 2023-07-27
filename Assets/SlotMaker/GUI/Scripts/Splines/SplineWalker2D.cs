using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    [ExecuteInEditMode]
    public class SplineWalker2D : SplineWalker
    {
        public bool applyRotation;
        public Animator animator;
        public int segmentCount = 4;

        private static readonly int PROPERTY_ROTATION = Animator.StringToHash("Rotation");

        protected override void UpdateWalker(float progress)
        {
            Vector3 point = spline.GetPoint(progress);
            if (positionMode == PositionMode.Local)
                transform.localPosition = point;
            else if (positionMode == PositionMode.World)
                transform.position = point;
            float angle = Vector3.SignedAngle(Vector3.up, spline.GetDirection(progress), Vector3.forward);
            if (applyRotation)
                transform.eulerAngles = new Vector3(0f, 0f, angle);

            if (animator != null && segmentCount > 0)
            {
                if (angle > 0f)
                    angle = 360f - angle;
                else if (angle < 0f)
                    angle = -angle;
                angle = Mathf.Clamp01(angle / 360f);
                
                float segmentAngle = 1f / segmentCount;
                angle += segmentAngle * 0.5f;
                if (angle > 1f)
                    angle -= 1f;

                animator.SetInteger(PROPERTY_ROTATION, (int)(angle / segmentAngle));
            }
        }
    }
}
