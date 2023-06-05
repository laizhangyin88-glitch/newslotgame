package com.bagelcode.v3;

import android.util.Log;

import com.ironsource.mediationsdk.IronSource;
import com.ironsource.mediationsdk.integration.IntegrationHelper;
import com.ironsource.mediationsdk.logger.IronSourceError;
import com.ironsource.mediationsdk.logger.IronSourceLogger;
import com.ironsource.mediationsdk.logger.LogListener;
import com.ironsource.mediationsdk.model.Placement;
import com.ironsource.mediationsdk.sdk.RewardedVideoListener;
// import com.ironsource.adqualitysdk.sdk.IronSourceAdQuality;
// import com.ironsource.adqualitysdk.sdk.ISAdQualityConfig;
// import com.ironsource.adqualitysdk.sdk.ISAdQualityLogLevel;
// import com.ironsource.adqualitysdk.sdk.ISAdQualityInitListener;
// import com.ironsource.adqualitysdk.sdk.ISAdQualityInitError;
import com.unity3d.player.UnityPlayer;

/**
 * Created by ruman on 13/06/2017.
 */

@SuppressWarnings("unused")
public class VideoAdsController {
  private static final String TAG = "VideoAdsController";
  private String unityObjectName = null;
  private String unityMethodName = null;

  private static VideoAdsController _instance;
  public static VideoAdsController Instance()
  {
    if (_instance == null)
      _instance = new VideoAdsController();

    return _instance;
  }

  public void Init(String name, boolean devBuild)
  {
    UnityPlayerActivity currentInstance = UnityPlayerActivity.CurrentInstance();
    if (currentInstance == null) {
      return ;
    }

    unityObjectName = name;

    IronSource.setConsent(true);
    IronSource.setRewardedVideoListener(rewardedVideoListener);
    IronSource.setLogListener(logListener);

    String appKey = currentInstance.getResources().getString(R.string.ironsource_app_key);
    // Log.d(TAG, appKey);

    IronSource.init(currentInstance, appKey, IronSource.AD_UNIT.REWARDED_VIDEO);    
//    IntegrationHelper.validateIntegration(UnityPlayerActivity.CurrentInstance());
    
    // Initialize Ironsource Ad Quality
    // ISAdQualityConfig.Builder adQualityConfigBuilder = new ISAdQualityConfig.Builder().setAdQualityInitListener(new ISAdQualityInitListener() { 

    //     @Override 
    //     public void adQualitySdkInitSuccess() { 
    //         Log.d(TAG, "adQualitySdkInitSuccess"); 
    //     } 

    //     @Override 
    //     public void adQualitySdkInitFailed(ISAdQualityInitError error, String message) { 
    //         Log.d(TAG, "adQualitySdkInitFailed " + error + " message: " + message); 
    //     }
    // });

    // // There are 5 different log levels:
    // // ERROR, WARNING, INFO, DEBUG, VERBOSE
    // if(devBuild)
    // {
    //     adQualityConfigBuilder.setTestMode(true); 
    //     adQualityConfigBuilder.setLogLevel(ISAdQualityLogLevel.VERBOSE);
    // }
    // else
    // {
    //     // The default is false - set to true only to test your Ad Quality integration
    //     adQualityConfigBuilder.setTestMode(false); 

    //     // The default is INFO
    //     adQualityConfigBuilder.setLogLevel(ISAdQualityLogLevel.INFO);
    // }

    // ISAdQualityConfig adQualityConfig = adQualityConfigBuilder.build();
    // IronSourceAdQuality.getInstance().initialize(currentInstance, appKey, adQualityConfig);
  }

  @SuppressWarnings("unused")
  public static void Initialize(String name, boolean devBuild) {
    VideoAdsController.Instance().Init(name, devBuild);
  }

  @SuppressWarnings("unused")
  public static boolean IsVideoAdsAvailable(String placement) {
    Placement info = IronSource.getRewardedVideoPlacementInfo(placement);
    if (info != null) {
      return IronSource.isRewardedVideoAvailable() && !IronSource.isRewardedVideoPlacementCapped(placement);
    }
    else
    {
      return false;
    }
  }

  @SuppressWarnings("unused")
  public static void ShowRewardedVideo(String placement, String methodName) {
    // Log.d(TAG, "isRewardedAvailable : " + IronSource.isRewardedVideoAvailable());
    // Log.d(TAG, "isCapped : " + IronSource.isRewardedVideoPlacementCapped(placement));

    VideoAdsController.Instance().unityMethodName = methodName;
    IronSource.showRewardedVideo(placement);
  }

  @SuppressWarnings("unused")
  public static long GetPlacementReward(String placement) {
    Placement placementInfo = IronSource.getRewardedVideoPlacementInfo(placement);
// Null can be returned instead of a placement if the placementName is not valid.
    if (placementInfo != null) {
      int rewardAmount = placementInfo.getRewardAmount();
      return rewardAmount;
    } else {
      return 0;
    }
  }

  @SuppressWarnings("unused")
  public static void VideoAdsValidateIntegration() {
    IntegrationHelper.validateIntegration(UnityPlayerActivity.CurrentInstance());
  }

  LogListener logListener = new LogListener() {
    @Override
    public void onLog(IronSourceLogger.IronSourceTag ironSourceTag, String s, int i) {
//       Log.d(TAG, ironSourceTag.name() + " " + s + " " + i);
    }
  };

  RewardedVideoListener rewardedVideoListener = new RewardedVideoListener() {
    /**
     * Invoked when the RewardedVideo ad view has opened.
     * Your Activity will lose focus. Please avoid performing heavy
     * tasks till the video ad will be closed.
     */
    @Override
    public void onRewardedVideoAdOpened() {
//       Log.d(TAG, "onRewardedVideoAdOpened");
    }

    /*Invoked when the RewardedVideo ad view is about to be closed.
    Your activity will now regain its focus.*/
    @Override
    public void onRewardedVideoAdClosed() {
       UnityPlayer.UnitySendMessage(unityObjectName, unityMethodName, "");
//       Log.d(TAG, "onRewardedVideoAdClosed");
    }

    /**
     * Invoked when there is a change in the ad availability status.
     *
     * @param - available - value will change to true when rewarded videos are *available.
     *          You can then show the video by calling showRewardedVideo().
     *          Value will change to false when no videos are available.
     */
    @Override
    public void onRewardedVideoAvailabilityChanged(boolean b) {
      //Change the in-app 'Traffic Driver' state according to availability.
      UnityPlayer.UnitySendMessage(unityObjectName, "OnAvailabilityChanged", Boolean.toString(b));
//       Log.d(TAG, "onRewardedVideoAvailabilityChanged to " + b);
    }

    /**
     * Invoked when the video ad starts playing.
     */
    @Override
    public void onRewardedVideoAdStarted() {
//       Log.d(TAG, "onRewardVideoAdStarted");
    }
    /*Invoked when the video ad finishes playing.*/

    @Override
    public void onRewardedVideoAdEnded() {
//       Log.d(TAG, "onRewardedVideoAdEnded");
    }

    /**
     * Invoked when the user completed the video and should be rewarded.
     * If using server-to-server callbacks you may ignore this events and wait *for the callback from the ironSource server.
     *
     * @param - placement - the Placement the user completed a video from.
     */
    @Override
    public void onRewardedVideoAdRewarded(Placement placement) {
//       Log.d(TAG, "onRewardedVideoAdRewarded");
//      String placementName = placement.getPlacementName();
//      String rewardName = placement.getRewardName();
//      int rewardAmount = placement.getRewardAmount();
      UnityPlayer.UnitySendMessage(unityObjectName, unityMethodName, "");
    }

    /* Invoked when RewardedVideo call to show a rewarded video has failed
     * IronSourceError contains the reason for the failure.
     */
    @Override
    public void onRewardedVideoAdShowFailed(IronSourceError ironSourceError) {
//       Log.d(TAG, "onRewardedVideoAdShowFailed " + ironSourceError.getErrorMessage());
       UnityPlayer.UnitySendMessage(unityObjectName, unityMethodName, "");
    }

    /*Invoked when the end user clicked on the RewardedVideo ad
    */
    @Override
    public void onRewardedVideoAdClicked(Placement placement) {
//      Log.d(TAG, "onRewardedVideoAdClicked");
    }
  };

}
