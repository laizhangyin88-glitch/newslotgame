using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.IoC.Tween
{
    public class TweenVector4 : TweenMediator<Vector4, TweenSequenceVector4>
    {
        public Vector4Mask constraints;

        protected override Vector4 GetValue(Component comp)
        {
            if (constraints.Any())
            {
                var tmp = propertyInjector.GetVector4(comp);
                if (constraints.x) tmp.x = 0f;
                if (constraints.y) tmp.y = 0f;
                if (constraints.z) tmp.z = 0f;
                if (constraints.w) tmp.w = 0f;
                return tmp;
            }
            else 
            {
                return propertyInjector.GetVector4(comp);
            }
        }

        protected override void SetValue(Component comp, Vector4 value)
        {
            if (constraints.Any())
            {
                var tmp = propertyInjector.GetVector4(comp);
                if (!constraints.x) tmp.x = value.x;
                if (!constraints.y) tmp.y = value.y;
                if (!constraints.z) tmp.z = value.z;
                if (!constraints.w) tmp.w = value.w;
                propertyInjector.SetVector4(comp, tmp);
            }
            else 
            {
                propertyInjector.SetVector4(comp, value);
            }
        }

        protected override void UpdateStaticTween()
        {
            base.UpdateStaticTween();

            SetValue(target, tween.UpdateTween(from, to, PlayingOffset));
        }

        protected override void UpdateDynamicTween()
        {
            base.UpdateDynamicTween();

            SetValue(target, tween.UpdateTween(GetValue(target), to, ref velocity, desiredVelocity, DeltaTime));
        }

        protected override bool IsCompleteDynamicTween()
        {
            return tween.IsCompleteTween(GetValue(target), to, velocity, desiredVelocity);
        }
    }
}