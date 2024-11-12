using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public class SharedPtr
    {
        public int UseCount { get; protected set; }

        public float UnUsedTime { get; protected set; }

        public SharedPtr()
        {
            ClearSharedPtr();
        }

        public bool IsUnUsedPtr(float cachingTime = float.MinValue)
        {
            return (UseCount == 0) && (Time.time - UnUsedTime) > cachingTime;
        }

        public void IncreaseUseCount()
        {
            ++UseCount;

            UnUsedTime = float.MaxValue;
        }

        public void DecreaseUseCount()
        {
            --UseCount;

            if (UseCount == 0)
                UnUsedTime = Time.time;
        }

        public void ClearSharedPtr()
        {
            UseCount = 0;
            UnUsedTime = float.MaxValue;
        }
    }
}
