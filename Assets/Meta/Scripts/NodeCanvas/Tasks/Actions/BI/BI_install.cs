using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_install : ActionTask
{
    protected override void OnExecute()
    {
        var isFirstInstall = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "/deviceFirstInstalled");

        if(isFirstInstall == null)
        {
        	EndAction();
        	return;
        }

        if(isFirstInstall.value)
        {
#if UNITY_ANDROID && !PLATFORM_AMAZON && !UNITY_EDITOR
            NativeHelper.Instance.GetReferrerUrl(
                (referrerUrl) =>
                {
                    BiEventUtils.SendClientInstall(referrerUrl);
                }
            );
#else
            BiEventUtils.SendClientInstall(null);
#endif
        }

        EndAction();
    }
}

}
