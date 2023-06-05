package com.bagelcode.v3.push;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;

import com.bagelcode.v3.R;

public class PushReceiver extends BroadcastReceiver {

  @Override
  public void onReceive(Context context, Intent intent) {
    if (context == null) { return; }
    if (intent == null)  { return; }

    String strSender 		  = intent.getStringExtra(context.getString(R.string.noti_title_key));
    String strMessage 		= intent.getStringExtra(context.getString(R.string.noti_body_key));

    NotificationFactory.sendNotification(context, strSender, strMessage, intent.getExtras());
  }
}
