using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public class PlatformToggle : MonoBehaviour
    {
        public TargetPlatform platform;
        public List<TargetPlatform> platforms;
        public bool activate;

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
                gameObject.SetActive(activate);
            else
                gameObject.SetActive(!activate);
        }
    }
}
