using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SlotMaker;

namespace BagelCode
{
    public class OrientationUtils : MonoWeakSingleton<OrientationUtils>
    {
        public ScreenOrientation contentOrientation;

        public bool PossibleChangeOrientation()
        {
#if UNITY_WSA || UNITY_WEBGL || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || FIXABLE_LANDSCAPE
            return false;
#else
    #if UNITY_IOS && !UNITY_EDITOR
            if( UnityEngine.iOS.Device.iosAppOnMac )
            {
                return false;
            }
    #endif
            return true;
#endif
        }
    }
}
