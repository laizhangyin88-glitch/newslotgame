using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public class AnimationBigWheel : MonoBehaviour
    {
        public AnimationCurve curve;
        public float animationTime;
        public float animationAngle;
        public bool clockwise = true;
        public float direction { get { return clockwise ? -1f : 1f; } }

        public float desiredAngle;

        private Rigidbody rigidbody3d;
        protected Rigidbody Rigidbody3d
        {
            get
            {
                if (rigidbody3d == null)
                    rigidbody3d = GetComponent<Rigidbody>();
                return rigidbody3d;
            }
        }

        private float simulationTime;
        private float targetAngle;
        private float simulationAngle;

        [ContextMenu("Simulation")]
        public void Simulation()
        {
            Quaternion desiredRotation = Quaternion.Euler(0f, 0f, desiredAngle);
            Quaternion localRotation = GetLocalRotation();

            float angle, axis;
            CalcAngleAxis(localRotation, desiredRotation, out angle, out axis);
            angle *= direction;
            if ((axis * direction) < 0f)
                angle = 360f * direction - angle;
            targetAngle = 360f * animationAngle * direction + angle;

            simulationTime = 0f;
            simulationAngle = 0f;
        }

        private void FixedUpdate()
        {
            if (simulationTime < animationTime)
            {
                float evaluate = curve.Evaluate(simulationTime / animationTime);
                Quaternion deltaRotation = Quaternion.Euler(Vector3.forward * targetAngle * (evaluate - simulationAngle));

                Quaternion localRotation = GetLocalRotation();
                Rigidbody3d.MoveRotation(localRotation * deltaRotation);

                simulationAngle = evaluate;
                simulationTime += Time.fixedDeltaTime;
            }
        }

        protected void CalcAngleAxis(Quaternion oldRotation, Quaternion newRotation, out float angle, out float axis)
        {
            Vector3 oldPoint = oldRotation * Vector3.up;
            Vector3 newPoint = newRotation * Vector3.up;
            angle = Vector3.Angle(oldPoint, newPoint);
            axis = Vector3.Cross(oldPoint.normalized, newPoint.normalized).z;
        }

        protected Quaternion GetLocalRotation()
        {
            return Quaternion.Inverse(transform.parent.rotation) * Rigidbody3d.rotation;
        }
    }
}
