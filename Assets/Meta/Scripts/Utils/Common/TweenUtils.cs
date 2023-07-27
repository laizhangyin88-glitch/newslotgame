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

        #endregion
    }
}
