using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public class ScreenSafeAreaCameraManager : MonoBehaviour
    {
        public TargetPlatform platform;

        public Camera[] cameras;

        public GameObject marginCameraPrefab;
        private GameObject marginCameraObj;
        private Camera marginCamera;

        private Rect prevSafeArea = new Rect(0,0,0,0);

        private Rect oneRect = new Rect(0,0,1,1);

        private bool isNotch = false;
        private bool isLandscape = true;

        private int prevWidth = 0;
        private int prevHeight = 0;

        private void Awake()
        {
#if UNITY_IOS
            if (((int)platform & (int)TargetPlatform.IOS) != 0)
#elif UNITY_ANDROID
            if (((int)platform & (int)TargetPlatform.Android) != 0)
// #elif UNITY_STANDALONE
//             if (((int)platform & (int)TargetPlatform.Standalone) != 0)
// #elif UNITY_WEBGL
//             if (((int)platform & (int)TargetPlatform.WebGL) != 0)
// #elif UNITY_WSA
//             if (((int)platform & (int)TargetPlatform.WSA) != 0)
#else 
            if (false)
#endif
            {
                Init();
            }
            else
            {
                GameObject.Destroy(this);
            }
        }

        private void Init()
        {
            cameras = GetComponentsInChildren<Camera>();

            isNotch = ScreenSafeAreaUtils.IsNotch();

            UpdateCameraRect();
        }

        private void MakeMarginCamera()
        {
            if(marginCameraObj == null && marginCameraPrefab != null)
            {
                marginCameraObj = GameObject.Instantiate(marginCameraPrefab) as GameObject;
                marginCameraObj.name = "Margin Camera";
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

        public void ChangeOrientation(ScreenOrientation targetOrientation)
        {
            isLandscape = Screen.autorotateToLandscapeLeft || Screen.autorotateToLandscapeRight;

            StopCoroutine("ChangeRect");
            StartCoroutine("ChangeRect");
        }

        private IEnumerator ChangeRect()
        {
            while(prevWidth == Screen.width && prevHeight == Screen.height)
            {
                yield return null;
            }

            prevWidth = Screen.width;
            prevHeight = Screen.height;

            UpdateCameraRect();
        }

        private void UpdateCameraRect()
        {
            var safeRect = GetSafeArea();
            if(prevSafeArea == safeRect) return;

            MakeMarginCamera();
            SetCameraRect( new Rect( safeRect.xMin / (float)Screen.width,
                                     safeRect.yMin / (float)Screen.height,
                                     safeRect.width / (float)Screen.width,
                                     safeRect.height / (float)Screen.height)
            );

            prevSafeArea = safeRect;
        }

        private Rect GetSafeArea()
        {
            // Todo : not fit. safe area
            if(isNotch)
            {
                // Debug.LogError(string.Format("Safe Area {10} : {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}",
                //      Screen.safeArea.xMin, Screen.safeArea.yMin, Screen.safeArea.xMax, Screen.safeArea.yMax, Screen.safeArea.width, Screen.safeArea.height, Screen.safeArea.center.x, Screen.safeArea.center.y, Screen.width, Screen.height, isLandscape ? "Landscape" : "Portrait"));
                float fixelX = 0.04f;
                float fixelY = 0.04f;

                float relativeX = fixelX * Screen.width;
                float relativeY = fixelY * Screen.height;

                if(isLandscape)
                {
                    return new Rect(relativeX, relativeY, Screen.width-(relativeX*2f), Screen.height-relativeY);
                }
                else
                {
                    return new Rect(0, relativeY/2f, Screen.width, Screen.height-(relativeY+relativeY/2f));
                }
            }
            else
                return new Rect(0, 0, Screen.width, Screen.height);
        }
    }
}