using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.IoC.Tween
{
    public class TweenSingle : TweenMediator<float, TweenSequenceSingle>
    {
        protected override float GetValue(Component comp)
        {
            return propertyInjector.GetSingle(comp);
        }

        protected override void SetValue(Component comp, float value)
        {
            propertyInjector.SetSingle(comp, value);
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