using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    /// http://digitalopus.ca/site/pd-controllers/
    public static class PIDUtils
    {
        public static void CalcCoefficient(float frequency, float damping, float dt, out float ksg, out float kdg)
        {
            float ks = (6f * frequency) * (6f * frequency) * 0.25f;
            float kd = 4.5f * frequency * damping;
    	    float g = 1f / (1f + kd * dt + ks * dt * dt);
    	    ksg = ks * g;
    	    kdg = (kd + ks * dt) * g;
        }

        public static float CalcForce(float position, float desiredPosition, float velocity, float desiredVelocity, float ksg, float kdg)
        {
            return (desiredPosition - position) * ksg + (desiredVelocity - velocity) * kdg;
        }

        public static Vector2 CalcForce(Vector2 position, Vector2 desiredPosition, Vector2 velocity, Vector2 desiredVelocity, float ksg, float kdg)
        {
            return (desiredPosition - position) * ksg + (desiredVelocity - velocity) * kdg;
        }

        public static Vector3 CalcForce(Vector3 position, Vector3 desiredPosition, Vector3 velocity, Vector3 desiredVelocity, float ksg, float kdg)
        {
            return (desiredPosition - position) * ksg + (desiredVelocity - velocity) * kdg;
        }

        public static Vector4 CalcForce(Vector4 position, Vector4 desiredPosition, Vector4 velocity, Vector4 desiredVelocity, float ksg, float kdg)
        {
            return (desiredPosition - position) * ksg + (desiredVelocity - velocity) * kdg;
        }

        public static Color CalcForce(Color color, Color desiredColor, Color velocity, Color desiredVelocity, float ksg, float kdg)
        {
            return (desiredColor - color) * ksg + (desiredVelocity - velocity) * kdg;
        }

        public static float CalcDisplacement(float position, float desiredPosition, ref float velocity, float desiredVelocity, float frequency, float damping, float dt)
        {
            float ksg, kdg;
            CalcCoefficient(frequency, damping, dt, out ksg, out kdg);

            float force = CalcForce(position, desiredPosition, velocity, desiredVelocity, ksg, kdg);
            velocity += force * dt;

            return velocity * dt;
        }

        public static Vector2 CalcDisplacement(Vector2 position, Vector2 desiredPosition, ref Vector2 velocity, Vector2 desiredVelocity, float frequency, float damping, float dt)
        {
            float ksg, kdg;
            CalcCoefficient(frequency, damping, dt, out ksg, out kdg);

            Vector2 force = CalcForce(position, desiredPosition, velocity, desiredVelocity, ksg, kdg);
            velocity += force * dt;

            return velocity * dt;
        }

        public static Vector3 CalcDisplacement(Vector3 position, Vector3 desiredPosition, ref Vector3 velocity, Vector3 desiredVelocity, float frequency, float damping, float dt)
        {
            float ksg, kdg;
            CalcCoefficient(frequency, damping, dt, out ksg, out kdg);

            Vector3 force = CalcForce(position, desiredPosition, velocity, desiredVelocity, ksg, kdg);
            velocity += force * dt;

            return velocity * dt;
        }

        public static Vector4 CalcDisplacement(Vector4 position, Vector4 desiredPosition, ref Vector4 velocity, Vector4 desiredVelocity, float frequency, float damping, float dt)
        {
            float ksg, kdg;
            CalcCoefficient(frequency, damping, dt, out ksg, out kdg);

            Vector4 force = CalcForce(position, desiredPosition, velocity, desiredVelocity, ksg, kdg);
            velocity += force * dt;

            return velocity * dt;
        }

        public static Color CalcDisplacement(Color color, Color desiredColor, ref Color velocity, Color desiredVelocity, float frequency, float damping, float dt)
        {
            float ksg, kdg;
            CalcCoefficient(frequency, damping, dt, out ksg, out kdg);

            Color force = CalcForce(color, desiredColor, velocity, desiredVelocity, ksg, kdg);
            velocity += force * dt;

            return velocity * dt;
        }
    }
}
