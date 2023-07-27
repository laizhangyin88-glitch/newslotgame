using UnityEngine;
using System.Collections.Generic;

namespace BagelCode
{

public class VideoAdsControllerUnity : IVideoAdsController
{
    public void Initialize(string name)
    {

    }

    public void LoadPlacement(string placement)
    {
        //Nothing to do.
    }

    public bool IsVideoAdsAvailable(string placement)
    {
        return true;
    }

    public void ShowRewardedVideo(string placement, string methodName)
    {
        VideoAdsController.Instance.OnVideoAdsEnd("Rewarded");
        VideoAdsController.Instance.OnVideoAdsEnd("Failed");
    }

    public long GetPlacementReward(string placement)
    {
        return 0L;
    }

    public void IntegrationTest()
    {
        Debug.LogError("Nothing to do");
    }

}

}
