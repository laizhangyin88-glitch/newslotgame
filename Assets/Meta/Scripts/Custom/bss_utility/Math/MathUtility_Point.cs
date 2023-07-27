using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BSS.Utils {
    public static partial class MathUtility
    {
        public static Vector3 BezierCurve(Vector3 start,Vector3 curve,Vector3 end,float t)
        {
            Vector3 p1 = Vector3.Lerp(start, curve, t);
            Vector3 p2 = Vector3.Lerp(curve, end, t);
            Vector3 p3 = Vector3.Lerp(p1, p2, t);
            
            return p3;
        }
    }
}
