using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;


namespace BSS.Utils { 
    public static partial class MathUtility
    {
        public static int WeightedRandom(IEnumerable<float> list)
        { 
            float sum = list.Sum();
            float r = Random.Range(0, sum);
            float checkVal = 0;
            int i = 0;
            foreach (var it in list)
            {
                checkVal += it;
                if (r < checkVal)
                {
                    return i;
                }
                i++;
            }
            return i - 1;
        }
    }
}
