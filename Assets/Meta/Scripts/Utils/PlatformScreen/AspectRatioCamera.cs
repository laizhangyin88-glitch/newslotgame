using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    [RequireComponent(typeof(Camera))]
    public class AspectRatioCamera : MonoBehaviour
    {
        public TargetPlatform platform;
        public bool useRatio = false;

        public float targetRatio = 1.777f;

        private Camera targetCamera;

        private int prevWidth = 0;
        private int prevHeight = 0;

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
                useRatio = true;
                targetCamera = GetComponent<Camera>();
                UpdateAspectRatio();
            }
            else
            {
#if UNITY_EDITOR
                targetCamera = GetComponent<Camera>();
                UpdateAspectRatio();
#else
                GameObject.Destroy(this);
#endif
            }
        }

        private void UpdateAspectRatio()
        {
            if(useRatio == false || targetCamera == null) return;

            float currentAspectRatio = (float)Screen.width / Screen.height;

            if (currentAspectRatio > targetRatio)
            {
                // Pillarbox
                float inset = 1.0f - targetRatio / currentAspectRatio;
                targetCamera.rect = new Rect(inset / 2, 0.0f, 1.0f - inset, 1.0f);
            }
            else
            {
                // Letterbox
                float inset = 1.0f - currentAspectRatio / targetRatio;
                targetCamera.rect = new Rect(0.0f, inset / 2, 1.0f, 1.0f - inset);
            }

            prevWidth = Screen.width;
            prevHeight = Screen.height;
        }

        private void Update()
        {
            if(useRatio && (prevWidth != Screen.width || prevHeight != Screen.height))
            {
                UpdateAspectRatio();
            }
        }
    }
}