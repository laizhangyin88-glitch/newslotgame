using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using UnityEngine;

namespace BagelCode {
    public class PlatformEditorToggle : MonoBehaviour {
        public bool isEditorActivate;

        public TargetPlatform platform;
        public bool platformActivate;

        private void Awake() {
#if UNITY_EDITOR
            gameObject.SetActive(isEditorActivate);
            return;
#endif

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
                gameObject.SetActive(platformActivate);
            else
                gameObject.SetActive(!platformActivate);
        }
    }
}
