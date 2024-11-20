using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/NativeHelper")]
public class InitializeNative : ActionTask
{
    protected override string info
    {
        get
        {
            return "Initialize Native Component";
        }
    }

    protected override void OnExecute()
    {
#if !UNITY_EDITOR
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    #if DEV
        Input.multiTouchEnabled = true;
    #else
        Input.multiTouchEnabled = false;
    #endif
        NativeHelper.Instance.Initialize();

    #if UNITY_IOS
        IDFAHelper.Instance.Initialize();
    #endif
#endif

        EndAction();
    }
}

}
