package com.bagelcode.v3;

import android.app.Activity;
import android.content.ClipData;
import android.content.ClipboardManager;
import android.content.Context;
import android.content.Intent;
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
import android.view.KeyEvent;
import android.view.MotionEvent;
import android.view.Window;
import android.view.WindowManager;

import com.adjust.sdk.ActivityHandler;
import com.adjust.sdk.Adjust;
import com.adjust.sdk.AdjustAttribution;
import com.adjust.sdk.AdjustEvent;
import com.amazon.device.iap.PurchasingService;
import com.amazon.device.messaging.ADM;
import com.bagelcode.v3.iap.IapManager;
import com.bagelcode.v3.push.PushManager;
import com.bagelcode.v3.onesignal.NotificationOpenedHandler;
import com.facebook.CallbackManager;
import com.onesignal.OneSignal;
import com.onesignal.OSNotificationReceivedEvent;
import com.onesignal.OSDeviceState;
import com.onesignal.OSSubscriptionObserver;
import com.onesignal.OSSubscriptionStateChanges;
import com.theartofdev.edmodo.cropper.CropImage;
import com.theartofdev.edmodo.cropper.CropImageView;
import com.unity3d.player.UnityPlayer;

import java.io.ByteArrayOutputStream;
import java.lang.Runnable;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.util.ArrayList;

import org.json.JSONException;
import org.json.JSONObject;

import com.surveymonkey.surveymonkeyandroidsdk.SurveyMonkey;

public class UnityPlayerActivity extends Activity implements OSSubscriptionObserver {
  private static final String TAG = "UnityPlayerActivity";

  static final int SURVEY_MONKEY_REQUEST = 10001;

  private IapManager mIapManager;

  private static String unityObjectName = null;
  private static String adjustUnityObjectName = null;
  private static String adjustAttributionCallBack = null;
  private static String profileImageUnityCallBack = null;
  private static int profileWidth;
  private static int profileHeight;
  private static Boolean isAdjustAttributionCallBackAlreadyCalled = false;

  private static Boolean isForeground = false;
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

  protected DeviceUuidFactory mDeviceUuidFactory;

  private Boolean ADMAvailable = false;
  private ADM adm;

  public SurveyMonkey smSDKInstance = new SurveyMonkey();

  // Setup activity layout
  @Override
  protected void onCreate(Bundle savedInstanceState) {
    requestWindowFeature(Window.FEATURE_NO_TITLE);
    super.onCreate(savedInstanceState);

    PushManager.Init(this);

    // ADM
    try
    {
      Class.forName( "com.amazon.device.messaging.ADM" );
      ADMAvailable = true;
      Log.i(TAG, "ADM supported");
      adm = new ADM(this);
      String registrationId = adm.getRegistrationId();
      if (registrationId == null)
      {
        Log.i(TAG, "Not registered. Start registering");
        // startRegister() is asynchronous; your app is notified via the
        // onRegistered() callback when the registration ID is available.
        adm.startRegister();
      } else {
        Log.i(TAG, "registrationId:" + registrationId);
      }
    }
    catch (ClassNotFoundException e)
    {
      Log.i(TAG, "ADM not supported");
    }

    // Set deviceID synchronously.
    mDeviceUuidFactory = new DeviceUuidFactory(this);

    getWindow().setFormat(PixelFormat.RGBX_8888); // <--- This makes xperia play happy

    setupIAPOnCreate();

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

    } catch (NoSuchAlgorithmException e) {

    }
  }

  private void setupIAPOnCreate() {
    mIapManager = new IapManager(getApplicationContext());
  }

  // Quit Unity
  @Override
  protected void onDestroy() {
    mUnityPlayer.quit();
    super.onDestroy();
    isForeground = false;
  }

  // Pause Unity
  @Override
  protected void onPause() {
    super.onPause();
    if (android.os.Build.VERSION.SDK_INT >= Build.VERSION_CODES.N) {
      if (!isInMultiWindowMode()) {
        mUnityPlayer.pause();
        isForeground = false;
      }
    } else {
      mUnityPlayer.pause();
      isForeground = false;
    }
  }

  @Override
  protected void onStop() {
    super.onStop();
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
      }

      if (Intent.ACTION_VIEW.equals(intent.getAction())){
        Uri uri = intent.getData();
        HandleDeepLink(uri);
        getIntent().setData(null);
      }      

    }

    // Log.d(TAG, "onResume: getPurchaseUpdates");
    PurchasingService.getPurchaseUpdates(false);
    mUnityPlayer.resume();
    isForeground = true;
    PushManager.Clear(this);
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
    super.onWindowFocusChanged(hasFocus);
    mUnityPlayer.windowFocusChanged(hasFocus);
  }

  // For some reason the multiple keyevent type is not supported by the ndk.
  // Force event injection by overriding dispatchKeyEvent().
  @Override
  public boolean dispatchKeyEvent(KeyEvent event) {
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
    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null) {
      return null;
    }
    if (!currentInstance.ADMAvailable || currentInstance.adm == null) {
      return null;
    }
    return currentInstance.adm.getRegistrationId();
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

    Log.d("getFreeSpace", Long.toString(freeBytes));

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
  public static void purchase(int serverProductId, String sku, final String unityCallBack) {
    if (unityCallBack == null) {
      return;
    }
    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null) {
      return;
    }
    currentInstance.mIapManager.purchase(sku, new IapManager.OnIapPurchaseFinishedListener() {
      @Override
      public void onIapPurchaseFinished(String error, String userId, String receiptId) {
        if (error != null) {
          Log.e(TAG, error);
          UnityPlayer.UnitySendMessage(unityObjectName, unityCallBack, "Error" + error);
        } else {
          try {
            JSONObject receipt = new JSONObject();
            receipt.put("userId", userId);
            receipt.put("receiptId", receiptId);
            UnityPlayer.UnitySendMessage(unityObjectName, unityCallBack, receipt.toString());
          } catch (JSONException ex) {
            Log.e(TAG, "Stringify failed:", ex);
            UnityPlayer.UnitySendMessage(unityObjectName, unityCallBack, "Error" + com.bagelcode.v3.iap.IapManager.STRINGIFY_FAILED);
          }
        }
      }
    });
  }

  @SuppressWarnings("unused")
  public static void consumeUnclaimedPurchase(String unityCallBack) throws java.lang.Exception {
    if (unityCallBack == null) {
      return;
    }
    // consume it from onResume. PurchasingService.getPurchaseUpdates(false);
    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null) {
      return;
    }
    UnityPlayer.UnitySendMessage(unityObjectName, unityCallBack, "NO_UNCLAIMED_PURCHASE");
  }

  public void openAppNotificationSettings()
  {
    UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null || currentInstance.mDeviceUuidFactory == null) return;

    Context context = currentInstance.getApplicationContext();
    if (context == null) return;

    Intent intent = new Intent();
    intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);

    if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O)
    {
      intent.setAction(Settings.ACTION_APP_NOTIFICATION_SETTINGS);
      intent.putExtra(Settings.EXTRA_APP_PACKAGE, context.getPackageName());
    }
    else if(Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP)
    {
      intent.setAction("android.settings.APP_NOTIFICATION_SETTINGS");
      intent.putExtra("app_package", context.getPackageName());
      intent.putExtra("app_uid", context.getApplicationInfo().uid);
    }
    else
    {
      intent.setAction(Settings.ACTION_APPLICATION_DETAILS_SETTINGS);
      intent.addCategory(Intent.CATEGORY_DEFAULT);
      intent.setData(Uri.parse("package:" + context.getPackageName()));
    }

    context.startActivity(intent);
  }

  //
  // Adjust
  //

  @SuppressWarnings("unused")
  public static void initAdjust(String unityObjectName, String unityAttributionCallBack) {
    adjustUnityObjectName = unityObjectName;
    adjustAttributionCallBack = unityAttributionCallBack;

    if(isAdjustAttributionCallBackAlreadyCalled)
    {
      AdjustAttribution attribution = Adjust.getAttribution();
      respondAttributionCallBack(attribution);
    }
  }

  public static void respondAttributionCallBack(AdjustAttribution attribution) {
    isAdjustAttributionCallBackAlreadyCalled = true;
    if(adjustAttributionCallBack == null) return;

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
    } catch (JSONException e) {}
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
    
  }

  @Override
  protected void onActivityResult(int requestCode, int resultCode, Intent data) {
    CallbackManager fbCallbackManager = FacebookManager.Instance().GetCallbackManager();
    if (fbCallbackManager != null) {
      fbCallbackManager.onActivityResult(requestCode, resultCode, data);
    }

    switch (requestCode) {
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

  //
  // SuperSonic
  //
  @SuppressWarnings("unused")
  public static boolean IsVideoAdsAvailable(String placement) {
    // Amazon do nothing
    return false;
  }

  @SuppressWarnings("unused")
  public static void ShowRewardedVideo(String placement) {
    // Amazon do nothing
  }

  @SuppressWarnings("unused")
  public static long GetPlacementReward(String placement) {
    // Amazon do nothing
    return 0;
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

  @SuppressWarnings("unused")
  public static void OpenSurveyMonkey(String hash) {
    final UnityPlayerActivity currentInstance = CurrentInstance();
    if (currentInstance == null) {
      return;
    }

    currentInstance.smSDKInstance.onStart(currentInstance, currentInstance.getResources().getString(R.string.app_name), SURVEY_MONKEY_REQUEST, hash);
    currentInstance.smSDKInstance.startSMFeedbackActivityForResult(currentInstance, SURVEY_MONKEY_REQUEST, hash);
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
}
