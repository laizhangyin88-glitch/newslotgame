using UnityEngine;
using System.Collections.Generic;

namespace BagelCode
{
    public static class MathUtils
    {
        #region General

        public static void NormalizeArrayIndex(ref int idx, int size)
        {
            if (size <= 0) return;

            while (idx < 0)
                idx += size;

            while (idx >= size)
                idx -= size;
        }

        #endregion

        #region Vector3

        #endregion

        #region Vector2

        public static float Inner(Vector2 a, Vector2 b)
        {
            float leftExpression = a.x * b.x + a.y * b.y;
            return leftExpression / (a.magnitude * b.magnitude);
        }

        public static Vector2 Direction(Vector2 from, Vector2 to)
        {
            return (to - from).normalized;
        }

        public static float Degrees(Vector2 from, Vector2 to)
        {
            Vector2 dir = (to - from).normalized;
            return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }

        #endregion

        #region float

        #endregion

        #region int

        #endregion

        #region long

        public static long Lerp(long a, long b, float t)
        {
            return (long)(a + (b - a) * Mathf.Clamp01(t));
        }

        #endregion
    }
}
