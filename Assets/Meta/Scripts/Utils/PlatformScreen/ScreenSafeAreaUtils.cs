using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace BagelCode
{
    public class ScreenSafeAreaUtils
    {

#if UNITY_EDITOR
        private static Rect[] editorIPhoneX = new Rect[]
        {
            new Rect (0f, 102f / 2436f, 1f, 2202f / 2436f),  // Portrait
            new Rect (132f / 2436f, 63f / 1125f, 2172f / 2436f, 1062f / 1125f)  // Landscape
        };
#endif

        public static bool IsNotch()
        {
#if UNITY_IOS
            switch(Device.generation)
            {
                case DeviceGeneration.iPhoneX:
                case DeviceGeneration.iPhoneXS:
                case DeviceGeneration.iPhoneXSMax:
                case DeviceGeneration.iPhoneXR:
                case DeviceGeneration.iPhone11:
                case DeviceGeneration.iPhone11Pro:
                case DeviceGeneration.iPhone11ProMax:
                // case DeviceGeneration.iPhoneSE2Gen: not notch
                case DeviceGeneration.iPhone12Mini:
                case DeviceGeneration.iPhone12:
                case DeviceGeneration.iPhone12Pro:
                case DeviceGeneration.iPhone12ProMax:
                case DeviceGeneration.iPhone13:
                case DeviceGeneration.iPhone13Mini:
                case DeviceGeneration.iPhone13Pro:
                case DeviceGeneration.iPhone13ProMax:
                    return true;
                case DeviceGeneration.iPhoneUnknown:
                    //return IsUknowniPhoneX();
                    return true;
            }
#endif
            return false;
        }
        public static bool IsUknowniPhoneX()
        {
            if((Screen.width == 2436 && Screen.height == 1125) || (Screen.width == 1125 && Screen.height == 2436))
            {
                // iPhone XS
                return true;
            }
            else if((Screen.width == 2688 && Screen.height == 1242) || (Screen.width == 1242 && Screen.height == 2688))
            {
                // iPhone XS Max
                return true;
            }
            else if((Screen.width == 1792 && Screen.height == 828) || (Screen.width == 828 && Screen.height == 1792))
            {
                // iPhone XR
                return true;
            }
            else if ((Screen.width == 2532 && Screen.height == 1170) || (Screen.width == 1170 && Screen.height == 2532))
            {
                // iPhone 12 Pro
                return true;
            }
            else if ((Screen.width == 2778 && Screen.height == 1284) || (Screen.width == 1284 && Screen.height == 2778))
            {
                // iPhone 12 Pro Max
                return true;
            }
            else if(((float)Screen.width/(float)Screen.height > 2f) || ((float)Screen.height / (float)Screen.width > 2f))
            {
                return true;
            }
            else
            {
                Debug.LogError(string.Format("Screen w = {0} h = {1}", Screen.width, Screen.height));
            }

            return false;
        }

        public static Rect GetSafeArea()
        {
#if UNITY_EDITOR
            Rect safeArea = Screen.safeArea;

    #if USE_SAFE_AREA
            Rect nsa = new Rect (0, 0, Screen.width, Screen.height);

            if (Screen.height > Screen.width)  // Portrait
                nsa = editorIPhoneX[0];
            else
                nsa = editorIPhoneX[1];

            safeArea = new Rect (Screen.width * nsa.x, Screen.height * nsa.y, Screen.width * nsa.width, Screen.height * nsa.height);

            if (Screen.height > Screen.width)  // Portrait
            {
                safeArea.x = 0;
                safeArea.width = Screen.width;
            }
            else
            {
                safeArea.y = 0;
                safeArea.height = Screen.height;
            }
    #endif
            return safeArea;
#else
    #if UNITY_IOS
            Rect safeArea = Screen.safeArea;
            if (Screen.height > Screen.width)  // Portrait
            {
                safeArea.x = 0;
                safeArea.width = Screen.width;
            }
            else
            {
                safeArea.y = 0;
                safeArea.height = Screen.height;
            }
            return safeArea;
    #else
            return new Rect(0, 0, Screen.width, Screen.height);
    #endif
#endif
        }
    }
}
