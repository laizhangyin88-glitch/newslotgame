using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SlotMaker
{
    public class PBBigWheel : MonoBehaviour
    {
        public int target;

        public bool asleep = true;
        public float simulationTime;

        public Vector3 axis = Vector3.forward;
        public float spinForce = 10f;
        public float spinTime = 0.1f;
        public float damping = 0.1f;
        public float drag = 0.1f;
        public float sleepThreshold = 0.1f;
        public float sleepTime = 0.1f;
        public int segmentCount = 6;
        [Range(0.0001f, 0.9999f)]
        public float safeFactor = 0.5f;

        public int maximumSimulationCount = 10000;
        public float simulationAccuracy = 0.1f;
        public float[] simulationTable;

        public float angularVelocityMag;

        public UnityEvent onSpin;
        public UnityEvent onStopped;

        Vector3 angularVelocity;
        Vector3 displacement;
        float spinningForce;
        float sleepingTime;
        float spinningTime;
        float segmentAngle;

        [ContextMenu("Test")]
        public void Test()
        {
            spinningForce = spinForce;
            angularVelocity = Vector3.zero;
            displacement = Vector3.zero;
            spinningTime = spinTime;
            sleepingTime = 0f;

            asleep = false;
            simulationTime = 0f;
        }

        [ContextMenu("Approximate")]
        void Approximate()
        {
            if (damping == 0f || drag == 0f || sleepThreshold == 0f)
            {
                Debug.LogWarning("damping, drag, sleepThreshold could not be zero!!");
                return;
            }

            simulationTable = new float[segmentCount + 2];// For continuous circling forces
            segmentAngle = 360f / segmentCount;

            // Find pivot
            float simulationSpinForce = spinForce;
            for (int simulationCount = 0; simulationCount < maximumSimulationCount; ++simulationCount)
            {
                int found = Simulate(simulationSpinForce);
                if (found == (segmentCount - 1))
                    break;

                simulationSpinForce += simulationAccuracy;
            }

            // Find first
            for (int simulationCount = 0; simulationCount < maximumSimulationCount; ++simulationCount)
            {
                int found = Simulate(simulationSpinForce);
                if (found == 0)
                {
                    simulationTable[0] = simulationSpinForce;
                    break;
                }

                simulationSpinForce += simulationAccuracy;
            }

            // Find others
            for (int simulationCount = 0; simulationCount < maximumSimulationCount; ++simulationCount)
            {
                simulationSpinForce += simulationAccuracy;

                int found = Simulate(simulationSpinForce);
                if (found < 2 && simulationTable[segmentCount - 1] != 0f)
                    found += segmentCount;

                if (simulationTable[found] == 0f)
                {
                    simulationTable[found] = simulationSpinForce;

                    if (found > segmentCount)
                        break;
                }
            }
        }

        [ContextMenu("Spin")]
        public void Spin()
        {
            if (target < 0 || target >= segmentCount)
                return;

            segmentAngle = 360f / segmentCount;
            float minimumAngle = segmentAngle * (1f - safeFactor) * 0.5f;
            float maximumAngle = segmentAngle - minimumAngle;

            float currentAngle = GetAngle();
            int current = (int)(currentAngle / segmentAngle);
            int realTarget = target - current;
            if (realTarget < 0)
                realTarget += segmentCount;
            float offset = currentAngle - segmentAngle * current;

            float minimumSpinForce, maximumSpinForce;
            if (realTarget > 0)
            {
                minimumSpinForce = simulationTable[realTarget - 1];
                maximumSpinForce = simulationTable[realTarget + 1];
            }
            else
            {
                minimumSpinForce = simulationTable[segmentCount - 1];
                maximumSpinForce = simulationTable[segmentCount + 1];
            }

            for (int simulationCount = 0; simulationCount < maximumSimulationCount; ++simulationCount)
            {
                spinningForce = UnityEngine.Random.Range(minimumSpinForce, maximumSpinForce);

                float angleDistance;
                int found = Simulate(spinningForce, offset, out angleDistance);

                int deltaIndex = found - realTarget;
                if (deltaIndex == 0)
                {
                    if (angleDistance < minimumAngle)
                        minimumSpinForce = spinningForce;
                    else if (angleDistance > maximumAngle)
                        maximumSpinForce = spinningForce;
                    else 
                        break;
                }
                else 
                {
                    if (deltaIndex == 1 || deltaIndex == -(segmentCount - 1))
                        maximumSpinForce = spinningForce;
                    else // (deltaIndex == -1 || deltaIndex == (segmentCount - 1))
                        minimumSpinForce = spinningForce;
                }
            }

            angularVelocity = Vector3.zero;
            displacement = Vector3.zero;
            spinningTime = spinTime;
            sleepingTime = 0f;

            asleep = false;
            simulationTime = 0f;

            onSpin.Invoke();
        }

        void FixedUpdate()
        {
            if (!asleep)
            {
                float dt = Time.fixedDeltaTime;
                simulationTime += dt;

                // damping force
                angularVelocity *= 1f - damping * dt;

                // spin force
                if (spinningTime > 0f)
                {
                    angularVelocity += spinningForce * axis * dt;
                    spinningTime -= dt;
                }

                // check sleep
                angularVelocityMag = angularVelocity.magnitude;
                if (angularVelocityMag < sleepThreshold)
                {
                    sleepingTime += dt;
                    if (sleepingTime > sleepTime)
                    {
                        asleep = true;
                        angularVelocityMag = 0f;
                        onStopped.Invoke();
                        return;
                    }
                }
                else 
                {
                    sleepingTime = 0f;
                }

                // drag
                angularVelocity -= Mathf.Min(drag * dt, angularVelocityMag) * axis;

                // displacement
                Vector3 delta = angularVelocity * Mathf.Rad2Deg * dt;
                displacement += delta;

                // rotate
                transform.Rotate(delta);
            }
        }

        int Simulate(float simulationForce)
        {
            float totalDisplacement = PBBigWheelUtils.Simulate(simulationForce, spinTime, damping, drag, sleepThreshold, sleepTime);
            int spinCount = (int)(totalDisplacement / 360f);
            float modularDisplacement = totalDisplacement - 360f * spinCount;
            return (int)(modularDisplacement / segmentAngle);
        }

        int Simulate(float simulationForce, float offset, out float angleDistance)
        {
            float totalDisplacement = PBBigWheelUtils.Simulate(simulationForce, spinTime, damping, drag, sleepThreshold, sleepTime);
            totalDisplacement += offset;
            int spinCount = (int)(totalDisplacement / 360f);
            float modularDisplacement = totalDisplacement - 360f * spinCount;
            int index = (int)(modularDisplacement / segmentAngle);
            angleDistance = modularDisplacement - segmentAngle * index;
            return index;
        }

        float GetAngle()
        {
            float angle;
            Vector3 angleAxis;
            transform.localRotation.ToAngleAxis(out angle, out angleAxis);
            if (Vector3.Dot(axis, angleAxis) < 0f)
                angle = 360f - angle;
            return angle;
        }
    }
}
