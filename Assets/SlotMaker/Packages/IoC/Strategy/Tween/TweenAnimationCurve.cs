using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.IoC.Strategy.Tween
{
    [CreateAssetMenu(fileName="New TweenCurve", menuName="SlotMaker2/IoC/Tween/AnimationCurve")]
    public class TweenAnimationCurve : TweenStrategy
    {
        public AnimationCurve animationCurve;

        public override bool StaticTween() { return true; }

        public override float UpdateTween(float start, float end, float value)
        {
            return Mathf.LerpUnclamped(start, end, animationCurve.Evaluate(value));
        }

        public override Vector2 UpdateTween(Vector2 start, Vector2 end, float value)
        {
            return Vector2.LerpUnclamped(start, end, animationCurve.Evaluate(value));
        }

        public override Vector3 UpdateTween(Vector3 start, Vector3 end, float value)
        {
            return Vector3.LerpUnclamped(start, end, animationCurve.Evaluate(value));
        }

        public override Vector4 UpdateTween(Vector4 start, Vector4 end, float value)
        {
            return Vector4.LerpUnclamped(start, end, animationCurve.Evaluate(value));
        }

        public override Quaternion UpdateTween(Quaternion start, Quaternion end, float value)
        {
            return Quaternion.SlerpUnclamped(start, end, animationCurve.Evaluate(value));
        }

        public override Color UpdateTween(Color start, Color end, float value)
        {
            return Color.LerpUnclamped(start, end, animationCurve.Evaluate(value));
        }
    }
}