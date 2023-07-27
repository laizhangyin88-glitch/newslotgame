#if UNITY_WEBGL && !UNITY_EDITOR

using UnityEngine;
using System.Collections.Generic;

namespace BagelCode
{

public class VideoAdsControllerCanvas : IVideoAdsController
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
        // Nothing to do
    }
}

}

#endif
