using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/NativeHelper")]
public class InitializeVideoAdsController : ActionTask
{
    protected override string info
    {
        get
        {
            return "Initialize VideoAdsController";
        }
    }

    protected override void OnExecute()
    {
//#if !UNITY_EDITOR
//        VideoAdsController.Instance.Initialize();
//#endif
        EndAction();
    }
}

}
