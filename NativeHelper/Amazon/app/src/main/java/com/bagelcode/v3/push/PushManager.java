package com.bagelcode.v3.push;

import android.app.AlarmManager;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.media.AudioAttributes;
import android.net.Uri;
import android.os.Build;
import android.preference.PreferenceManager;
import android.util.Log;

import com.bagelcode.v3.R;

import androidx.core.app.NotificationManagerCompat;

public class PushManager {
  //
  // Local Push
  //

  public static void Init(Context context) {
    if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
      // Create channel to show notifications.
      String channelId  = context.getString(R.string.default_notification_channel_id);
      String channelName = context.getString(R.string.default_notification_channel_name);
      NotificationChannel notificationChannel = new NotificationChannel(channelId, channelName, NotificationManager.IMPORTANCE_HIGH);

      Uri customSoundUri = Uri.parse("android.resource://" + context.getPackageName() + "/raw/push_sound");
      AudioAttributes audioAttributes = new AudioAttributes.Builder()
        .setUsage(AudioAttributes.USAGE_NOTIFICATION)
        .setContentType(AudioAttributes.CONTENT_TYPE_SONIFICATION)
        .build();
      notificationChannel.setSound(customSoundUri, audioAttributes);

      NotificationManager notificationManager = context.getSystemService(NotificationManager.class);
      if (notificationManager == null) {
        return;
      }
      try {
        notificationManager.createNotificationChannel(notificationChannel);
      } catch (Exception ex) {
        Log.e("BagelCode", "createNotificationChannel failed:", ex);
      }
    }
  }

  public static void SavePushState(Context context, boolean isAccept)
  {
    SharedPreferences settings = PreferenceManager.getDefaultSharedPreferences(context);
    SharedPreferences.Editor editor = settings.edit();
    editor.putBoolean("push", isAccept);
    editor.commit();
  }

  public static boolean GetPushState(Context context)
  {
    SharedPreferences settings = PreferenceManager.getDefaultSharedPreferences(context);
    boolean pushAccept = settings.getBoolean("push", true);

    return pushAccept;
  }

  @SuppressWarnings("static-access")
  public static void SetLocalPush(Context context, int pushID, String strSender, String strMessage, String type, int iSeconds, long getPushTimestamp) {
    Log.d("BagelCode", "SetLocalPush");

    Intent intent = new Intent(context, LocalPushReceiver.class);

    final String titleKey = context.getString(R.string.json_data_title_key);
    final String msgKey = context.getString(R.string.json_data_msg_key);

    intent.putExtra("pushID", pushID);
    intent.putExtra(titleKey, strSender);
    intent.putExtra(msgKey, strMessage);
    intent.putExtra("getPushTimestamp", getPushTimestamp);
    intent.putExtra("type", type);

    final String action = context.getString(R.string.intent_msg_action);
    intent.setAction(action);

    intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);

    PendingIntent pendingIntent = PendingIntent.getBroadcast(context, pushID, intent, PendingIntent.FLAG_CANCEL_CURRENT);

    AlarmManager am = (AlarmManager) context.getSystemService(context.ALARM_SERVICE);
    am.set(AlarmManager.RTC, System.currentTimeMillis() + (long)iSeconds * 1000, pendingIntent);
  }

  @SuppressWarnings("static-access")
  public static void CancelLocalPushNotification(Context context, int pushID) {
    Log.d("BagelCode", "CancelLocalPush");

    AlarmManager am = (AlarmManager) context.getSystemService(context.ALARM_SERVICE);
    Intent intent = new Intent(context, LocalPushReceiver.class);
    intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
    final String action = context.getString(R.string.intent_msg_action);
    intent.setAction(action);

    PendingIntent pendingIntent = PendingIntent.getBroadcast(context, pushID, intent, PendingIntent.FLAG_CANCEL_CURRENT);
    pendingIntent.cancel();
    am.cancel(pendingIntent);
  }

  public static void Clear(Context context) {
    NotificationManager nm = (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);
    if (nm != null) {
      try {
        nm.cancelAll();
      } catch (SecurityException se) {
        Log.d("BagelCode", "Clear notification failed:" + se.getMessage());
      }
    }
  }

  public static boolean areNotificationsEnabled(Context context)
  {
    if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O)
    {
      String channelId  = context.getString(R.string.default_notification_channel_id);

      if(NotificationManagerCompat.from(context).areNotificationsEnabled())
      {
        if(channelId != null && !channelId.isEmpty())
        {
          NotificationManager manager = context.getSystemService(NotificationManager.class);
          if(manager == null) return false;

          NotificationChannel channel = manager.getNotificationChannel(channelId);
          if(channel == null) return false;

          return channel.getImportance() != NotificationManager.IMPORTANCE_NONE;
        }
      }

      return false;
    }
    else
    {
      return NotificationManagerCompat.from(context).areNotificationsEnabled();
    }
  }
}
