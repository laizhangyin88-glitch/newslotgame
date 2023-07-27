#if UNITY_ANDROID && PLATFORM_AMAZON && !UNITY_EDITOR

using UnityEngine;
using System.Collections.Generic;

namespace BagelCode
{

public class VideoAdsControllerAmazon : IVideoAdsController
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
        return false;
    }

    public void ShowRewardedVideo(string placement, string methodName)
    {
        VideoAdsController.Instance.OnVideoAdsEnd("");
    }

    public long GetPlacementReward(string placement)
    {
        return 0L;
    }

    public void IntegrationTest()
    {
        Debug.LogError("TODO(amazon)");
    }
}

}

#endif
