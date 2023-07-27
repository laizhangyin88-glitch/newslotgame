#if UNITY_WSA && !UNITY_EDITOR

using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace BagelCode
{

public class VideoAdsControllerWindows : IVideoAdsController
{
    [DllImport ("__Internal")]
    private static extern void initializeVideoAdsController([MarshalAs(UnmanagedType.LPWStr)]string name);
    public void Initialize(string name)
    {
        initializeVideoAdsController(name);
    }

    [DllImport("__Internal")]
    private static extern void loadPlacement([MarshalAs(UnmanagedType.LPWStr)]string placement);
    public void LoadPlacement(string placement)
    {
        loadPlacement(placement);
    }

    [DllImport("__Internal")]
    private static extern bool isVideoAdsAvailable([MarshalAs(UnmanagedType.LPWStr)]string placement);
    public bool IsVideoAdsAvailable(string placement)
    {
        return isVideoAdsAvailable(placement);
    }

    [DllImport("__Internal")]
    private static extern void showRewardedVideo([MarshalAs(UnmanagedType.LPWStr)]string placement, [MarshalAs(UnmanagedType.LPWStr)]string methodName);
    public void ShowRewardedVideo(string placement, string methodName)
    {
        showRewardedVideo(placement, methodName);
    }

    [DllImport("__Internal")]
    private static extern long getPlacementReward([MarshalAs(UnmanagedType.LPWStr)]string placement);
    public long GetPlacementReward(string placement)
    {
        return getPlacementReward(placement);
    }

    public void IntegrationTest()
    {
        Debug.LogError("TODO(Windows)");
    }

}

}

#endif
