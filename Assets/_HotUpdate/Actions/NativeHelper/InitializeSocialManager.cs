using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/NativeHelper")]
[Description("!!空实现")]
public class InitializeSocialManager : ActionTask
{
    protected override string info
    {
        get
        { 
            return "Initialize SocialManager";
        }
    }

    protected override void OnExecute()
    {   
//#if !UNITY_EDITOR
//        SocialManager.Instance.Initialize();
//#endif
        EndAction();
    }
}

}
