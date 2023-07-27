using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/NativeHelper")]
public class AfterLoginInitializeVideoAdsController : ActionTask
{
    protected override string info
    {
        get
        {
            return "Initialize VideoAdsController (After Login)";
        }
    }

    protected override void OnExecute()
    {
#if UNITY_WSA && !UNITY_EDITOR
        var videoAdsEnabled = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "values/misc/VIDEO_ADS_ENABLED");

        if(videoAdsEnabled.value)
        {
            if(ApplicationSettings.LogTest())
                Debug.LogError("AfterLoginInitializeVideoAdsController");

            VideoAdsController.Instance.InitializeAfterLogin();

            LoadPlacement();

        }
#endif
        EndAction();
    }

    private void LoadPlacement()
    {
        var placementsBB = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "videoAdsPlacementNames");

        if(placementsBB != null && placementsBB.value != null)
        {
            var variables = placementsBB.value.variables;
            // Variables routine.
            foreach( var placementData in variables )
            {
                var placement = (string)placementData.Value.value;

                if( !string.IsNullOrEmpty(placement) )
                {
                    if(ApplicationSettings.LogTest())
                        Debug.LogError(string.Format("{0} : {1}", placementData.Key, placement));

                    VideoAdsController.Instance.LoadPlacement(placement);
                }


            }

        }
    }
}

}
