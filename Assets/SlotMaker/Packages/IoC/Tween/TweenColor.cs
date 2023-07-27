using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.IoC.Tween
{
    public class TweenColor : TweenMediator<Color, TweenSequenceColor>
    {
        public ColorMask constraints;

        protected override Color GetValue(Component comp)
        {
            if (constraints.Any())
            {
                var tmp = propertyInjector.GetColor(comp);
                if (constraints.r) tmp.r = 0f;
                if (constraints.g) tmp.g = 0f;
                if (constraints.b) tmp.b = 0f;
                if (constraints.a) tmp.a = 0f;
                return tmp;
            }
            else
            {
                return propertyInjector.GetColor(comp);
            }
        }

        protected override void SetValue(Component comp, Color value)
        {
            if (constraints.Any())
            {
                var tmp = propertyInjector.GetColor(comp);
                if (!constraints.r) tmp.r = value.r;
                if (!constraints.g) tmp.g = value.g;
                if (!constraints.b) tmp.b = value.b;
                if (!constraints.a) tmp.a = value.a;
                propertyInjector.SetColor(comp, tmp);
            }
            else 
            {
                propertyInjector.SetColor(comp, value);
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