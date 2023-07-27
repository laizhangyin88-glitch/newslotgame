using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.IoC.Tween
{
    public class TweenVector2 : TweenMediator<Vector2, TweenSequenceVector2>
    {
        public Vector2Mask constraints;

        protected override Vector2 GetValue(Component comp)
        {
            if (constraints.Any())
            {
                var tmp = propertyInjector.GetVector2(comp);
                if (constraints.x) tmp.x = 0f;
                if (constraints.y) tmp.y = 0f;
                return tmp;
            }
            else 
            {
                return propertyInjector.GetVector2(comp);
            }
        }

        protected override void SetValue(Component comp, Vector2 value)
        {
            if (constraints.Any())
            {
                var tmp = propertyInjector.GetVector2(comp);
                if (!constraints.x) tmp.x = value.x;
                if (!constraints.y) tmp.y = value.y;
                propertyInjector.SetVector2(comp, tmp);
            }
            else 
            {
                propertyInjector.SetVector2(comp, value);
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