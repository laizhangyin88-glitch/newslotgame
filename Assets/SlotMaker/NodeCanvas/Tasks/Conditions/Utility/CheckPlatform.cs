using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Condition
{

[Category("★ SlotMaker/Utility")]
public class CheckPlatform : ConditionTask
{
    public TargetPlatform platform;

    protected override string info { get { return "Check( " + platform.ToString() + " )"; } }

    protected override bool OnCheck()
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
}

}
