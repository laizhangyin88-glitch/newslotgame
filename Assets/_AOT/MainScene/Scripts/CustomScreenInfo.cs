using System;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace SlotMaker
{
    [CreateAssetMenu(fileName = "CustomScreenInfo", menuName = "SlotMaker/ScriptableObject/CustomScreenInfo")]
    public class CustomScreenInfo : ScriptableObjectSingleton<CustomScreenInfo>
    {
        public bool test;
        public Vector2 iPhoneXReferenceResolution;
        public Rect iPhoneXCameraRect;

        private bool isNotch
        {
            get 
            {
                if (test) return true;
#if UNITY_IOS
                switch(Device.generation)
                {
                    case DeviceGeneration.iPhoneX:
                    case DeviceGeneration.iPhoneXS:
                    case DeviceGeneration.iPhoneXSMax:
                    case DeviceGeneration.iPhoneXR:
                        return true;
                    case DeviceGeneration.iPhoneUnknown:
                        return IsUknowniPhoneX();
                }
#endif
                return false;
            }
        }
        
        public static void SetCustomReferenceResolution(CanvasScaler canvasScaler)
        {
            if (Instance.isNotch)
            {
                canvasScaler.referenceResolution = Instance.iPhoneXReferenceResolution;
            }
        }

        public static void SetCustomRectTransformAnchor(RectTransform rectTransform)
        {
            if (Instance.isNotch)
            {
                var sizeDeltaX = rectTransform.rect.width * (Instance.iPhoneXCameraRect.size.x - 1);
                var sizeDeltaY = rectTransform.rect.height * (Instance.iPhoneXCameraRect.size.y - 1);
                var positionX = rectTransform.rect.width * Instance.iPhoneXCameraRect.position.x /2;
                var positionY = rectTransform.rect.height * Instance.iPhoneXCameraRect.position.y /2;

                rectTransform.sizeDelta = new Vector2(sizeDeltaX, sizeDeltaY);
                rectTransform.anchoredPosition = new Vector2(positionX, positionY);
            }
        }

        public static void SetCustomCameraRect(Camera camera)
        {
            if (Instance.isNotch)
            {
                camera.rect = Instance.iPhoneXCameraRect;
            }
        }

        public static bool IsUknowniPhoneX()
        {
            if(Screen.width == 2436 && Screen.height == 1125)
            {
                // iPhone XS
                return true;
            }
            else if(Screen.width == 2688 && Screen.height == 1242)
            {
                // iPhone XS Max
                return true;
            }
            else if(Screen.width == 1792 && Screen.height == 828)
            {
                // iPhone XR
                return true;
            }

            return false;
        }
    }
}
