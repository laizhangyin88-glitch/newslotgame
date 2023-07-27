using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BSS.Utils { 
    public static class ConvertUtility 
    {
        public static Rect ToRect(this Bounds bounds)
        {
            return new Rect(bounds.min, bounds.size);
        }
    }
}
