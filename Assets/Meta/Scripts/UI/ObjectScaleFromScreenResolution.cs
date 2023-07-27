using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public class ObjectScaleFromScreenResolution : MonoBehaviour
    {
        public TargetPlatform platform;

        public float minRatio = 1.333f; // ex ) 4 : 3 = 1.333
        public float maxRatio = 1.777f; // ex ) 16 : 9 = 1.777

        public Vector3 minScale = Vector3.one; // ex ) 1.2, 1.2, 1
        public Vector3 maxScale = Vector3.one; // ex ) 1, 1, 1

        private void Awake()
        {
    #if UNITY_IOS
            if (((int)platform & (int)TargetPlatform.IOS) != 0)
    #elif UNITY_ANDROID
            if (((int)platform & (int)TargetPlatform.Android) != 0)
    #elif UNITY_STANDALONE
            if (((int)platform & (int)TargetPlatform.Standalone) != 0)
    #elif UNITY_WEBGL
            if (((int)platform & (int)TargetPlatform.WebGL) != 0)
    #elif UNITY_WSA
            if (((int)platform & (int)TargetPlatform.WSA) != 0)
    #else
            if (false)
    #endif
            {
                transform.localScale = GetScale();
            }
        }

        public Vector3 GetScale()
        {
            float width = (float)Screen.width;
            float height = (float)Screen.height;
            float ratio = width/height;

            ratio = Mathf.Clamp(ratio, minRatio, maxRatio);

            if(ratio <= minRatio)
                return minScale;
            else if(ratio >= maxRatio)
                return maxScale;

            ratio = (maxRatio - ratio)/(maxRatio - minRatio);
            return Vector3.Lerp(minScale, maxScale, 1f - ratio);
        }
    }

}