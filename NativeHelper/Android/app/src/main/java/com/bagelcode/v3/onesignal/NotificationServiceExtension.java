package com.bagelcode.v3.onesignal;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.util.Log;

import android.widget.RemoteViews;

import androidx.core.app.NotificationCompat;

import org.json.JSONObject;

import com.bagelcode.v3.R;
import com.onesignal.OSNotification;
import com.onesignal.OSMutableNotification;
import com.onesignal.OSNotificationReceivedEvent;
import com.onesignal.OneSignal.OSRemoteNotificationReceivedHandler;

import java.net.URL;


@SuppressWarnings("unused")
public class NotificationServiceExtension implements OSRemoteNotificationReceivedHandler {

  @Override
  public void remoteNotificationReceived(Context context, OSNotificationReceivedEvent notificationReceivedEvent) {

    // https://developer.android.com/training/notify-user/custom-notification
    // Get the layouts to use in the custom notification
    RemoteViews notificationSmallLayoutView = new RemoteViews(context.getPackageName(), R.layout.notification_small);
    RemoteViews notificationLargeLayoutView = new RemoteViews(context.getPackageName(), R.layout.notification_large);

    OSNotification notification = notificationReceivedEvent.getNotification();
    OSMutableNotification mutableNotification = notification.mutableCopy();

    JSONObject data = notification.getAdditionalData();
    String notification_small_image_url;
    String notification_large_image_url;

    if (data != null) {
      notification_small_image_url = data.optString("notification_small_image_url", null);
      notification_large_image_url = data.optString("notification_large_image_url", null);
      if (notification_small_image_url != null && notification_large_image_url != null) {

        notificationSmallLayoutView.setImageViewBitmap(R.id.image_view_notification_small, getBitmapFromURL(notification_small_image_url));
        notificationLargeLayoutView.setImageViewBitmap(R.id.image_view_notification_large, getBitmapFromURL(notification_large_image_url));
        mutableNotification.setExtender(builder -> builder.setCustomContentView(notificationSmallLayoutView)
            .setCustomBigContentView(notificationLargeLayoutView)
            .setStyle(new NotificationCompat.DecoratedCustomViewStyle())//recommended for full background and app title
        );
      }
    }
    // If complete isn't call within a time period of 25 seconds, OneSignal internal logic will show the original notification
    // To omit displaying a notification, pass `null` to complete()
    notificationReceivedEvent.complete(mutableNotification);
  }

  private static Bitmap getBitmapFromURL(String location) {
    try {
      return BitmapFactory.decodeStream(new URL(location).openConnection().getInputStream());
    } catch (Throwable t) {
      Log.i("OneSignal", "COULD NOT DOWNLOAD IMAGE");
    }

    return null;
  }

}
