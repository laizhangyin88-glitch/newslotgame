package com.bagelcode.v3;

import android.app.Activity;
import android.app.AppOpsManager;
import android.app.PictureInPictureParams;
import android.content.ClipData;
import android.content.ClipboardManager;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.content.res.Configuration;
import android.graphics.Bitmap;
import android.graphics.PixelFormat;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.os.Environment;
import android.os.StatFs;
import android.provider.MediaStore;
import android.provider.Settings;
import androidx.annotation.Nullable;
import android.text.TextUtils;
import android.util.Base64;
import android.util.Log;
import android.util.Rational;
import android.view.KeyEvent;
import android.view.MotionEvent;
import android.view.Window;
import android.view.WindowManager;

import com.adjust.sdk.ActivityHandler;
import com.adjust.sdk.Adjust;
import com.adjust.sdk.AdjustAttribution;
import com.adjust.sdk.AdjustEvent;

// import com.bagelcode.v3.iab.IabHelper;
// import com.bagelcode.v3.iab.IabResult;
// import com.bagelcode.v3.iab.Inventory;
// import com.bagelcode.v3.iab.Purchase;
import com.bagelcode.v3.push.PushManager;

import com.android.billingclient.api.BillingClient;
import com.android.billingclient.api.Purchase;
import com.android.billingclient.api.SkuDetails;
import com.bagelcode.v3.billing.BillingHelper;

import com.bagelcode.v3.onesignal.NotificationOpenedHandler;

import com.facebook.CallbackManager;
import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.GoogleApiAvailability;
import com.google.firebase.FirebaseApp;
import com.google.firebase.crashlytics.FirebaseCrashlytics;
import com.google.firebase.analytics.FirebaseAnalytics;
import com.ironsource.mediationsdk.IronSource;
import com.theartofdev.edmodo.cropper.CropImage;
import com.theartofdev.edmodo.cropper.CropImageView;
import com.unity3d.player.UnityPlayer;
import com.onesignal.OneSignal;
import com.onesignal.OSNotificationReceivedEvent;
import com.onesignal.OSDeviceState;
import com.onesignal.OSSubscriptionObserver;
import com.onesignal.OSSubscriptionStateChanges;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.ByteArrayOutputStream;
import java.lang.Runnable;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.security.Signature;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Dictionary;
import java.util.List;

import com.surveymonkey.surveymonkeyandroidsdk.SurveyMonkey;

import com.android.installreferrer.api.InstallReferrerClient;
import com.android.installreferrer.api.InstallReferrerStateListener;
import com.android.installreferrer.api.ReferrerDetails;

public class UnityPlayerActivity extends Activity implements OSSubscriptionObserver
                                                              , BillingHelper.BillingHelperListener
{
  private static final String TAG = "UnityPlayerActivity";

  static final int PURCHASE_REQUEST                 = 10001;
  static final int PLAY_SERVICES_RESOLUTION_REQUEST = 10002;
  static final int SURVEY_MONKEY_REQUEST            = 10003;

  static final float PIP_RATIO_MIN = 0.42f;
  static final float PIP_RATIO_MAX = 2.3f;

  protected static final String PREFS_FILE = "purchase.xml";
  protected static final String PREFS_PURCHASE = "purchase";

  private static String unityObjectName = null;
  private static String adjustUnityObjectName = null;
  private static String adjustAttributionCallBack = null;
  private static String profileImageUnityCallBack = null;
  private static String referrerUnityCallBack = null;
  private static int profileWidth;
  private static int profileHeight;
  private static String iABUnityCallBack = null;
  private static Boolean isAdjustAttributionCallBackAlreadyCalled = false;

  public FirebaseAnalytics mFirebaseAnalytics = null;
  protected InstallReferrerClient referrerClient;

  private static Boolean enablePIP = false;
  private static Boolean triggerPIPMode = false;
  private static Boolean isForeground = false;
  private static String pipUnityCallback = "OnChangePipMode";
  public static Boolean getIsForeground() {
    return isForeground;
  }

  private static UnityPlayerActivity _mCurrentInstance = null;

  public static UnityPlayerActivity CurrentInstance() {
    return _mCurrentInstance;
  }

  public UnityPlayerActivity() {
    // Following condition is not expected to be true, because launchMode is set to singleTask.
    // Galaxy Nexus 4.3 with Android 4.3, however, it does happen.
    if (_mCurrentInstance != null) {
      // Finish the previous unity player.
      _mCurrentInstance.mUnityPlayer.quit();
    }
    _mCurrentInstance = this;
  }

  protected UnityPlayer mUnityPlayer; // don't change the name of this variable; referenced from native code

  // protected IabHelper oldHelper;

  protected BillingHelper mBillingHelper;

  protected DeviceUuidFactory mDeviceUuidFactory;

  public SurveyMonkey smSDKInstance = new SurveyMonkey();

  /**
   * Check the device to make sure it has the Google Play Services APK. If
   * it doesn't, display a dialog that allows users to download the APK from
   * the Google Play Store or enable it in the device's system settings.
   */
  private boolean checkPlayServices() {
    GoogleApiAvailability apiAvailability = GoogleApiAvailability.getInstance();
    int resultCode = apiAvailability.isGooglePlayServicesAvailable(this);
    if (resultCode != ConnectionResult.SUCCESS) {
      if (apiAvailability.isUserResolvableError(resultCode)) {
        apiAvailability.getErrorDialog(this, resultCode, PLAY_SERVICES_RESOLUTION_REQUEST)
          .show();
      } else {
        Log.i(TAG, "This device is not supported.");
        finish();
      }
      return false;
    }
    return true;
  }

  // Setup activity layout
  @Override
  protected void onCreate(Bundle savedInstanceState) {
    requestWindowFeature(Window.FEATURE_NO_TITLE);
    super.onCreate(savedInstanceState);

    // Recommend update google play service
    checkPlayServices();

    // Firebase initialize takes long time for some devices...
    FirebaseApp.initializeApp(this);

    Log.i(TAG, "FirebaseCrashlytics Enable!!!");
    FirebaseCrashlytics.getInstance().setCrashlyticsCollectionEnabled(true);

    mFirebaseAnalytics = FirebaseAnalytics.getInstance(this);

    PushManager.Init(this);

    // Set deviceID synchronously.
    mDeviceUuidFactory = new DeviceUuidFactory(this);

    mBillingHelper = BillingHelper.getInstance(this, this);

    getWindow().setFormat(PixelFormat.RGBX_8888); // <--- This makes xperia play happy

    mUnityPlayer = new UnityPlayer(this);
    setContentView(mUnityPlayer);
    mUnityPlayer.requestFocus();

    // OneSignal Initialization
    OneSignal.initWithContext(this);
    OneSignal.setAppId(getString(R.string.onesignal_app_id));

    OneSignal.addSubscriptionObserver(this);
    OneSignal.setNotificationWillShowInForegroundHandler(new OneSignal.OSNotificationWillShowInForegroundHandler() {
      @Override
      public void notificationWillShowInForeground(OSNotificationReceivedEvent notificationReceivedEvent) {
        // Not showing notification when it's on Foreground
        notificationReceivedEvent.complete(null);

      //   OSNotification notification = notificationReceivedEvent.getNotification();
      //   // Get custom additional data you sent with the notification
      //   JSONObject data = notification.getAdditionalData();
   
      //   if (/* some condition */ ) {
      //      // Complete with a notification means it will show
      //      notificationReceivedEvent.complete(notification);
      //   }
      //   else {
      //     // Complete with null means don't show a notification
      //     notificationReceivedEvent.complete(null);
      //  }
     }
   });

    OSDeviceState osDeviceStatus = OneSignal.getDeviceState();
    String osUserId = osDeviceStatus.getUserId(); 
    if (osUserId != null && !osUserId.isEmpty()) {
      Log.d(TAG, "OneSignal Player Id: " + osUserId);
      UnityPlayer.UnitySendMessage("NativeHelper", "UpdateOneSignalToken", osUserId);
    }

//    PrintKeyHash();
  }

  private void PrintKeyHash()
  {
    Log.i("KeyHash:", "printKEyHash");
    // Add code to print out the key hash
    try {
      PackageInfo info = getPackageManager().getPackageInfo(
        getPackageName(),
        PackageManager.GET_SIGNATURES);
      for (android.content.pm.Signature signature : info.signatures) {
        MessageDigest md = MessageDigest.getInstance("SHA");
        md.update(signature.toByteArray());
        Log.i("KeyHash:", Base64.encodeToString(md.digest(), Base64.DEFAULT));
      }
    } catch (PackageManager.NameNotFoundException e) {
      Log.i("KeyHash:", e.toString());

    } catch (NoSuchAlgorithmException e) {
      Log.i("KeyHash:", e.toString());
    }
  }

  // Quit Unity
  @Override
  protected void onDestroy() {
    mUnityPlayer.quit();
    super.onDestroy();
    isForeground = false;

    if (mBillingHelper != null)
    {
        mBillingHelper.dispose();
        mBillingHelper = null;
    }
  }

  // Pause Unity
  @Override
  protected void onPause() {
    super.onPause();

    if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N) {
      if (isInPictureInPictureMode())
      {
        // Log.d(TAG, "onPause : reume pip");
        mUnityPlayer.resume();
        mUnityPlayer.windowFocusChanged(true);
      }
      else if (triggerPIPMode)
      {
        // Log.d(TAG, "onPause : trigger pip");
        enterPipMode();
        mUnityPlayer.resume();
      } 
      else if (!isInMultiWindowMode()) 
      {
        mUnityPlayer.pause();
        IronSource.onPause(this);
        isForeground = false;
      }
    }
    else
    {
      mUnityPlayer.pause();
      IronSource.onPause(this);
      isForeground = false;
    } 
  }

  @Override
  protected void onStop() {
    super.onStop();

    if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N) {
      if(isInPictureInPictureMode())
      {
        // Log.d(TAG, "ChangedStateFromPipMode : stop");
        IronSource.onPause(this);
      }
    }

    mUnityPlayer.pause();
    isForeground = false;
  }

  @Override
  protected void onNewIntent(Intent intent) {
    super.onNewIntent(intent);
    setIntent(intent);
  }

  public static void HandleDeepLink(Uri uri)
  {
    // Log.d(TAG, "HandleDeepLink");
    if (uri != null) {
      // Log.d(TAG, "openURL");
      // Log.d(TAG, uri.toString());
      UnityPlayer.UnitySendMessage("NativeHelper", "OnOpenUrl", uri.toString());
      Adjust.appWillOpenUrl(uri);
    }
  }

  // Resume Unity
  @Override
  protected void onResume() {
    super.onResume();

    Log.d(TAG, "onResume");

    Bundle extra = null;
    Intent intent = getIntent();
    if (intent != null) {
      extra = intent.getExtras();
      if (extra != null) {
        // exist local push intent
        if (extra.getInt("pushID", 0) > 0) {
          StringBuilder builder = new StringBuilder();

          builder.append(extra.getString("type", "default_local_push"));
          builder.append(":");
          builder.append(extra.getString("title", "default_comment"));
          builder.append(":");
          builder.append(extra.getLong("getPushTimestamp", 0));

          UnityPlayer.UnitySendMessage("NativeHelper", "OnLocalPushMessage", builder.toString());
        }
        // exist server push intent
        if (!TextUtils.isEmpty(extra.getString("data", ""))) {
          UnityPlayer.UnitySendMessage("NativeHelper", "OnPendingMessage", extra.getString("data", ""));
        }
        intent.replaceExtras(new Bundle());
      } else {
        // Log.d(TAG, "Found no extra");
      }

      if (Intent.ACTION_VIEW.equals(intent.getAction())){
        Uri uri = intent.getData();
        HandleDeepLink(uri);
        getIntent().setData(null);
      }
    }

    mUnityPlayer.resume();
    IronSource.onResume(this);
    isForeground = true;
    PushManager.Clear(this);

    // notify the unity of a change in condition
    // UnityPlayer.UnitySendMessage("NativeHelper", "ChangedStateFromPipMode", "resume");
  }

  private void enterPipMode()
  {
    // Pip aspect ratio. must be between 0.418410 and 2.390000.
    float aspectRatio = (float)mUnityPlayer.getWidth() / (float)mUnityPlayer.getHeight();

    int width = mUnityPlayer.getWidth();
    int height = mUnityPlayer.getHeight();

    if(aspectRatio < PIP_RATIO_MIN) // Portrait
    {
        height = (int)((float)width * PIP_RATIO_MAX);
    }
    else if (aspectRatio > PIP_RATIO_MAX) // Landscape
    {
        width = (int)((float)height * PIP_RATIO_MAX);
    }

    Rational rational = new Rational(width, height);
    PictureInPictureParams params = new PictureInPictureParams.Builder()
            .setAspectRatio(rational)
            .build();
    enterPictureInPictureMode(params);
  }

  @Override
  protected void onUserLeaveHint()
  {
    // Log.d(TAG, "onUserLeaveHint");
    if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O && enablePIP)
    {
      // Log.d(TAG, "onUserLeaveHint Active PIP");
      triggerPIPMode = true;
    }
    else
    {
      // Log.d(TAG, "onUserLeaveHint Inactive PIP");
      triggerPIPMode = false;
    }
  }

  @Override
  public void onPictureInPictureModeChanged(boolean isInPictureInPictureMode, Configuration newConfig) {
    // Log.d(TAG, "onPictureInPictureModeChanged");
      if (isInPictureInPictureMode) {
        // Log.d(TAG, "onPictureInPictureModeChanged : Is PIP Mode");
        UnityPlayer.UnitySendMessage(unityObjectName, pipUnityCallback, "True");
      } else {
          // Restore the full-screen UI.
        // Log.d(TAG, "onPictureInPictureModeChanged : Isn't PIP Mode");
        UnityPlayer.UnitySendMessage(unityObjectName, pipUnityCallback, "False");
        triggerPIPMode = false;
      }
  }

  // This ensures the layout will be correct.
  @Override
  public void onConfigurationChanged(Configuration newConfig) {
    super.onConfigurationChanged(newConfig);
    mUnityPlayer.configurationChanged(newConfig);
  }

  // Notify Unity of the focus change.
  @Override
  public void onWindowFocusChanged(boolean hasFocus) {
    // Log.d(TAG, "onWindowFocusChanged : " + Boolean.toString(hasFocus));
    super.onWindowFocusChanged(hasFocus);
    mUnityPlayer.windowFocusChanged(hasFocus);
  }

  // For some reason the multiple keyevent type is not supported by the ndk.
  // Force event injection by overriding dispatchKeyEvent().
  @Override
  public boolean dispatchKeyEvent(KeyEvent event) {
    // Log.d(TAG, "dispatchKeyEvent");
    if (event.getAction() == KeyEvent.ACTION_MULTIPLE)
      return mUnityPlayer.injectEvent(event);
    return super.dispatchKeyEvent(event);
  }

  // Pass any events not handled by (unfocused) views straight to UnityPlayer
  @Override
  public boolean onKeyUp(int keyCode, KeyEvent event) {
    return mUnityPlayer.injectEvent(event);
  }

  @Override
  public boolean onKeyDown(int keyCode, KeyEvent event) {
    return mUnityPlayer.injectEvent(event);
  }

  @Override
  public boolean onTouchEvent(MotionEvent event) {
    return mUnityPlayer.injectEvent(event);
  }

  /* API12 */
  public boolean onGenericMotionEvent(MotionEvent event) {
    return mUnityPlayer.injectEvent(event);
  }

  //
  // In App Billing
  //

  public void purchase(int serverProductId, String sku, long purchaseProgressId) {
    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null) return;

    SharedPreferences prefs = currentInstance.getSharedPreferences(PREFS_FILE, 0);

    JSONObject json = new JSONObject();
    try
    {
      json.put("serverProductId", serverProductId);
      json.put("sku", sku);
      json.put("purchaseProgressId", purchaseProgressId);
    }
    catch (JSONException e)
    {
      e.printStackTrace();
    }

    // Write the value out to the prefs file
    prefs.edit()
      .putString(PREFS_PURCHASE, json.toString())
      .commit();

    try
    {
      mBillingHelper.launchBillingFlow(this, BillingClient.SkuType.INAPP, sku);
    }
    catch (Exception e)
    {
      Log.e(TAG, "Failed to purchase", e);
      UnityPlayer.UnitySendMessage(unityObjectName, iABUnityCallBack, "Error" + e.toString());
    }
  }

  public void subscribe(int serverProductId, String sku, long purchaseProgressId)
  {
    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null) return;

    SharedPreferences prefs = currentInstance.getSharedPreferences(PREFS_FILE, 0);

    JSONObject json = new JSONObject();
    try
    {
      json.put("serverProductId", serverProductId);
      json.put("sku", sku);
      json.put("purchaseProgressId", purchaseProgressId);
    }
    catch (JSONException e)
    {
      e.printStackTrace();
    }

    // Write the value out to the prefs file
    prefs.edit()
      .putString(PREFS_PURCHASE, json.toString())
      .commit();

    try
    {
      if(mBillingHelper.isSubscriptionSupported())
      {
        mBillingHelper.launchBillingFlow(this, BillingClient.SkuType.SUBS, sku);
      }
      else
      {
        Log.e(TAG, "Error Subscription is not supported");
        UnityPlayer.UnitySendMessage(unityObjectName, iABUnityCallBack, "Error Subscription is not supported.");
      }
      
    } catch (Exception e) {
      Log.e(TAG, "Failed to subscription", e);
      UnityPlayer.UnitySendMessage(unityObjectName, iABUnityCallBack, "Error" + e.toString());
    }
  }

  public void reportPurchaseFulfillment()
  {
    SharedPreferences prefs = this.getSharedPreferences(PREFS_FILE, 0);
    String purchaseInfo = prefs.getString(PREFS_PURCHASE, null);

    if (purchaseInfo != null)
    {
      try
      {
        JSONObject purchaseInfoJSON = new JSONObject(purchaseInfo);
        mBillingHelper.consumeInappPurchase(purchaseInfoJSON.get("sku").toString());
      }
      catch (JSONException e)
      {

      }
    }

    UnityPlayer.UnitySendMessage(unityObjectName, iABUnityCallBack, "Success");
  }

  @Override // BillingHelperListener
  public void onPurchaseSkuDetails(SkuDetails skuDetails)
  {
    if (null == skuDetails) return;

    // save information.
    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null)
    {
        UnityPlayer.UnitySendMessage(unityObjectName, iABUnityCallBack, "Error Activity(onPurchaseSkuDetails)");
        return;
    }
    SharedPreferences prefs = currentInstance.getSharedPreferences(PREFS_FILE, 0);
    String purchaseInfo = prefs.getString(PREFS_PURCHASE, null);

    JSONObject purchaseInfoJSON = null;
    try
    {
      // Log.d(TAG, "onPurchaseSkuDetails. Parse json");
      purchaseInfoJSON = new JSONObject(purchaseInfo);
      if(!purchaseInfoJSON.getString("sku").equals(skuDetails.getSku())){
          throw new Exception("SkuDetails does not match stored PREFS_PURCHASE data");
      }

      // Log.d(TAG, "onPurchaseSkuDetails. currencyCode");
      purchaseInfoJSON.put("currencyCode", skuDetails.getPriceCurrencyCode() );
      // Log.d(TAG, "onPurchaseSkuDetails. price");
      purchaseInfoJSON.put("localPrice", skuDetails.getPrice() );
    }
    catch (Exception e)
    {
      Log.d(TAG, "Error Json Exception(onPurchaseSkuDetails)");
      e.printStackTrace();
      if(purchaseInfoJSON == null)
        UnityPlayer.UnitySendMessage(unityObjectName, iABUnityCallBack, "Error Json Exception(onPurchaseSkuDetails)" + e.getLocalizedMessage() + " restoreDataJson: [ null ]");
      else
        UnityPlayer.UnitySendMessage(unityObjectName, iABUnityCallBack, "Error Json Exception(onPurchaseSkuDetails)" + e.getLocalizedMessage() + " restoreDataJson: [ " + purchaseInfoJSON.toString() + " ]");
      return;
    }

    prefs.edit()
      .putString(PREFS_PURCHASE, purchaseInfoJSON.toString())
      .commit();
  }

  @Override // BillingHelperListener
  public void onPurchaseFinishedListener(int billingResponseCode, String message, Purchase purchase)
  {
    // Log.d(TAG, "Purchase finished: " + billingResponseCode + ", purchase: " + purchase);

    // if we were disposed of in the meantime, quit.
    if (mBillingHelper == null)
    {
      UnityPlayer.UnitySendMessage(unityObjectName, iABUnityCallBack, "Error Billing helper is null");
      return;
    }

    if(purchase == null && "NO_UNCLAIMED_PURCHASE" == message)
    {
      Log.d(TAG, "No Outstanding Purchases");
      UnityPlayer.UnitySendMessage(unityObjectName, iABUnityCallBack, "NO_UNCLAIMED_PURCHASE");
      return;
    }

    if("PENDING" == message)
    {
      Log.d(TAG, "Pending Purchase");
      if(purchase != null && purchase.getSkus() != null && purchase.getSkus().get(0) != null)
      {
        UnityPlayer.UnitySendMessage(unityObjectName, iABUnityCallBack, "Pending " + purchase.getSkus().get(0));
      }
      else
      {
        UnityPlayer.UnitySendMessage(unityObjectName, iABUnityCallBack, "Pending Purchase null");
      }
      return;
    }

    if (billingResponseCode != BillingClient.BillingResponseCode.OK)
    {
      // Log.d(TAG, "Error purchasing: " + result);
      if(purchase != null && purchase.getSkus() != null && purchase.getSkus().get(0) != null)
      {
        mBillingHelper.consumeInappPurchase(purchase.getSkus().get(0));
      }

      UnityPlayer.UnitySendMessage(unityObjectName, iABUnityCallBack, "Error" + message);
      return;
    }

    Log.d(TAG, "Purchase successful.");
    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null)
    {
        UnityPlayer.UnitySendMessage(unityObjectName, iABUnityCallBack, "Error Activity(onPurchaseFinishedListener)");
        return;
    }

    SharedPreferences prefs = currentInstance.getSharedPreferences(PREFS_FILE, 0);
    String purchaseInfo = prefs.getString(PREFS_PURCHASE, null);

    JSONObject purchaseInfoJSON = null;
    try
    {
      String sku = purchase.getSkus().get(0);
      purchaseInfoJSON = new JSONObject(purchaseInfo);
      if(!purchaseInfoJSON.getString("sku").equals(sku)){
          throw new Exception("Purchase SKU does not match stored PREFS_PURCHASE data");
      }
      purchaseInfoJSON.put("receipt", purchase.getOriginalJson());
      purchaseInfoJSON.put("signature", purchase.getSignature());

      SkuDetails skuDetails = mBillingHelper.GetSkuDetails(sku);
      if(skuDetails != null)
      {
        purchaseInfoJSON.put("currencyCode", skuDetails.getPriceCurrencyCode() );
        purchaseInfoJSON.put("localPrice", skuDetails.getPrice() );
      }
      
    }
    catch (Exception e)
    {
      e.printStackTrace();
      // TODO: Do not automatically consume from the native code. Send back to Unity for consistent processing across platforms
      mBillingHelper.consumeInappPurchase(purchase.getSkus().get(0));
      if(purchaseInfoJSON == null)
        UnityPlayer.UnitySendMessage(unityObjectName, iABUnityCallBack, "Error Json Exception(onPurchaseFinishedListener)" + e.getLocalizedMessage() + " restoreDataJson: [ null ]");
      else
        UnityPlayer.UnitySendMessage(unityObjectName, iABUnityCallBack, "Error Json Exception(onPurchaseFinishedListener)" + e.getLocalizedMessage() + " restoreDataJson: [ " + purchaseInfoJSON.toString() + " ]");
      return;
    }

    prefs.edit()
            .putString(PREFS_PURCHASE, purchaseInfoJSON.toString())
            .commit();

    String encodedJson = Base64.encodeToString(purchaseInfoJSON.toString().getBytes(), Base64.NO_CLOSE | Base64.NO_WRAP);
    UnityPlayer.UnitySendMessage(unityObjectName, iABUnityCallBack, encodedJson);
  }

  @Override // BillingHelperListener
  public void onConsumeFinished(Purchase purchase)
  {
    // Log.d(TAG, "Consumption finished. Purchase: " + purchase + ", result: " + result);

    // Remove string in the prefs file
    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null) return;

    SharedPreferences prefs = currentInstance.getSharedPreferences(PREFS_FILE, 0);
    String purchaseInfo = prefs.getString(PREFS_PURCHASE, null);
    if (purchaseInfo != null) {
      prefs.edit()
        .remove(PREFS_PURCHASE)
        .commit();
    }

    Log.d(TAG, "onConsumeFinished");
  }

  // Unused. 
  public void consumeUnclaimedPurchase() throws JSONException
  {
      mBillingHelper.consumeUnclaimedPurchases();
  }

  public Context getCurrentInstanceContext()
  {
    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null || currentInstance.mDeviceUuidFactory == null) return null;

    Context context = currentInstance.getApplicationContext();
    return context;
  }

  public void openAppNotificationSettings()
  {
    if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O)
    {
      Context context = getCurrentInstanceContext();
      if (context == null) return;
      
      Intent intent = new Intent();
      intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
      intent.setAction(Settings.ACTION_APP_NOTIFICATION_SETTINGS);
      intent.putExtra(Settings.EXTRA_APP_PACKAGE, context.getPackageName());
      context.startActivity(intent);
    }
    else if(Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP)
    {
      Context context = getCurrentInstanceContext();
      if (context == null) return;
      
      Intent intent = new Intent();
      intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
      intent.setAction("android.settings.APP_NOTIFICATION_SETTINGS");
      intent.putExtra("app_package", context.getPackageName());
      intent.putExtra("app_uid", context.getApplicationInfo().uid);
      context.startActivity(intent);
    }
    else
    {
      openAppDetailsSettings();
    }
  }

  public void openAppDetailsSettings()
  {
    Context context = getCurrentInstanceContext();
    if (context == null) return;

    Intent intent = new Intent();
    intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
    intent.setAction(Settings.ACTION_APPLICATION_DETAILS_SETTINGS);
    intent.addCategory(Intent.CATEGORY_DEFAULT);
    intent.setData(Uri.parse("package:" + context.getPackageName()));
    context.startActivity(intent);
  }

  //
  // Native Helper APIs
  //

  @SuppressWarnings("unused")
  public static void Initialize(String name) {
    unityObjectName = name;
  }

  @Nullable
  @SuppressWarnings("unused")
  public static String getDeviceID() {
    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null || currentInstance.mDeviceUuidFactory == null) {
      // Will never happen.
      return null;
    }
    return currentInstance.mDeviceUuidFactory.getDeviceUuid().toString();
  }

  @SuppressWarnings("unused")
  public static boolean isNetworkAvailable() {
    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null) {
      return true;
    }

    Context context = currentInstance.getApplicationContext();
    if (context == null) {
      return true;
    }

    ConnectivityManager manager = (ConnectivityManager) (context.getSystemService(Context.CONNECTIVITY_SERVICE));
    if (manager == null) {
      return true;
    }

    NetworkInfo mobile = manager.getNetworkInfo(ConnectivityManager.TYPE_MOBILE);
    NetworkInfo wifi = manager.getNetworkInfo(ConnectivityManager.TYPE_WIFI);

    if (mobile == null || wifi == null) return true;
    if (mobile.isConnected() || wifi.isConnected()) {
      return true;
    }

    return false;
  }

  @SuppressWarnings("unused")
  @Nullable
  public static String getNotificationToken() {
    try {
      // TODO 
      // FirebaseMessaging.getInstance().getToken().addOnSuccessListener
      // <Task> 
      return null;
    } catch (IllegalStateException ex) {
      // May not initialized yet.
      return null;
    }
  }

  @SuppressWarnings("unused")
  @Nullable
  public static String getServerBaseUrl() {
    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null) {
      return null;
    }
    return currentInstance.getResources().getString(R.string.server_base_url);
  }

  @SuppressWarnings("unused")
  @Nullable
  public static String getChattingUrl() {
    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null) {
      return null;
    }
    return currentInstance.getResources().getString(R.string.chatting_url);
  }

  @SuppressWarnings("unused")
  @Nullable
  public static String getAppDownloadUrl() {
    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null) {
      return null;
    }
    return currentInstance.getResources().getString(R.string.app_download_url);
  }

  @SuppressWarnings("unused")
  public static void getProfileImage(int width, int height, boolean isCropable, String unityCallBack) {
    if (unityCallBack == null) {
      return;
    }
    profileImageUnityCallBack = unityCallBack;
    profileWidth = width;
    profileHeight = height;

    float ratio = width / height;

    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null) {
      return;
    }

    if (isCropable) {
      CropImage.activity(null)
        .setGuidelines(CropImageView.Guidelines.ON)
        .setAspectRatio(1, 1)
        .setMinCropResultSize(width, height)
        .start(currentInstance);
    }
    else {
      CropImage.startPickImageActivity(currentInstance);
    }
  }

  @SuppressWarnings("unused")
  public static void copyClipboard(String str) {
    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null) {
      return;
    }

    ClipboardManager clipboard = (ClipboardManager) currentInstance.getSystemService(Context.CLIPBOARD_SERVICE);
    ClipData clip = ClipData.newPlainText(null, str);
    clipboard.setPrimaryClip(clip);
  }

  private static final long MEGA_BYTE = 1048576;

  @SuppressWarnings({"unused", "deprecation"})
  public static long getFreeDiskSpace() {
    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null) {
      return 0;
    }

    StatFs statFs = getStats(false);
    long freeBytes = 0;

    if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR2) {
        freeBytes = statFs.getAvailableBytes() / MEGA_BYTE;
    }
    else {
      freeBytes = ((long) statFs.getBlockSize() * (long) statFs
        .getAvailableBlocks()) / MEGA_BYTE;
    }

    return freeBytes;
  }

  private static StatFs getStats(boolean external) {
    String path;

    if (external) {
      path = Environment.getExternalStorageDirectory().getAbsolutePath();
    } else {
      path = Environment.getDataDirectory().getAbsolutePath();
    }

    return new StatFs(path);
  }


  //
  // Push
  //

  @SuppressWarnings("unused")
  public static boolean isAcceptingPush() {
    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null) {
      return false;
    }
    return PushManager.GetPushState(currentInstance);
  }

  @SuppressWarnings("unused")
  public static void setAcceptingPush(boolean isAccept) {
    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null) {
      return;
    }
    PushManager.SavePushState(currentInstance, isAccept);
  }

  @SuppressWarnings("unused")
  public static void setLocalPush(int iPushID, String strSender, String strMessage, String type, int iSeconds, long getPushTimestamp) {
    // Log.d("push", "setLocalPush");
    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null) {
      return;
    }
    if (isAcceptingPush()) {
      // Log.d("push", "isAcceptingPush");
      PushManager.SetLocalPush(currentInstance, iPushID, strSender, strMessage, type, iSeconds, getPushTimestamp);
    }
  }

  @SuppressWarnings("unused")
  public static void cancelLocalPushNotification(int iPushID) {
    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null) {
      return;
    }
    PushManager.CancelLocalPushNotification(currentInstance, iPushID);
  }

  //
  // Purchase
  //

  @SuppressWarnings("unused")
  public static void purchase(int serverProductId, String sku, boolean isSubscription, long purchaseProgressId, String unityCallBack) {
    if (unityCallBack == null) {
      return;
    }
    iABUnityCallBack = unityCallBack;

    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null) {
      return;
    }

    if (isSubscription) {
      currentInstance.subscribe(serverProductId, sku, purchaseProgressId);
    } else {
      currentInstance.purchase(serverProductId, sku, purchaseProgressId);
    }
  }

  @SuppressWarnings("unused")
  public static void consumeUnclaimedPurchase(String unityCallBack) throws JSONException {
    if (unityCallBack == null) {
      return;
    }
    iABUnityCallBack = unityCallBack;

    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null) {
      return;
    }
    currentInstance.consumeUnclaimedPurchase();
  }

  @SuppressWarnings("unused")
  public static void reportPurchaseFulfillment(String unityCallBack) {
    if (unityCallBack == null) {
      return;
    }
    iABUnityCallBack = unityCallBack;

    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null) {
      return;
    }

    currentInstance.reportPurchaseFulfillment();
  }

  //
  // Adjust
  //

  @SuppressWarnings("unused")
  public static void initAdjust(String unityObjectName, String unityAttributionCallBack) {
    adjustUnityObjectName = unityObjectName;
    adjustAttributionCallBack = unityAttributionCallBack;

    if (isAdjustAttributionCallBackAlreadyCalled) {
      AdjustAttribution attribution = Adjust.getAttribution();
      respondAttributionCallBack(attribution);
    }
  }

  public static void respondAttributionCallBack(AdjustAttribution attribution) {
    isAdjustAttributionCallBackAlreadyCalled = true;
    if (adjustAttributionCallBack == null) return;

    JSONObject data = new JSONObject();
    try {
      data.put("adid", attribution.adid);
      data.put("trackerToken", attribution.trackerToken);
      data.put("trackerName", attribution.trackerName);
      data.put("network", attribution.network);
      data.put("campaign", attribution.campaign);
      data.put("adgroup", attribution.adgroup);
      data.put("creative", attribution.creative);
      data.put("clickLabel", attribution.clickLabel);

      UnityPlayer.UnitySendMessage(adjustUnityObjectName, adjustAttributionCallBack, data.toString());
    } catch (JSONException e) {
    }
  }

  @Nullable
  @SuppressWarnings("unused")
  public static String getAdjustID() {
    return Adjust.getAdid();
  }

  @SuppressWarnings("unused")
  public static void sendAdjustEvent(String eventToken) {
    AdjustEvent event = new AdjustEvent(eventToken);
    Adjust.trackEvent(event);
  }

  @SuppressWarnings("unused")
  public static void sendAdjustRevenueEvent(String eventToken, double revenue, String purchaseID) {
    AdjustEvent event = new AdjustEvent(eventToken);
    event.setRevenue(revenue, "USD");
    event.setOrderId(purchaseID);
    Adjust.trackEvent(event);
  }

  @SuppressWarnings("unused")
  public static void sendFIREvent(String eventID) {
    final UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null || currentInstance.mFirebaseAnalytics == null) {
      return;
    }
    Bundle params = new Bundle();
    // params.putString("image_name", name);
    // params.putString("full_text", text);
    currentInstance.mFirebaseAnalytics.logEvent(eventID, params);
  }

  @Override
  protected void onActivityResult(int requestCode, int resultCode, Intent data) {
    CallbackManager fbCallbackManager = FacebookManager.Instance().GetCallbackManager();
    if (fbCallbackManager != null) {
      fbCallbackManager.onActivityResult(requestCode, resultCode, data);
    }

    switch (requestCode) {
      case PLAY_SERVICES_RESOLUTION_REQUEST:
        break;
      case CropImage.PICK_IMAGE_CHOOSER_REQUEST_CODE:
        if (resultCode == RESULT_OK) {
          Uri resultUri = CropImage.getPickImageResultUri(this, data);

          try {
            Bitmap resultImage = MediaStore.Images.Media.getBitmap(this.getContentResolver(), resultUri);
            Bitmap scaledBitmap = ImageResizer.Resize(resultImage, profileWidth, profileHeight);
            // Delete resultImage
            resultImage.recycle();
            // Encode to bytes
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            scaledBitmap.compress(Bitmap.CompressFormat.JPEG, 100, baos);
            // Delete resized image
            scaledBitmap.recycle();
            byte[] bytes = baos.toByteArray();
            // Base64 encode to return to unity
            String encodedImage = Base64.encodeToString(bytes, Base64.DEFAULT);
            UnityPlayer.UnitySendMessage(unityObjectName, profileImageUnityCallBack, encodedImage);
          } catch (Exception e) {
            Log.e(TAG, "Failed to encode image", e);
            if (unityObjectName != null && profileImageUnityCallBack != null) {
              UnityPlayer.UnitySendMessage(unityObjectName, profileImageUnityCallBack, "");
            }
          }
        } else {
          if (resultCode == CropImage.CROP_IMAGE_ACTIVITY_RESULT_ERROR_CODE) {
            Log.e(TAG, "Failed to select image");
          }
          if (unityObjectName != null && profileImageUnityCallBack != null) {
            UnityPlayer.UnitySendMessage(unityObjectName, profileImageUnityCallBack, "");
          }
        }
        break;
      case CropImage.CROP_IMAGE_ACTIVITY_REQUEST_CODE:
        CropImage.ActivityResult result = CropImage.getActivityResult(data);
        if (resultCode == RESULT_OK) {
          Uri resultUri = result.getUri();
          try {
            Bitmap resultImage = MediaStore.Images.Media.getBitmap(this.getContentResolver(), resultUri);
            Bitmap scaledBitmap = ImageResizer.Resize(resultImage, profileWidth, profileHeight);
            // Delete resultImage
            resultImage.recycle();
            // Encode to bytes
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            scaledBitmap.compress(Bitmap.CompressFormat.JPEG, 100, baos);
            // Delete resized image
            scaledBitmap.recycle();
            byte[] bytes = baos.toByteArray();
            // Base64 encode to return to unity
            String encodedImage = Base64.encodeToString(bytes, Base64.DEFAULT);
            UnityPlayer.UnitySendMessage(unityObjectName, profileImageUnityCallBack, encodedImage);
          } catch (Exception e) {
            Log.e(TAG, "Failed to encode image", e);
            if (unityObjectName != null && profileImageUnityCallBack != null) {
              UnityPlayer.UnitySendMessage(unityObjectName, profileImageUnityCallBack, "");
            }
          }
        } else {
          if (resultCode == CropImage.CROP_IMAGE_ACTIVITY_RESULT_ERROR_CODE) {
            Log.e(TAG, "Failed to select image", result.getError());
          }
          if (unityObjectName != null && profileImageUnityCallBack != null) {
            UnityPlayer.UnitySendMessage(unityObjectName, profileImageUnityCallBack, "");
          }
        }
        break;

      case PURCHASE_REQUEST:
        // if (oldHelper == null) return;
        // try {
        //   if (oldHelper.handleActivityResult(requestCode, resultCode, data)) {
        //     // Log.d(TAG, "onActivityResult handled by IABUtil.");
        //   }
        // } catch (JSONException e) {
        //   Log.e(TAG, "IAB error", e);
        // }
        break;
      case SURVEY_MONKEY_REQUEST:
        UnityPlayer.UnitySendMessage(unityObjectName, "OnSurveyEnd", "");
        break;
    }

    super.onActivityResult(requestCode, resultCode, data);
  }

  @SuppressWarnings("unused")
  public static void setIdleTimerDisabled(boolean value) {
    if (value) {
      UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
        public void run() {
          UnityPlayer.currentActivity.getWindow().addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);
        }
      });
    }
    else
    {
      UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
        public void run() {
          UnityPlayer.currentActivity.getWindow().clearFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);
        }
      });
    }
  }

  public void onOSSubscriptionChanged(OSSubscriptionStateChanges stateChanges) {
//    if (!stateChanges.getFrom().getSubscribed() && stateChanges.getTo().getSubscribed()) {
    if (stateChanges.getTo().getUserId() != null && !stateChanges.getTo().getUserId().isEmpty()) {
      // The user is subscribed
      // Either the user subscribed for the first time
      // Or the user was subscribed -> unsubscribed -> subscribed
      Log.d(TAG, "OneSignal Player Id: " + stateChanges.getTo().getUserId());
      UnityPlayer.UnitySendMessage("NativeHelper", "UpdateOneSignalToken", stateChanges.getTo().getUserId());
    }
  }

  private Long GetStringResourcesAsLong(int resourceId)
  {
    String longString = getResources().getString(resourceId);
    long parsedLong = Long.parseLong(longString);

    return parsedLong;
  }

  public void getReferrerURL()
  {
    // Log.d(TAG, "getReferrerURL");
    referrerClient = InstallReferrerClient.newBuilder(this).build();
    referrerClient.startConnection(new InstallReferrerStateListener() {
        @Override
        public void onInstallReferrerSetupFinished(int responseCode) {
          String referrerUrl = null;

          switch (responseCode) {
              case InstallReferrerClient.InstallReferrerResponse.OK:
                  try {
                      ReferrerDetails response = referrerClient.getInstallReferrer();
                      referrerUrl = response.getInstallReferrer();
                      // long referrerClickTime = response.getReferrerClickTimestampSeconds();
                      // long appInstallTime = response.getInstallBeginTimestampSeconds();
                      // boolean instantExperienceLaunched = response.getGooglePlayInstantParam();
                      // Log.d(TAG, "Android Referrer URL:" + referrerUrl);
                  } catch (Exception e) {
                      // Log.d(TAG, "Android Referrer URL: Error. " + e.toString());
                  }
                  referrerClient.endConnection();
                  break;
              case InstallReferrerClient.InstallReferrerResponse.FEATURE_NOT_SUPPORTED:
                  // API not available on the current Play Store app.
                  // Log.d(TAG, "Android Referrer: InstallReferrerResponse.FEATURE_NOT_SUPPORTED");
                  referrerClient.endConnection();
                  break;
              case InstallReferrerClient.InstallReferrerResponse.SERVICE_UNAVAILABLE:
                  // Connection couldn't be established.
                  // Log.d(TAG, "Android Referrer: InstallReferrerResponse.SERVICE_UNAVAILABLE");
                  referrerClient.endConnection();
                  break;
            }

            if(referrerUnityCallBack != null)
              UnityPlayer.UnitySendMessage(unityObjectName, referrerUnityCallBack, referrerUrl);
        }

        @Override
        public void onInstallReferrerServiceDisconnected() {
            // Try to restart the connection on the next request to
            // Google Play by calling the startConnection() method.
            // Log.d(TAG, "Android Referrer: onInstallReferrerServiceDisconnected");

            if(referrerUnityCallBack != null)
              UnityPlayer.UnitySendMessage(unityObjectName, referrerUnityCallBack, null);
        }
    });
  }

  public void setPIP(boolean enable)
  {
    if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O)
    {
      // Log.d(TAG, "SetPIP Possible");
      enablePIP = enable;
    }
    else
    {
      // Log.d(TAG, "SetPIP Impossible");
      enablePIP = false;
    }
  }

  public void moveHomeScreen()
  {
    Intent startMain = new Intent(Intent.ACTION_MAIN);
    startMain.addCategory(Intent.CATEGORY_HOME);
    startMain.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
    getApplicationContext().startActivity(startMain);
  }

  @SuppressWarnings("unused")
  public static void OpenSurveyMonkey(String hash, String userId) {
    final UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null) {
      return;
    }

    JSONObject obj = new JSONObject();
    try {
      obj.put("userId", userId);
    } catch (Exception e) {
      Log.i(TAG, "Error while putting value into dictionary on survey");
    }
    currentInstance.smSDKInstance.onStart(currentInstance, currentInstance.getResources().getString(R.string.app_name), SURVEY_MONKEY_REQUEST, hash, obj);
    currentInstance.smSDKInstance.startSMFeedbackActivityForResult(currentInstance, SURVEY_MONKEY_REQUEST, hash, obj);

  }

  public static void OpenAppNotificationSettings()
  {
    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null) return;

    currentInstance.openAppNotificationSettings();
  }

  public static boolean areNotificationsEnabled()
  {
    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null) return false;

    return PushManager.areNotificationsEnabled(currentInstance);
  }

  public static void OpenAppSettingsPipMode()
  {
    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null) return;
    
    if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O)
    {
      currentInstance.openAppDetailsSettings();
    }
  }

  public static boolean arePipModeEnabled()
  {
    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null) return false;
    
    Context context = currentInstance.getApplicationContext();
    if (context == null) return false;

    if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O)
    {
      AppOpsManager appOps = (AppOpsManager)currentInstance.getSystemService(Context.APP_OPS_SERVICE);

      return appOps.checkOpNoThrow(AppOpsManager.OPSTR_PICTURE_IN_PICTURE,
        context.getApplicationInfo().uid, context.getPackageName()) == AppOpsManager.MODE_ALLOWED;
    }
    
    return false;
  }

  public static boolean arePipModeAvailable()
  {
    return Build.VERSION.SDK_INT >= Build.VERSION_CODES.O;
  }

  public static void setExternalUserId(String userId) 
  {
     // You will supply the external user id to the OneSignal SDK
      // Setting External User Id with Callback Available in SDK Version 3.13.0+

      Log.d(TAG, "set OneSignal Player External Id : " + userId);

      OneSignal.setExternalUserId(userId, new OneSignal.OSExternalUserIdUpdateCompletionHandler() {
        @Override
        public void onSuccess(JSONObject results) {
          // The results will contain push and email success statuses
          OneSignal.onesignalLog(OneSignal.LOG_LEVEL.VERBOSE, "Set external user id done with results: " + results.toString());
        }

        @Override
        public void onFailure(OneSignal.ExternalIdError error) {

        }
      });
  }

  public static void removeExternalUserId()
  {
      // Removing External User Id with Callback Available in SDK Version 3.13.0+
      OneSignal.removeExternalUserId(new OneSignal.OSExternalUserIdUpdateCompletionHandler() {
        @Override
        public void onSuccess(JSONObject results) {
          // The results will contain push and email success statuses
          OneSignal.onesignalLog(OneSignal.LOG_LEVEL.VERBOSE, "Remove external user id done with results: " + results.toString());
  
          Log.d(TAG, "Remove OneSignal Player External Id");
        }
  
        @Override
        public void onFailure(OneSignal.ExternalIdError error) {
  
        }
    });
  }

  public static boolean isApplicationInstall(String packageName)
  {
    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null)
    {
      return false;
    }

    try
    {
      currentInstance.getPackageManager().getApplicationInfo(packageName, PackageManager.GET_META_DATA);
      Log.d(TAG, packageName + " is installed.");
      return true;
    }
    catch (PackageManager.NameNotFoundException e)
    {
      Log.d(TAG, packageName + " is not installed.");
    }

    return false;
  }

  @SuppressWarnings("unused")
  public static void OnTestRuntimeCrash(String message)
  {

    Log.i(TAG, message);
    throw new RuntimeException(message);
  }

  public static void GetReferrerURL(String unityCallBack) {
    if (unityCallBack == null) return;

    referrerUnityCallBack = unityCallBack;

    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null) return;

    currentInstance.getReferrerURL();
  }
  
  public static void SetPIP(boolean enable)
  {
    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null) return;

    currentInstance.setPIP(enable);
  }

  public static void MoveHomeScreen()
  {
    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null) return;

    currentInstance.moveHomeScreen();
  }
}
