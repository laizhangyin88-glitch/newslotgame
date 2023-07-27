using UnityEngine;

namespace SlotMaker
{
    public class DestroyMask : MonoBehaviour
    {
        [System.Flags]
        public enum DestroyPolicy
        {
            Auto   = (1 << 0),
            Manual = (2 << 0),
        };
        
    #if UNITY_EDITOR
        [EnumFlags]
    #endif
        public DestroyPolicy policy = DestroyPolicy.Auto;

        public static bool HasAttribute(DestroyPolicy source, DestroyPolicy mask)
        {
            return (int)(source & mask) == (int)mask;
        }

        public static bool HasAttribute(DestroyPolicy source, int mask)
        {
            return (int)((int)source & mask) == mask;
        }
    }
}
