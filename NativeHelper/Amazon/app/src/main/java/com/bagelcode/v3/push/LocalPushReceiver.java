package com.bagelcode.v3.push;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;

import com.bagelcode.v3.R;

import android.util.Log;

public class LocalPushReceiver extends BroadcastReceiver {

  private static final String TAG = "BagelCode_Notification";

  @Override
  public void onReceive(Context context, Intent intent) {
    if (context == null) { return; }
    if (intent == null)  { return; }

    final String titleKey = context.getString(R.string.json_data_title_key);
    final String msgKey = context.getString(R.string.json_data_msg_key);

    String strSender = intent.getStringExtra(titleKey);
    String strMessage = intent.getStringExtra(msgKey);

    Log.d(TAG, "LocalPushReceiver onReceive");

    NotificationFactory.sendNotification(context, strSender, strMessage, intent.getExtras());
  }
}
