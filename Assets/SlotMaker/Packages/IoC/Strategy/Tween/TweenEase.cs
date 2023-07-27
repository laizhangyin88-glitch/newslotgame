/// See - Unity - Easing Libarary Visualisation
/// https://unitylist.com/p/27q/Unity-Easing-Library-Visualisation

/////////////////////////////////////////////////////////////////////////////////////
/// Bounce.cs
/////////////////////////////////////////////////////////////////////////////////////

// Author: Daniele Giardini (C# port of the easing equations created by Robert Penner - http://robertpenner.com/easing)
//
// TERMS OF USE - EASING EQUATIONS
//
// Open source under the BSD License.
//
// Copyright © 2001 Robert Penner
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice,
// this list of conditions and the following disclaimer.
// - Redistributions in binary form must reproduce the above copyright notice,
// this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
// - Neither the name of the author nor the names of contributors may be used to endorse
// or promote products derived from this software without specific prior written permission.
// - THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
// IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
// EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.THERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
// EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.IoC.Strategy.Tween
{
    [CreateAssetMenu(fileName="New TweenEase", menuName="SlotMaker2/IoC/Tween/Ease")]
    public class TweenEase : TweenStrategy
    {
        public enum Ease
        {
            Linear,
            InSine,
            OutSine,
            InOutSine,
            InQuad,
            OutQuad,
            InOutQuad,
            InCubic,
            OutCubic,
            InOutCubic,
            InQuart,
            OutQuart,
            InOutQuart,
            InQuint,
            OutQuint,
            InOutQuint,
            InExpo,
            OutExpo,
            InOutExpo,
            InCirc,
            OutCirc,
            InOutCirc,
            InElastic,
            OutElastic,
            InOutElastic,
            InBack,
            OutBack,
            InOutBack,
            InBounce,
            OutBounce,
            InOutBounce
        }
        public Ease easeType;
        [ShowIf("ShowOptions")]
        public float overshootOrAmplitude = 1f;
        [ShowIf("ShowOptions")]
        public float period = 0.3f;

        const float _PiOver2 = Mathf.PI * 0.5f;
        const float _TwoPi = Mathf.PI * 2;

        public override bool StaticTween() { return true; }

        public override float UpdateTween(float start, float end, float value)
        {
            return Mathf.LerpUnclamped(start, end, Evaluate(value));
        }

        public override Vector2 UpdateTween(Vector2 start, Vector2 end, float value)
        {
            return Vector2.LerpUnclamped(start, end, Evaluate(value));
        }

        public override Vector3 UpdateTween(Vector3 start, Vector3 end, float value)
        {
            return Vector3.LerpUnclamped(start, end, Evaluate(value));
        }

        public override Vector4 UpdateTween(Vector4 start, Vector4 end, float value)
        {
            return Vector4.LerpUnclamped(start, end, Evaluate(value));
        }

        public override Quaternion UpdateTween(Quaternion start, Quaternion end, float value)
        {
            return Quaternion.SlerpUnclamped(start, end, Evaluate(value));
        }

        public override Color UpdateTween(Color start, Color end, float value)
        {
            return Color.LerpUnclamped(start, end, Evaluate(value));
        }

        public bool ShowOptions
        {
            get
            {
                switch (easeType)
                {
                case Ease.InElastic:
                case Ease.OutElastic:
                case Ease.InOutElastic:
                case Ease.InBack:
                case Ease.OutBack:
                case Ease.InOutBack:
                    return true;
                default:
                    break;
                }

                return false;
            }
        }

        public float Evaluate(float value)
        {
            switch (easeType)
            {
            case Ease.Linear:
                return value;
            case Ease.InSine:
                return -Mathf.Cos(value * _PiOver2) + 1f;
            case Ease.OutSine:
                return Mathf.Sin(value * _PiOver2);
            case Ease.InOutSine:
                return -0.5f * (Mathf.Cos(Mathf.PI * value) - 1f);
            case Ease.InQuad:
                return value * value;
            case Ease.OutQuad:
                return -value * (value - 2f);
            case Ease.InOutQuad:
                value *= 2f;
                if (value < 1f) return 0.5f * value * value;
                value -= 1f;
                return -0.5f * (value * (value - 2f) - 1f);
            case Ease.InCubic:
                return value * value * value;
            case Ease.OutCubic:
                value -= 1f;
                return value * value * value + 1f;
            case Ease.InOutCubic:
                value *= 2f;
                if (value < 1f) return 0.5f * value * value * value;
                value -= 2f;
                return 0.5f * (value * value * value + 2f);
            case Ease.InQuart:
                return value * value * value * value;
            case Ease.OutQuart:
                value -= 1f;
                return -(value * value * value * value - 1f);
            case Ease.InOutQuart:
                value *= 2f;
                if (value < 1f) return 0.5f * value * value * value * value;
                value -= 2f;
                return -0.5f * (value * value * value * value - 2f);
            case Ease.InQuint:
                return value * value * value * value * value;
            case Ease.OutQuint:
                value -= 1f;
                return value * value * value * value * value + 1f;
            case Ease.InOutQuint:
                value *= 2f;
                if (value < 1f) return 0.5f * value * value * value * value * value;
                value -= 2f;
                return 0.5f * (value * value * value * value * value + 2f);
            case Ease.InExpo:
                if (value == 0f) return 0f;
                return Mathf.Pow(2f, 10f * (value - 1f));
            case Ease.OutExpo:
                if (value == 1f) return 1f;
                return -Mathf.Pow(2f, -10f * value) + 1f;
            case Ease.InOutExpo:
                if (value == 0f) return 0f;
                else if (value == 1f) return 1f;
                value *= 2f;
                if (value < 1f) return 0.5f * Mathf.Pow(2f, 10f * (value - 1f));
                value -= 1f;
                return 0.5f * (-Mathf.Pow(2f, -10f * value) + 2f);
            case Ease.InCirc:
                return -(Mathf.Sqrt(1f - value * value) - 1f);
            case Ease.OutCirc:
                value -= 1f;
                return Mathf.Sqrt(1f - value * value);
            case Ease.InOutCirc:
                value *= 2f;
                if (value < 1f) return -0.5f * (Mathf.Sqrt(1f - value * value) - 1f);
                value -= 2f;
                return 0.5f * (Mathf.Sqrt(1f - value * value) + 1f);
            case Ease.InElastic:
                if (value == 0f) return 0f;
                else if (value == 1f) return 1f;
                if (period == 0f) period = 0.3f;
                float s0;
                if (overshootOrAmplitude < 1f)
                {
                    overshootOrAmplitude = 1f;
                    s0 = period * 0.25f;
                }
                else 
                {
                    s0 = period / _TwoPi * Mathf.Asin(1f / overshootOrAmplitude);
                }
                value -= 1f;
                return -(overshootOrAmplitude * Mathf.Pow(2f, 10f * value) * Mathf.Sin((value - s0) * _TwoPi / period));
            case Ease.OutElastic:
                if (value == 0f) return 0f;
                else if (value == 1f) return 1f;
                if (period == 0f) period = 0.3f;
                float s1;
                if (overshootOrAmplitude < 1f)
                {
                    overshootOrAmplitude = 1f;
                    s1 = period * 0.25f;
                }
                else
                {
                    s1 = period / _TwoPi * Mathf.Asin(1f / overshootOrAmplitude);
                }
                return (overshootOrAmplitude * Mathf.Pow(2f, -10f * value) * Mathf.Sin((value - s1) * _TwoPi / period) + 1f);
            case Ease.InOutElastic:
                if (value == 0f) return 0f;
                else if (value == 1f) return 1f;
                if (period == 0f) period = 0.45f;
                float s;
                if (overshootOrAmplitude < 1f)
                {
                    overshootOrAmplitude = 1f;
                    s = period * 0.25f;
                }
                else 
                {
                    s = period / _TwoPi * Mathf.Asin(1f / overshootOrAmplitude);
                }
                value *= 2f;
                if (value < 1f)
                {
                    value -= 1f;
                    return -(0.5f * overshootOrAmplitude * Mathf.Pow(2f, 10f * value) * Mathf.Sin((value - s) * _TwoPi / period));
                }
                value -= 1f;
                return (0.5f * overshootOrAmplitude * Mathf.Pow(2f, -10f * value) * Mathf.Sin((value - s) * _TwoPi / period)) + 1f;
            case Ease.InBack:
                return value * value * ((overshootOrAmplitude + 1f) * value - overshootOrAmplitude);
            case Ease.OutBack:
                value -= 1f;
                return value * value * ((overshootOrAmplitude + 1f) * value + overshootOrAmplitude) + 1f;
            case Ease.InOutBack:
                value *= 2f;
                float _overshootOrAmplitude = overshootOrAmplitude * 1.525f;
                if (value < 1f) return 0.5f * value * value * ((_overshootOrAmplitude + 1f) * value - _overshootOrAmplitude);
                value -= 2f;
                return 0.5f * (value * value * ((_overshootOrAmplitude + 1f) * value + _overshootOrAmplitude) + 2f);
            case Ease.InBounce:
                return Bounce.EaseIn(value);
            case Ease.OutBounce:
                return Bounce.EaseOut(value);
            case Ease.InOutBounce:
                return Bounce.EaseInOut(value);
            default:
                break;
            }
            return value;// Linear
        }

        public static class Bounce
        {
            public static float EaseIn(float value)
            {
                return 1f - EaseOut(1f - value);
            }

            public static float EaseOut(float value)
            {
                if (value < (1f / 2.75f))
                {
                    return 7.5625f * value * value;
                }
                else if (value < (2f / 2.75f))
                {
                    value -= (1.5f / 2.75f);
                    return 7.5625f * value * value + 0.75f;
                }
                else if (value < 2.5f / 2.75f)
                {
                    value -= (2.25f / 2.75f);
                    return 7.5625f * value * value + 0.9375f;
                }
                else 
                {
                    value -= (2.625f / 2.75f);
                    return 7.5625f * value * value + 0.984375f;
                }
            }

            public static float EaseInOut(float value)
            {
                if (value < 0.5f)
                    return EaseIn(value * 2f) * 0.5f;
                else 
                    return EaseOut(value * 2f - 1f) * 0.5f + 0.5f;
            }
        }
    }
}