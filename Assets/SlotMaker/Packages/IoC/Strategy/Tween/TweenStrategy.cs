using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.IoC.Strategy.Tween
{
    [Serializable]
    public abstract class TweenStrategy : ScriptableObject
    {
        public virtual float Epsilon { get { return Mathf.Epsilon; } }

        public abstract bool StaticTween();
        
        public virtual float UpdateTween(float start, float end, float value) { return end; }
        public virtual Vector2 UpdateTween(Vector2 start, Vector2 end, float value) { return end; }
        public virtual Vector3 UpdateTween(Vector3 start, Vector3 end, float value) { return end; }
        public virtual Vector4 UpdateTween(Vector4 start, Vector4 end, float value) { return end; }
        public virtual Quaternion UpdateTween(Quaternion start, Quaternion end, float value) { return end; }
        public virtual Color UpdateTween(Color start, Color end, float value) { return end; }
        
        public virtual bool IsCompleteTween(float start, float end, float velocity, float desiredVelocity)
        { 
            return (Mathf.Abs(end - start) < Epsilon) &&
                (Mathf.Abs(desiredVelocity - velocity) < Epsilon);
        }

        public virtual bool IsCompleteTween(Vector2 start, Vector2 end, Vector2 velocity, Vector2 desiredVelocity)
        {
            return ((end - start).sqrMagnitude < Epsilon) &&
                ((desiredVelocity - velocity).sqrMagnitude < Epsilon);
        }

        public virtual bool IsCompleteTween(Vector3 start, Vector3 end, Vector3 velocity, Vector3 desiredVelocity)
        {
            return ((end - start).sqrMagnitude < Epsilon) &&
                ((desiredVelocity - velocity).sqrMagnitude < Epsilon);
        }

        public virtual bool IsCompleteTween(Vector4 start, Vector4 end, Vector4 velocity, Vector4 desiredVelocity)
        {
            return ((end - start).sqrMagnitude < Epsilon) &&
                ((desiredVelocity - velocity).sqrMagnitude < Epsilon);
        }

        public virtual bool IsCompleteTween(Quaternion start, Quaternion end, Vector3 angularVelocity, Vector3 desiredAngularVelocity)
        {
            // TODO
            return true;
        }

        public virtual bool IsCompleteTween(Color start, Color end, Color velocity, Color desiredVelocity)
        {
            return IsCompleteTween((Vector4)start, (Vector4)end, (Vector4)velocity, (Vector4)desiredVelocity);
        }

        public virtual float UpdateTween(float start, float end, ref float velocity, float desiredVelocity, float deltaTime) { return end; }
        public virtual Vector2 UpdateTween(Vector2 start, Vector2 end, ref Vector2 velocity, Vector2 desiredVelocity, float deltaTime) { return end; }
        public virtual Vector3 UpdateTween(Vector3 start, Vector3 end, ref Vector3 velocity, Vector3 desiredVelocity, float deltaTime) { return end; }
        public virtual Vector4 UpdateTween(Vector4 start, Vector4 end, ref Vector4 velocity, Vector4 desiredVelocity, float deltaTime) { return end; }
        public virtual Quaternion UpdateTween(Quaternion start, Quaternion end, ref Vector3 angularVelocity, Vector3 desiredAngularVelocity, float deltaTime) { return end; }
        public virtual Color UpdateTween(Color start, Color end, ref Color velocity, Color desiredVelocity, float deltaTime) { return end; }
    }
}