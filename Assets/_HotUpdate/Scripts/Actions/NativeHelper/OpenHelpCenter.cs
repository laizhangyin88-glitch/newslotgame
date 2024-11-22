using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using SlotMaker.Json;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/NativeHelper")]
public class OpenHelpCenter : ActionTask
{
    protected override string info { get { return string.Format("Open Help Center"); } }
    protected override void OnExecute()
    {
        string supportPageUrl = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "/values/misc/SUPPORT_PAGE_URL").value;

#if UNITY_WEBGL && !UNITY_EDITOR
        NativeHelper.Instance.OpenUrl(UrlBuildUtils.GetHelpCenterUrl(supportPageUrl));
#else
        Application.OpenURL(UrlBuildUtils.GetHelpCenterUrl(supportPageUrl));
#endif

        EndAction();
    }
}

}
