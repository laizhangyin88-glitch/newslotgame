package com.bagelcode.v3.fcm;

import android.annotation.SuppressLint;
import android.os.Bundle;
import android.util.Log;

import com.bagelcode.v3.UnityPlayerActivity;
import com.bagelcode.v3.push.NotificationFactory;

import com.google.firebase.messaging.FirebaseMessagingService;
import com.google.firebase.messaging.RemoteMessage;

import java.util.Map;

public class MyFirebaseMessagingService extends FirebaseMessagingService {

  private static final String TAG = "MyFirebaseMsgService";

  /**
   * Called when message is received.
   *
   * @param remoteMessage Object representing the message received from Firebase Cloud Messaging.
   */
  // [START receive_message]
  @Override
  public void onMessageReceived(RemoteMessage remoteMessage) {
    // [START_EXCLUDE]
    // There are two types of messages data messages and notification messages. Data messages are handled
    // here in onMessageReceived whether the app is in the foreground or background. Data messages are the type
    // traditionally used with GCM. Notification messages are only received here in onMessageReceived when the app
    // is in the foreground. When the app is in the background an automatically generated notification is displayed.
    // When the user taps on the notification they are returned to the app. Messages containing both notification
    // and data payloads are treated as notification messages. The Firebase console always sends notification
    // messages. For more see: https://firebase.google.com/docs/cloud-messaging/concept-options
    // [END_EXCLUDE]

    String title = "";
    String message = "";

    // TODO(developer): Handle FCM messages here.
    // Not getting messages here? See why this may be: https://goo.gl/39bRNJ
    Log.d(TAG, "From: " + remoteMessage.getFrom());

    // Check if message contains a notification payload.
    if (remoteMessage.getNotification() != null) {
      Log.d(TAG, "Message Notification Body: " + remoteMessage.getNotification().getBody());
      title = remoteMessage.getNotification().getTitle();
      message = remoteMessage.getNotification().getBody();
    }

    // Check if message contains a data payload.
    Map<String, String> dataPayload = remoteMessage.getData();
    Bundle extra = null;
    if (dataPayload.size() > 0) {
      Log.d(TAG, "Message data payload: " + dataPayload);
      String silent = dataPayload.get("silent");
      if ("true".equals(silent)) {
        return;
      }
      title = dataPayload.get("title");
      message = dataPayload.get("body");
      extra = new Bundle();
      for (Map.Entry<String, String> pair : dataPayload.entrySet()) {
        extra.putString(pair.getKey(), pair.getValue());
      }
    }

    // Also if you intend on generating your own notifications as a result of a received FCM
    // message, here is where that should be initiated. See sendNotification method below.

    if (title == null || message == null) {
      Log.w(TAG, "Title or message is missing");
      return;
    }

    if (!UnityPlayerActivity.getIsForeground()) {
      sendNotification(title, message, extra);
    }
  }
  // [END receive_message]

  /**
   * Create and show a simple notification containing the received FCM message.
   *
   * @param title
   * @param message title and message of FCM RemoteMessage received
   */
  @SuppressLint("NewApi")
  private void sendNotification(String title, String message, Bundle extra) {
    NotificationFactory.sendNotification(getBaseContext(), title, message, extra);
  }
}
