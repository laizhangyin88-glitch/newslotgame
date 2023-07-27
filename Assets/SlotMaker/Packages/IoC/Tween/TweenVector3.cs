using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.IoC.Tween
{
    public class TweenVector3 : TweenMediator<Vector3, TweenSequenceVector3>
    {
        public Vector3Mask constraints;

        protected override Vector3 GetValue(Component comp)
        {
            if (constraints.Any())
            {
                var tmp = propertyInjector.GetVector3(comp);
                if (constraints.x) tmp.x = 0f;
                if (constraints.y) tmp.y = 0f;
                if (constraints.z) tmp.z = 0f;
                return tmp;
            }
            else 
            {
                return propertyInjector.GetVector3(comp);
            }
        }

        protected override void SetValue(Component comp, Vector3 value)
        {
            if (constraints.Any())
            {
                var tmp = propertyInjector.GetVector3(comp);
                if (!constraints.x) tmp.x = value.x;
                if (!constraints.y) tmp.y = value.y;
                if (!constraints.z) tmp.z = value.z;
                propertyInjector.SetVector3(comp, tmp);
            }
            else 
            {
                propertyInjector.SetVector3(comp, value);
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