package com.bagelcode.v3.push;

import android.annotation.SuppressLint;
import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Color;
import android.media.RingtoneManager;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.os.Environment;
import androidx.core.app.NotificationCompat;

import com.bagelcode.v3.R;
import com.bagelcode.v3.UnityPlayerActivity;

import android.util.Log;

public class NotificationFactory {
  public static final int NOTIFICATION_ID = 1;
  private static final String TAG = "BagelCode_Notification";

  private static Uri getRingtoneDefaultUriOnInternalStorage() {
    Uri defaultSoundUri = RingtoneManager.getDefaultUri(RingtoneManager.TYPE_NOTIFICATION);
    String extDir = Environment.getExternalStorageDirectory().getAbsolutePath();
    if(!defaultSoundUri.getPath().contains(extDir)) {
      return defaultSoundUri;
    }
    return null;
  }

  @SuppressLint("NewApi")
  public static void sendNotification(Context context, String title, String message, Bundle extra) {
    Log.d(TAG, "sendNotification");
    
    boolean isForeground = UnityPlayerActivity.getIsForeground();
    if (isForeground) {
      return;
    }
    boolean pushAccept = PushManager.GetPushState(context);
    if (!pushAccept) {
      return;
    }

    Intent intent = new Intent(context, UnityPlayerActivity.class);
    intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP | Intent.FLAG_ACTIVITY_SINGLE_TOP);
    if (extra != null) {
      intent.putExtras(extra);
    }
    PendingIntent pendingIntent = PendingIntent.getActivity(context, 0 /* Request code */, intent,
      PendingIntent.FLAG_UPDATE_CURRENT);

    Uri defaultSoundUri = getRingtoneDefaultUriOnInternalStorage();
    Uri customSoundUri = Uri.parse("android.resource://" + context.getPackageName() + "/raw/push_sound");
    Bitmap bMap = BitmapFactory.decodeResource(context.getResources(), R.mipmap.ic_launcher);

    int currentapiVersion = android.os.Build.VERSION.SDK_INT;
    if (currentapiVersion >= Build.VERSION_CODES.O) {
      String channelId  = context.getString(R.string.default_notification_channel_id);
      Notification.Builder notificationBuilder = new Notification.Builder(context, channelId)
        .setShowWhen(true)
        .setSmallIcon(R.drawable.notification_material)
        .setLargeIcon(bMap)
        .setContentTitle(title)
        .setContentText(message)
        .setColor(Color.parseColor("#FF0000"))
        .setAutoCancel(true)
        .setContentIntent(pendingIntent)
        .setStyle(new Notification.BigTextStyle().bigText(message));

      NotificationManager notificationManager = (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);
      notificationManager.notify(0 /* ID of notification */, notificationBuilder.build());
    } else if (currentapiVersion >= android.os.Build.VERSION_CODES.LOLLIPOP) {
      Notification.Builder notificationBuilder = new Notification.Builder(context)
        .setShowWhen(true)
        .setSmallIcon(R.drawable.notification_material)
        .setLargeIcon(bMap)
        .setContentTitle(title)
        .setContentText(message)
        .setColor(Color.parseColor("#FF0000"))
        .setAutoCancel(true)
        .setContentIntent(pendingIntent)
        .setStyle(new Notification.BigTextStyle().bigText(message));
      if (customSoundUri != null) {
        notificationBuilder.setSound(customSoundUri);
      } else if (defaultSoundUri != null) {
        notificationBuilder.setSound(defaultSoundUri);
      }

        NotificationManager notificationManager = (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);
        notificationManager.notify(0 /* ID of notification */, notificationBuilder.build());
    } else if (currentapiVersion > android.os.Build.VERSION_CODES.ICE_CREAM_SANDWICH_MR1) {
      Notification.Builder notificationBuilder = new Notification.Builder(context)
        .setSmallIcon(R.drawable.notification_icon)
        .setLargeIcon(bMap)
        .setContentTitle(title)
        .setContentText(message)
        .setAutoCancel(true)
        .setContentIntent(pendingIntent)
        .setStyle(new Notification.BigTextStyle().bigText(message));
      if (customSoundUri != null) {
        notificationBuilder.setSound(customSoundUri);
      } else if (defaultSoundUri != null) {
        notificationBuilder.setSound(defaultSoundUri);
      }

        NotificationManager notificationManager = (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);
        notificationManager.notify(0 /* ID of notification */, notificationBuilder.build());
    } else {
      NotificationCompat.Builder notificationBuilder = new NotificationCompat.Builder(context)
          .setShowWhen(true)
          .setSmallIcon(R.drawable.notification_icon)
          .setLargeIcon(bMap)
          .setContentTitle(title)
          .setContentText(message)
          .setAutoCancel(true)
          .setContentIntent(pendingIntent)
          .setStyle(new NotificationCompat.BigTextStyle().bigText(message));
      if (customSoundUri != null) {
        notificationBuilder.setSound(customSoundUri);
      } else if (defaultSoundUri != null) {
        notificationBuilder.setSound(defaultSoundUri);
      }

        NotificationManager notificationManager = (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);
        notificationManager.notify(0 /* ID of notification */, notificationBuilder.build());
    }
  }
}
