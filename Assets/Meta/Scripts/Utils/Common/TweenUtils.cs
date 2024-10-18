using System;
using UnityEngine;

namespace BagelCode
{
    public static class TweenUtils
    {
        #region Tween Vector

        public static Vector3 VectorTweenLinear(Vector3 from, Vector3 to, float t)
        {
            return Vector3.Lerp(from, to, t);
        }

        public static Vector3 VectorTweenCollectMove(Vector3 from, Vector3 to, float t)
        {
            return Vector3.Lerp(from, to, TweenCollectMove(t));
        }

        public static Vector3 VectorTweenInSine(Vector3 from, Vector3 to, float t)
        {
            return Vector3.Lerp(from, to, TweenInSine(t));
        }

        public static Vector3 VectorTweenOutSine(Vector3 from, Vector3 to, float t)
        {
            return Vector3.Lerp(from, to, TweenOutSine(t));
        }

        public static Vector3 VectorTweenInCubic(Vector3 from, Vector3 to, float t)
        {
            return Vector3.Lerp(from, to, TweenInCubic(t));
        }

        public static Vector3 VectorTweenOutCubic(Vector3 from, Vector3 to, float t)
        {
            return Vector3.Lerp(from, to, TweenOutCubic(t));
        }

        public static Vector3 VectorTweenInQuint(Vector3 from, Vector3 to, float t)
        {
            return Vector3.Lerp(from, to, TweenInQuint(t));
        }

        public static Color ColorTweenInSine(Color from, Color to, float t)
        {
            return Color.Lerp(from, to, TweenInSine(t));
        }

        public static Color ColorTweenInQuad(Color from, Color to, float t)
        {
            return Color.Lerp(from, to, TweenInQuad(t));
        }
        public static Vector3 VectorTweenEaseInExpo(Vector3 from, Vector3 to, float t)
        {
            return Vector3.Lerp(from, to, easeInExpo(t));
        }
        #endregion

        #region Tween Float
        // preview https://easings.net/ko

        public static float TweenInQuad(float t)
        {
            return t * t;
        }

        public static float TweenOutQuad(float t)
        {
            return 1f - (1f - t) * (1f - t);
        }

        public static float TweenInOutQuad(float t)
        {
            return t < 0.5f ? 2f * t * t : 1 - Mathf.Pow(-2f * t + 2f, 2f) * 0.5f;
        }

        public static float TweenInSine(float t)
        {
            return 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
        }

        public static float TweenOutSine(float t)
        {
            return Mathf.Sin(t * Mathf.PI * 0.5f);
        }

        public static float TweenInOutSine(float t)
        {
            return -(Mathf.Cos(t * Mathf.PI) - 1f) * 0.5f;
        }

        public static float TweenCollectMove(float t)
        {
            return -t * t + 2f * t;
        }

        public static float TweenInCubic(float t)
        {
            return t * t * t;
        }

        public static float TweenOutCubic(float t)
        {
            return 1 - Mathf.Pow(1 - t, 3);
        }

        public static float TweenInQuint(float t)
        {
            return t * t * t * t * t;
        }

        public static float easeInExpo(float t)
        {
            return (float)(t == 0 ? 0 : Math.Pow(2, 10 * t - 10));
        }

        #endregion
    }
}
