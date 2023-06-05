package com.bagelcode.v3;

import android.app.Activity;
import android.app.Application;
import android.net.Uri;
import android.os.Bundle;

import com.adjust.sdk.Adjust;
import com.adjust.sdk.AdjustAttribution;
import com.adjust.sdk.AdjustConfig;
import com.adjust.sdk.OnAttributionChangedListener;
import com.adjust.sdk.OnDeeplinkResponseListener;

public class GlobalApplication extends Application {
  @Override
  public void onCreate() {
    super.onCreate();

    String appToken = getString(R.string.adjust_app_token);
    String environment = getString(R.string.adjust_env);
    String adjEnv;
    if(environment.equals("production")) {
      adjEnv = AdjustConfig.ENVIRONMENT_PRODUCTION;
    }
    else {
      adjEnv = AdjustConfig.ENVIRONMENT_SANDBOX;
    }
    int adjustAppSecret = Integer.parseInt(getString(R.string.adjust_app_secret));
    int adjustInfo1 = Integer.parseInt(getString(R.string.adjust_info1));
    int adjustInfo2 = Integer.parseInt(getString(R.string.adjust_info2));
    int adjustInfo3 = Integer.parseInt(getString(R.string.adjust_info3));
    int adjustInfo4 = Integer.parseInt(getString(R.string.adjust_info4));

    AdjustConfig config = new AdjustConfig(this, appToken, adjEnv);
    config.setAppSecret(adjustAppSecret, adjustInfo1, adjustInfo2, adjustInfo3, adjustInfo4);

    config.setOnAttributionChangedListener(new OnAttributionChangedListener() {
      @Override
      public void onAttributionChanged(AdjustAttribution attribution) {
        UnityPlayerActivity.respondAttributionCallBack(attribution);
      }
    });

    // Evaluate the deeplink to be launched.
    config.setOnDeeplinkResponseListener(new OnDeeplinkResponseListener() {
      @Override
      public boolean launchReceivedDeeplink(Uri deeplink) {
//          UnityPlayerActivity.HandleDeepLink(deeplink); // Check this line affects
          return true;
        }
    });

    Adjust.onCreate(config);

    registerActivityLifecycleCallbacks(new AdjustLifecycleCallbacks());
  }

  private static final class AdjustLifecycleCallbacks implements ActivityLifecycleCallbacks {
    @Override
    public void onActivityCreated(Activity activity, Bundle bundle) {}

    @Override
    public void onActivityStarted(Activity activity) {}

    @Override
    public void onActivityResumed(Activity activity) {
      Adjust.onResume();
    }

    @Override
    public void onActivityPaused(Activity activity) {
      Adjust.onPause();
    }

    @Override
    public void onActivityStopped(Activity activity) {}

    @Override
    public void onActivitySaveInstanceState(Activity activity, Bundle bundle) {}

    @Override
    public void onActivityDestroyed(Activity activity) {}
  }
}
