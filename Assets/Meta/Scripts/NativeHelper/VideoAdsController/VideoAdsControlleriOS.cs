#if UNITY_IPHONE && !UNITY_EDITOR

using System.Runtime.InteropServices;

using UnityEngine;
using System.Collections.Generic;

namespace BagelCode
{

    public class VideoAdsControlleriOS : IVideoAdsController
    {
        //[DllImport("__Internal")]
        //private static extern void _initializeVideoAdsController(string name, bool devBuild);
        public void Initialize(string name)
        {
            //            bool devBuild = false;
            //#if DEV
            //        devBuild = true;
            //#endif
            //            _initializeVideoAdsController(name, devBuild);
        }

        public void LoadPlacement(string placement)
        {
            //Nothing to do.
        }

        //[DllImport("__Internal")]
        //private static extern bool _isAdPlayable(string placement);
        public bool IsVideoAdsAvailable(string placement)
        {
            //return _isAdPlayable(placement);
            return true;
        }

        //[DllImport("__Internal")]
        //private static extern void _showRewardedVideo(string placement, string methodName);
        public void ShowRewardedVideo(string placement, string methodName)
        {
            //_showRewardedVideo(placement, methodName);
            VideoAdsController.Instance.OnVideoAdsEnd("Rewarded");
            VideoAdsController.Instance.OnVideoAdsEnd("Failed");
        }

        //[DllImport("__Internal")]
        //private static extern long _getPlcaementReward(string placement);
        public long GetPlacementReward(string placement)
        {
            //return _getPlcaementReward(placement);
            return 0L;
        }

        //[DllImport("__Internal")]
        //private static extern void _integrationADS();
        public void IntegrationTest()
        {
            //_integrationADS();
        }
    }

}

#endif
