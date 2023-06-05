/*
 * [SampleADMMessageHandler.java]
 *
 * (c) 2012, Amazon.com, Inc. or its affiliates. All Rights Reserved.
 */

package com.bagelcode.v3.push;

import java.util.HashMap;
import java.util.Map;
import java.util.Set;

import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.content.Context;

import com.amazon.device.messaging.ADMConstants;
import com.amazon.device.messaging.ADMMessageHandlerBase;
import com.amazon.device.messaging.ADMMessageReceiver;
import com.bagelcode.v3.R;

/**
 * The SampleADMMessageHandler class receives messages sent by ADM via the MessageAlertReceiver receiver.
 *
 * @version Revision: 1, Date: 11/11/2012
 */
public class ADMMessageHandler extends ADMMessageHandlerBase
{
    /** Tag for logs. */
    private final static String TAG = "ADMMessageHandler";

    /**
     * The MessageAlertReceiver class listens for messages from ADM and forwards them to the 
     * SampleADMMessageHandler class.
     */
    public static class MessageAlertReceiver extends ADMMessageReceiver
    {
        /** {@inheritDoc} */
        public MessageAlertReceiver()
        {
            super(ADMMessageHandler.class);
        }
    }

    /**
     * Class constructor.
     */
    public ADMMessageHandler()
    {
        super(ADMMessageHandler.class.getName());
    }

    /**
     * Class constructor, including the className argument.
     * 
     * @param className The name of the class.
     */
    public ADMMessageHandler(final String className)
    {
        super(className);
    }

    /** {@inheritDoc} */
    @Override
    protected void onMessage(final Intent intent) 
    {
        Log.i(TAG, "SampleADMMessageHandler:onMessage");

        /* String to access message field from data JSON. */
        final String msgKey = getString(R.string.json_data_msg_key);

        /* String to access title field from data JSON. */
        final String titleKey = getString(R.string.json_data_title_key);

        /* String to access silent field from data JSON. */
        final String silentKey = getString(R.string.json_data_silent_key);

        /* Extras that were included in the intent. */
        final Bundle extras = intent.getExtras();
        
        verifyMD5Checksum(extras);
        
        /* Extract message from the extras in the intent. */
        final String msg = extras.getString(msgKey);
        final String title = extras.getString(titleKey);
        final String silent = extras.getString(silentKey);

        if ("true".equals(silent)) {
          return;
        }

        if (msg == null || title == null)
        {
            Log.w(TAG, "onMessage Unable to extract message data." +
                    "Make sure that msgKey and titleKey values match data elements of your JSON message");
            return;
        }

        /* Create a notification with message data. */
        /* This is required to test cases where the app or device may be off. */
        final Context context = getApplicationContext();
        NotificationFactory.sendNotification(context, title, msg, extras);
    }

    /**
     * This method verifies the MD5 checksum of the ADM message.
     * 
     * @param extras Extra that was included with the intent.
     */
    private void verifyMD5Checksum(final Bundle extras) 
    {
        /* String to access consolidation key field from data JSON. */
        final String consolidationKey = getString(R.string.json_data_consolidation_key);
        
        final Set<String> extrasKeySet = extras.keySet();
        final Map<String, String> extrasHashMap = new HashMap<String, String>();
        for (String key : extrasKeySet)
        {
            if (!key.equals(ADMConstants.EXTRA_MD5) && !key.equals(consolidationKey))
            {
                extrasHashMap.put(key, extras.getString(key));
            }            
        }
        final String md5 = ADMSampleMD5ChecksumCalculator.calculateChecksum(extrasHashMap);
        Log.i(TAG, "SampleADMMessageHandler:onMessage App md5: " + md5);
        
        /* Extract md5 from the extras in the intent. */
        final String admMd5 = extras.getString(ADMConstants.EXTRA_MD5);
        Log.i(TAG, "SampleADMMessageHandler:onMessage ADM md5: " + admMd5);
        
        /* Data integrity check. */
        if(!admMd5.trim().equals(md5.trim()))
        {
            Log.w(TAG, "SampleADMMessageHandler:onMessage MD5 checksum verification failure. " +
                "Message received with errors");
        }
    }

    /** {@inheritDoc} */
    @Override
    protected void onRegistrationError(final String string)
    {
        Log.e(TAG, "SampleADMMessageHandler:onRegistrationError " + string);
    }

    /** {@inheritDoc} */
    @Override
    protected void onRegistered(final String registrationId) 
    {
        Log.i(TAG, "SampleADMMessageHandler:onRegistered");
        Log.i(TAG, registrationId);

        /* Register the app instance's registration ID with your server. */
    }

    /** {@inheritDoc} */
    @Override
    protected void onUnregistered(final String registrationId) 
    {
        Log.i(TAG, "SampleADMMessageHandler:onUnregistered");

        /* Unregister the app instance's registration ID with your server. */
    }
}
