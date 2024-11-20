using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/NativeHelper")]
public class ShowRewardedVideoAds : ActionTask
{
    public BBParameter<string> placement;
    protected override string info
    {
        get
        {
            return "Show Rewarded Video";
        }
    }

    protected override void OnExecute()
    {
        if (VideoAdsController.Instance.IsVideoAdsAvailable(placement.value))
        {
            if(ApplicationSettings.LogTest())
                Debug.LogError("Available Video " + placement.value);

            VideoAdsController.Instance.ShowRewardedVideo(placement.value,
            () =>
            {
                Debug.Log("OnVideoAdsRewarded");
                QuickSendEvent("OnVideoAdsRewarded");
            });

            EndAction(true);
        }
        else
        {
            if(ApplicationSettings.LogTest())
                Debug.LogError("Not Available Video " + placement.value);

            EndAction(false);
        }
    }

    private void QuickSendEvent(string eventName)
    {
        GraphOwner owner = null;

        if(ownerSystem != null)
            owner = ownerSystem.agent.GetComponent<GraphOwner>();

        if(owner != null)
        {
            owner.SendEvent(eventName);
        }
        else
        {
            SendEvent(eventName);
        }
    }
}

}
