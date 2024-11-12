using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/NativeHelper")]
public class SetSupportPageUrl : ActionTask
{
    protected override string info { get { return string.Format("Set Support Page Url"); } }
    protected override void OnExecute()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        string supportPageUrl = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "/values/misc/SUPPORT_PAGE_URL").value;
        string supportPageUrlWithPayload = UrlBuildUtils.GetHelpCenterUrl(supportPageUrl);
        NativeHelper.Instance.SetSupportPageUrl(supportPageUrlWithPayload);
#endif
        
        EndAction();
    }
}

}
