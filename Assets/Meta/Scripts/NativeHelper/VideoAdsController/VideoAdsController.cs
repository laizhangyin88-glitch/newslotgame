using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public class VideoAdsController : MonoWeakSingleton<VideoAdsController>
    {
        public const string ON_VIDEO_ADS_REWARDED = "OnVideoAdsRewarded";

        private IVideoAdsController delegator;

        public bool ShowVideoAds(string placementKey, GameObject caller = null)
        {
            if (Instance.IsVideoAdsAvailable(placementKey))
            {
                if (ApplicationSettings.LogTest())
                    Debug.LogError("Available Video " + placementKey);

                Instance.ShowRewardedVideo(placementKey,
                () =>
                {
                    if (caller == null)
                        EventSender.SendGlobalEvent(ON_VIDEO_ADS_REWARDED);
                    else
                        EventSender.SendEvent(caller, ON_VIDEO_ADS_REWARDED);
                });

                return true;
            }
            else
            {
                if (ApplicationSettings.LogTest())
                    Debug.LogError("Not Available Video " + placementKey);

                return false;
            }
        }

        public void Awake()
        {
//#if UNITY_IPHONE && !UNITY_EDITOR
//            delegator = new VideoAdsControlleriOS();
//#elif UNITY_ANDROID && PLATFORM_AMAZON && !UNITY_EDITOR
//            delegator = new VideoAdsControllerAmazon();
//#elif UNITY_ANDROID && !UNITY_EDITOR
//            delegator = new VideoAdsControllerAndroid();
//#elif UNITY_WSA && !UNITY_EDITOR
//            delegator = new VideoAdsControllerWindows();
//#elif UNITY_STANDALONE_WIN && !UNITY_EDITOR
//            delegator = new VideoAdsControllerGameroom();
//#elif UNITY_WEBGL && !UNITY_EDITOR
//            delegator = new VideoAdsControllerCanvas();
//#else
            delegator = new VideoAdsControllerUnity();
//#endif
        }

        public void Initialize()
        {
#if !UNITY_WSA
            delegator.Initialize(gameObject.name);
#endif
        }

        public void InitializeAfterLogin()
        {
#if UNITY_WSA
            delegator.Initialize(gameObject.name);
#endif
        }

        public void LoadPlacement(string placement)
        {
            delegator.LoadPlacement(placement);
        }

        public bool IsVideoAdsAvailable(string placement)
        {
            if (string.IsNullOrEmpty(placement))
                return false;
            return delegator.IsVideoAdsAvailable(placement);
        }

        private System.Action onVideoAdsCallback;
        public void ShowRewardedVideo(string placement, System.Action callback)
        {
#if UNITY_WSA && !UNITY_EDITOR
            // Stop Unity music and sfx
            SlotMaker.GSManager.Instance.MusicVolume = 0f;
            SlotMaker.GSManager.Instance.SfxVolume = 0f;
#endif

            onVideoAdsCallback = callback;

            delegator.ShowRewardedVideo(placement, "OnVideoAdsEnd");
        }

        public long GetPlacementReward(string placement)
        {
            return delegator.GetPlacementReward(placement);
        }

        public void OnVideoAdsEnd(string data)
        {
#if UNITY_WSA && !UNITY_EDITOR
            // Resume Unity music and sfx
            SlotMaker.GSManager.Instance.MusicVolume = PlayerPrefs.GetFloat("MUTE_MUSIC", 1);
            SlotMaker.GSManager.Instance.SfxVolume = PlayerPrefs.GetFloat("MUTE_SFX", 1);
#endif

            NativeHelper.Instance.SetIdleTimerDisabled(true);

            if (IAMUtils.OnVideoAdsEnd(onVideoAdsCallback))
            {
                if (onVideoAdsCallback != null)
                    onVideoAdsCallback = null;
            }
            else
            {
                if (onVideoAdsCallback != null)
                {
                    onVideoAdsCallback();
                    onVideoAdsCallback = null;
                }
            }
        }

        public void OnAvailabilityChanged(string state)
        {
            // todo : use passive event?
        }

        public void IntegrationTest()
        {
            delegator.IntegrationTest();
        }
    }
}
