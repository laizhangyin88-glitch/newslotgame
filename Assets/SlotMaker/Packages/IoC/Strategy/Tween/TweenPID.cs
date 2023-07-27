using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.IoC.Strategy.Tween
{
    [CreateAssetMenu(fileName="New TweenPID", menuName="SlotMaker2/IoC/Tween/PID")]
    public class TweenPID : TweenStrategy
    {
        public float frequency;
        public float damping;
        public float epsilon = 0.001f;

        public override float Epsilon { get { return epsilon; } }

        public override bool StaticTween() { return false; }

        public override float UpdateTween(float start, float end, ref float velocity, float desiredVelocity, float deltaTime)
        {
            start += PIDUtils.CalcDisplacement(start, end, ref velocity, desiredVelocity, frequency, damping, deltaTime);            
            return start;
        }

        public override Vector2 UpdateTween(Vector2 start, Vector2 end, ref Vector2 velocity, Vector2 desiredVelocity, float deltaTime)
        {
            start += PIDUtils.CalcDisplacement(start, end, ref velocity, desiredVelocity, frequency, damping, deltaTime);
            return start;
        }

        public override Vector3 UpdateTween(Vector3 start, Vector3 end, ref Vector3 velocity, Vector3 desiredVelocity, float deltaTime)
        {
            start += PIDUtils.CalcDisplacement(start, end, ref velocity, desiredVelocity, frequency, damping, deltaTime);
            return start;
        }

        public override Vector4 UpdateTween(Vector4 start, Vector4 end, ref Vector4 velocity, Vector4 desiredVelocity, float deltaTime)
        {
            start += PIDUtils.CalcDisplacement(start, end, ref velocity, desiredVelocity, frequency, damping, deltaTime);
            return start;
        }

        public override Quaternion UpdateTween(Quaternion start, Quaternion end, ref Vector3 angularVelocity, Vector3 desiredAngularVelocity, float deltaTime) 
        { 
            // TODO
            return end; 
        }

        public override Color UpdateTween(Color start, Color end, ref Color velocity, Color desiredVelocity, float deltaTime)
        {
            start += PIDUtils.CalcDisplacement(start, end, ref velocity, desiredVelocity, frequency, damping, deltaTime);
            return start;
        }
    }
}