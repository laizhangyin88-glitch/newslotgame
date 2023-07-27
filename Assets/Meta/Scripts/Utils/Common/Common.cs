using System.Collections.Generic;
using ParadoxNotion.Services;
using UnityEngine;
using SlotMaker;

using static BagelCode.Common.EColor;

namespace BagelCode
{
    public class Common
    {
        public static T GetEnumTypeByString<T>(string str)
        {
            return (T)System.Enum.Parse(typeof(T), str);
        }

        // Examples: https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/StyledText.html
        public enum EColor
        {
            NONE = 0,
            CYAN,
            BLACK,
            BLUE,
            BROWN,
            DARKBLUE,
            MAGENTA,
            GREEN,
            GREY,
            LIGHTBLUE,
            LIME,
            MAROON,
            NAVY,
            OLIVE,
            ORANGE,
            PURPLE,
            RED,
            SILVER,
            TEAL,
            WHITE,
            YELLOW,
        }

        public static bool CheckPlatform(TargetPlatform platform)
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
                return true;
            else
                return false;
        }

        public static readonly EColor[] DEBUG_LOG_COLOR_SET = { WHITE, CYAN, LIGHTBLUE, ORANGE, TEAL, OLIVE, RED, ORANGE, YELLOW };

        //

        #region Event System

        public static MessageRouter GetOrAddMessageRouter(GameObject obj)
        {
            if (obj == null) return null;

            var router = obj.GetComponent<MessageRouter>();
            if (router == null)
                router = obj.AddComponent<MessageRouter>();

            return router;
        }

        public static MessageRouter GetOrAddMessageRouter(MonoBehaviour listener)
        {
            if (listener is MessageRouter)
                return listener as MessageRouter;

            var router = listener.GetComponent<MessageRouter>();
            if (router == null)
                router = listener.gameObject.AddComponent<MessageRouter>();

            return router;
        }

        #endregion
    }
}
