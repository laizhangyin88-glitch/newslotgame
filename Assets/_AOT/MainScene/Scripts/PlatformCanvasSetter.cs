using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SlotMaker;

namespace BagelCode
{
    [RequireComponent(typeof(CanvasScaler))]
    public class PlatformCanvasSetter : MonoBehaviour
    {
        public TargetPlatform platform;
        public Vector2 resolution;

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
                CanvasScaler canvasScaler = GetComponent<CanvasScaler>();
                canvasScaler.referenceResolution = resolution;
            }
            
            GameObject.Destroy(this);
        }
    }
}