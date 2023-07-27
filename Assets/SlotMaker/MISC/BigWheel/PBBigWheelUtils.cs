using UnityEngine;

namespace SlotMaker
{
    public static class PBBigWheelUtils
    {
        public static float Simulate(float spinForce, float spinTime, float damping, float drag, float sleepThreshold, float sleepTime)
        {
            float angularVelocity = 0f;
            float displacement = 0f;
            float spinningTime = spinTime;
            float sleepingTime = 0f;
            float dt = Time.fixedDeltaTime;

            while (true)
            {
                // damping force
                angularVelocity *= 1f - damping * dt;

                // spin force
                if (spinningTime > 0f)
                {
                    angularVelocity += spinForce * dt;
                    spinningTime -= dt;
                }

                // check sleep
                float angularVelocityMag = Mathf.Abs(angularVelocity);
                if (angularVelocityMag < sleepThreshold)
                {
                    sleepingTime += dt;
                    if (sleepingTime > sleepTime)
                        return displacement;
                }
                else
                {
                    sleepingTime = 0f;
                }

                // drag
                angularVelocity -= Mathf.Min(drag * dt, angularVelocityMag);

                // displacement
                displacement += angularVelocity * Mathf.Rad2Deg * dt;
            }
        }
    }
}
