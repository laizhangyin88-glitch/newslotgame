using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public class AspectRatioCameraManager : MonoBehaviour
    {
        public TargetPlatform platform;
        public bool useRatio = false;

        public float targetRatio = 1.777f;

        public Camera[] cameras;

        public GameObject marginCameraPrefab;
        private GameObject marginCameraObj;
        private Camera marginCamera;

        private int prevWidth = 0;
        private int prevHeight = 0;

        private Rect oneRect = new Rect(0,0,1,1);

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

                Init();
                UpdateAspectRatio();
            }
            else
            {
                GameObject.Destroy(this);
            }
        }

        private void OnEnable()
        {
            UpdateAspectRatio();
        }

        private void Init()
        {
            cameras = GetComponentsInChildren<Camera>();

            if(marginCameraObj == null && marginCameraPrefab != null)
            {
                marginCameraObj = GameObject.Instantiate(marginCameraPrefab) as GameObject;
                marginCameraObj.name = marginCameraPrefab.name;
                marginCameraObj.transform.SetParent(transform, false);
                marginCamera = marginCameraObj.GetComponent<Camera>();
            }
        }

        private void SetCameraRect(Rect rect)
        {
            if(cameras == null) return;

            for(int i=0; i < cameras.Length; ++i)
            {
                cameras[i].rect = rect;
            }

            if(marginCamera != null)
            {
                marginCamera.rect = oneRect;
            }
        }

        private void UpdateAspectRatio()
        {
            if(useRatio == false || cameras == null || cameras.Length == 0) return;

            float currentAspectRatio = (float)Screen.width / Screen.height;

            if (currentAspectRatio > targetRatio)
            {
                // Pillarbox
                float inset = 1.0f - targetRatio / currentAspectRatio;
                SetCameraRect( new Rect(inset / 2, 0.0f, 1.0f - inset, 1.0f) );
            }
            else
            {
                // Letterbox
                float inset = 1.0f - currentAspectRatio / targetRatio;
                SetCameraRect( new Rect(0.0f, inset / 2, 1.0f, 1.0f - inset) );
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