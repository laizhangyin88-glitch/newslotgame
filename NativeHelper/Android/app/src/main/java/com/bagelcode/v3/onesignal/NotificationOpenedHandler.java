package com.bagelcode.v3.onesignal;

import com.onesignal.OSNotificationOpenedResult;
import com.onesignal.OSNotification;
import com.onesignal.OneSignal;
import com.unity3d.player.UnityPlayer;

import android.util.Log;
import org.json.JSONObject;

/**
 * Created by sungyunhur on 2018. 10. 8..
 */

public class NotificationOpenedHandler implements OneSignal.OSNotificationOpenedHandler {
  // This fires when a notification is opened by tapping on it.
  @Override
  public void notificationOpened(OSNotificationOpenedResult result) {
    OSNotification notification = result.getNotification();
    JSONObject additionalData = notification.getAdditionalData();
    String notificationId = notification.getNotificationId();

    if (notificationId != null) {
      Log.i("OneSignalExample", "Set Notification Id: " + notificationId);
      UnityPlayer.UnitySendMessage("NativeHelper", "SetBlackboardValue", "lastNotificationId:" + notificationId);
    }

    if (additionalData != null) {
      String data = additionalData.optString("data", null);
      if (data != null) {
         Log.i("OneSignalExample", "customkey set with value: " + data);
        UnityPlayer.UnitySendMessage("NativeHelper", "OnPendingMessage", data);
      }
    }

    // The following can be used to open an Activity of your choice.
    // Replace - getApplicationContext() - with any Android Context.
    // Intent intent = new Intent(getApplicationContext(), YourActivity.class);
    // intent.setFlags(Intent.FLAG_ACTIVITY_REORDER_TO_FRONT | Intent.FLAG_ACTIVITY_NEW_TASK);
    // startActivity(intent);

    // Add the following to your AndroidManifest.xml to prevent the launching of your main Activity
    //   if you are calling startActivity above.
     /*
        <application ...>
          <meta-data android:name="com.onesignal.NotificationOpened.DEFAULT" android:value="DISABLE" />
        </application>
     */
  }
}
