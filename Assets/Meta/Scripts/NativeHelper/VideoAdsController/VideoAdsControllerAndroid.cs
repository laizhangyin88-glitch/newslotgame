#if UNITY_ANDROID && !UNITY_EDITOR

using UnityEngine;
using System.Collections.Generic;

namespace BagelCode
{

public class VideoAdsControllerAndroid : IVideoAdsController
{
    private AndroidJavaClass ajc = new AndroidJavaClass("com.bagelcode.v3.VideoAdsController");

    public void Initialize(string name)
    {
        bool devBuild = false;
#if DEV
        devBuild = true;
#endif
        ajc.CallStatic("Initialize", name, devBuild);
    }

    public void LoadPlacement(string placement)
    {
        //Nothing to do.
    }

    public bool IsVideoAdsAvailable(string placement)
    {
        return ajc.CallStatic<bool>("IsVideoAdsAvailable", placement);
    }

    public void ShowRewardedVideo(string placement, string methodName)
    {
        ajc.CallStatic("ShowRewardedVideo", placement, methodName);
    }

    public long GetPlacementReward(string placement)
    {
        return ajc.CallStatic<long>("GetPlacementReward", placement);
    }

    public void IntegrationTest()
    {
        ajc.CallStatic("VideoAdsValidateIntegration");
    }

}

}

#endif
