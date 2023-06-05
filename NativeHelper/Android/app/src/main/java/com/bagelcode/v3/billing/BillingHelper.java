/*
 * Copyright (C) 2021 Google Inc. All rights reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

package com.bagelcode.v3.billing;

import android.app.Activity;
import android.app.Application;
import android.os.Handler;
import android.os.Looper;
import android.os.SystemClock;
import android.util.Log;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.lifecycle.Lifecycle;
import androidx.lifecycle.LifecycleObserver;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.MediatorLiveData;
import androidx.lifecycle.MutableLiveData;
import androidx.lifecycle.OnLifecycleEvent;
import androidx.lifecycle.Transformations;

import com.android.billingclient.api.AcknowledgePurchaseParams;
import com.android.billingclient.api.BillingClient;
import com.android.billingclient.api.BillingClientStateListener;
import com.android.billingclient.api.BillingFlowParams;
import com.android.billingclient.api.BillingResult;
import com.android.billingclient.api.ConsumeParams;
import com.android.billingclient.api.Purchase;
import com.android.billingclient.api.PurchasesUpdatedListener;
import com.android.billingclient.api.SkuDetails;
import com.android.billingclient.api.SkuDetailsParams;
import com.android.billingclient.api.SkuDetailsResponseListener;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.HashSet;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.Set;

public class BillingHelper implements LifecycleObserver
                                          , PurchasesUpdatedListener
                                          , BillingClientStateListener
                                          // , SkuDetailsResponseListener
{
    private enum SkuState
    {
        SKU_STATE_UNPURCHASED,
        SKU_STATE_PENDING,
        SKU_STATE_PURCHASED,
        SKU_STATE_PURCHASED_AND_ACKNOWLEDGED,
    }

    private static volatile BillingHelper sInstance;

    private static final String TAG = "UnityPlayerActivity";

    private static final long RECONNECT_TIMER_START_MILLISECONDS = 1L * 1000L;
    private static final long RECONNECT_TIMER_MAX_TIME_MILLISECONDS = 1000L * 60L * 15L; // 15 mins
    private static final long SKU_DETAILS_REQUERY_TIME = 1000L * 60L * 60L * 4L; // 4 hours
    private static final Handler handler = new Handler(Looper.getMainLooper());

    private boolean billingSetupComplete = false;

    private final BillingClient billingClient;

    final private Map<String, MutableLiveData<SkuDetails>> skuDetailsLiveDataMap = new HashMap<>();

    // Observables that are used to communicate state.
    final private Set<Purchase> purchaseConsumptionInProcess = new HashSet<>();
    final private MutableLiveData<Boolean> billingFlowInProcess = new MutableLiveData<>();

    // how long before the data source tries to reconnect to Google play
    private long reconnectMilliseconds = RECONNECT_TIMER_START_MILLISECONDS;

    // when was the last successful SkuDetailsResponse?
    private long skuDetailsResponseTime = -SKU_DETAILS_REQUERY_TIME;

    public interface BillingHelperListener
    {
        public void onPurchaseFinishedListener(int billingResponseCode, String message, Purchase purchase);
        public void onConsumeFinished(Purchase purchase);
        public void onPurchaseSkuDetails(SkuDetails skuDetails);
    }

    BillingHelperListener mListener;

    private BillingHelper(@NonNull Activity activity, BillingHelperListener listener)
    {
        billingClient = BillingClient.newBuilder(activity)
                        .setListener(this)
                        .enablePendingPurchases()
                        .build();

        billingClient.startConnection(this);
        mListener = listener;
        billingFlowInProcess.postValue(false);
    }

    // Standard boilerplate double check locking pattern for thread-safe singletons.
    public static BillingHelper getInstance(@NonNull Activity activity, BillingHelperListener listener)
    {
        if (sInstance == null)
        {
            synchronized (BillingHelper.class)
            {
                if (sInstance == null)
                {
                    sInstance = new BillingHelper(activity, listener);
                }
            }
        }
        return sInstance;
    }

    public void dispose()
    {
        mListener = null;
    }

    // billingClient.startConnection(this); response
    @Override
    public void onBillingSetupFinished(BillingResult billingResult)
    {
        int responseCode = billingResult.getResponseCode();
        String debugMessage = billingResult.getDebugMessage();
        Log.d(TAG, "onBillingSetupFinished: " + responseCode + " " + debugMessage);
        switch (responseCode)
        {
            case BillingClient.BillingResponseCode.OK:
                // The billing client is ready. You can query purchases here.
                // This doesn't mean that your app is set up correctly in the console -- it just
                // means that you have a connection to the Billing service.
                reconnectMilliseconds = RECONNECT_TIMER_START_MILLISECONDS;
                billingSetupComplete = true;
                break;
            default:
                retryBillingServiceConnectionWithExponentialBackoff();
                break;
        }
    }


     // billingClient.startConnection(this); response
    /**
     * This is a pretty unusual occurrence. It happens primarily if the Google Play Store
     * self-upgrades or is force closed.
     */
    @Override
    public void onBillingServiceDisconnected()
    {
        billingSetupComplete = false;
        retryBillingServiceConnectionWithExponentialBackoff();
    }

    /**
     * Retries the billing service connection with exponential backoff, maxing out at the time
     * specified by RECONNECT_TIMER_MAX_TIME_MILLISECONDS.
     */
    private void retryBillingServiceConnectionWithExponentialBackoff()
    {
        handler.postDelayed(() -> billingClient.startConnection(this), reconnectMilliseconds);
        reconnectMilliseconds = Math.min(reconnectMilliseconds * 2, RECONNECT_TIMER_MAX_TIME_MILLISECONDS);
    }

    public void consumeUnclaimedPurchases(){
        billingClient.queryPurchasesAsync(BillingClient.SkuType.INAPP,
            (billingResult, list) ->
            {
                if (billingResult.getResponseCode() != BillingClient.BillingResponseCode.OK)
                {
                    Log.d(TAG, "Problem getting purchases: " + billingResult.getDebugMessage());
                    mListener.onPurchaseFinishedListener(billingResult.getResponseCode(), billingResult.getDebugMessage(), null);
                }
                else
                {
                    for (Purchase purchase : list)
                    {
                        // for right now any bundle of SKUs must all be consumable
                        for ( String purchaseSku : purchase.getSkus() )
                        {
                            if(purchase.getPurchaseState() == Purchase.PurchaseState.PURCHASED){
                                mListener.onPurchaseFinishedListener(BillingClient.BillingResponseCode.OK, "", purchase);
                            }
                            else if(purchase.getPurchaseState() == Purchase.PurchaseState.PENDING) {
                                // Pending is skip. 
                                Log.d(TAG, "consumeUnclaimedPurchases. Pending. Skip");
                                break;
                            }
                            else {
                                mListener.onPurchaseFinishedListener(BillingClient.BillingResponseCode.ERROR, getPurchaseStateText(purchase), null);
                            }
                            return;
                        }
                    }

                    mListener.onPurchaseFinishedListener(BillingClient.BillingResponseCode.OK, "NO_UNCLAIMED_PURCHASE", null);
                    return;
                }
            }
        );
    }

    public void consumeInappPurchase(@NonNull String sku)
    {
        billingClient.queryPurchasesAsync(BillingClient.SkuType.INAPP,
            (billingResult, list) ->
            {
                if (billingResult.getResponseCode() != BillingClient.BillingResponseCode.OK)
                {
                    Log.d(TAG, "Problem getting purchases: " + billingResult.getDebugMessage());
                }
                else
                {
                    for (Purchase purchase : list)
                    {
                        // for right now any bundle of SKUs must all be consumable
                        for ( String purchaseSku : purchase.getSkus() )
                        {
                            if (purchaseSku.equals(sku))
                            {
                                consumePurchase(purchase);
                                return;
                            }
                        }
                    }
                }
                Log.d(TAG, "Unable to consume SKU: " + sku + " Sku not found.");
            }
        );
    }

    private String getPurchaseStateText(@NonNull Purchase purchase)
    {
        String state = "UNKNOWN";
        switch (purchase.getPurchaseState())
        {
            case Purchase.PurchaseState.PENDING:
                state = "PENDING";
                break;
            case Purchase.PurchaseState.UNSPECIFIED_STATE:
                state = "UNSPECIFIED_STATE";
                break;
            case Purchase.PurchaseState.PURCHASED:
                state = "PURCHASED";
                break;
            default:
                Log.d(TAG, "Purchase in unknown state: " + purchase.getPurchaseState());
                break;
        }

        return state;
    }

    private void processPurchaseList(List<Purchase> purchases, List<String> skusToUpdate)
    {
        HashSet<String> updatedSkus = new HashSet<>();
        if (null != purchases)
        {
            for (final Purchase purchase : purchases)
            {
                int purchaseState = purchase.getPurchaseState();
                if (purchaseState == Purchase.PurchaseState.PURCHASED)
                {
                    mListener.onPurchaseFinishedListener(BillingClient.BillingResponseCode.OK, "", purchase);
                }
                else if(purchaseState == Purchase.PurchaseState.PENDING)
                {
                    Log.d(TAG, "processPurchaseList. Pending.");
                    // Todo : pending wait process. 
                    mListener.onPurchaseFinishedListener(BillingClient.BillingResponseCode.OK, "PENDING", purchase);
                }
                else
                {
                    // make sure the state is set
                    mListener.onPurchaseFinishedListener(BillingClient.BillingResponseCode.ERROR, getPurchaseStateText(purchase), null);
                }
            }
        }
        else
        {
            Log.d(TAG, "Empty purchase list.");

            mListener.onPurchaseFinishedListener(BillingClient.BillingResponseCode.ERROR, "Empty purchase list.", null);
        }
    }

    /**
     * Internal call only. Assumes that all signature checks have been completed and the purchase is
     * ready to be consumed. If the sku is already being consumed, does nothing.
     *
     * @param purchase purchase to consume
     */
    private void consumePurchase(@NonNull Purchase purchase)
    {
        // weak check to make sure we're not already consuming the sku
        if (purchaseConsumptionInProcess.contains(purchase))
        {
            // already consuming
            return;
        }

        purchaseConsumptionInProcess.add(purchase);
        billingClient.consumeAsync(ConsumeParams.newBuilder()
                .setPurchaseToken(purchase.getPurchaseToken())
                .build(), (billingResult, s) ->
        {
            // ConsumeResponseListener
            purchaseConsumptionInProcess.remove(purchase);
            if (billingResult.getResponseCode() == BillingClient.BillingResponseCode.OK)
            {
                Log.d(TAG, "Consumption successful. Delivering entitlement.");
                mListener.onConsumeFinished(purchase);
            }
            else
            {
                Log.d(TAG, "Error while consuming: " + billingResult.getDebugMessage());
            }
            Log.d(TAG, "End consumption flow.");
        });
    }

    public void querySkuDetails(String skuType, List<String> skuList, SkuDetailsResponseListener listener)
    {
        if (null != skuList && !skuList.isEmpty())
        {
            SkuDetailsParams.Builder params = SkuDetailsParams.newBuilder();
            params.setSkusList(skuList);
            params.setType(skuType);

            billingClient.querySkuDetailsAsync(params.build(), listener);
        }
    }

    // skuType : BillingClient.SkuType.INAPP, BillingClient.SkuType.SUBS
    public void launchBillingFlow(Activity activity, String skuType, @NonNull String sku)
    {
        Log.d(TAG, "launchBillingFlow");
        LiveData<SkuDetails> skuDetailsLiveData = skuDetailsLiveDataMap.get(sku);
        if( skuDetailsLiveData != null && skuDetailsLiveData.getValue() != null )
        {
            BillingProcess(activity, skuDetailsLiveData.getValue());
        }
        else
        {
            List<String> skuList = new ArrayList<String>() {{ add(sku); }};
            querySkuDetails(skuType, skuList,

                new SkuDetailsResponseListener()
                {
                    @Override
                    public void onSkuDetailsResponse(BillingResult billingResult, List<SkuDetails> skuDetailsList)
                    {
                        Log.d(TAG, "onSkuDetailsResponse");
                        // Process the result.
                        int responseCode = billingResult.getResponseCode();
                        String debugMessage = billingResult.getDebugMessage();

                        if(responseCode == BillingClient.BillingResponseCode.OK)
                        {
                            Log.d(TAG, "onSkuDetailsResponse: " + responseCode + " " + debugMessage);
                            if (skuDetailsList == null || skuDetailsList.isEmpty())
                            {
                                Log.d(TAG, "onSkuDetailsResponse: " +
                                        "Found null or empty SkuDetails. " +
                                        "Check to see if the SKUs you requested are correctly published " +
                                        "in the Google Play Console.");
                                 mListener.onPurchaseFinishedListener(BillingClient.BillingResponseCode.ERROR, "Found null or empty SkuDetails.", null);
                            }
                            else
                            {
                                Log.d(TAG, "Update Details");
                                SkuDetails targetPurchaseSkuDetails = null;
                                for (SkuDetails skuDetails : skuDetailsList)
                                {
                                    targetPurchaseSkuDetails = skuDetails;
                                    String responseSku = skuDetails.getSku();
                                    MutableLiveData<SkuDetails> detailsMutableLiveData = skuDetailsLiveDataMap.get(responseSku);
                                    if (null != detailsMutableLiveData)
                                    {
                                        Log.d(TAG, "Update New Details");
                                        detailsMutableLiveData.postValue(skuDetails);
                                    }
                                    else
                                    {
                                        Log.d(TAG, "Add New Details: " + responseSku);
                                        MutableLiveData<SkuDetails> details = new MutableLiveData<SkuDetails>();
                                        details.postValue(skuDetails);
                                        skuDetailsLiveDataMap.put(responseSku, details);
                                    }
                                }

                                // We can only purchase one sku. We can't buy multiple sku.
                                Log.d(TAG, "Check SkuDetails");
                                if(targetPurchaseSkuDetails != null)
                                {
                                    Log.d(TAG, "Call BillingProcess");
                                    BillingProcess(activity, targetPurchaseSkuDetails);
                                }
                                else
                                {
                                    mListener.onPurchaseFinishedListener(BillingClient.BillingResponseCode.ERROR, "Empty SkuDetails.", null);
                                }
                            } 
                        }
                        else
                        {
                            switch (responseCode)
                            {
                                case BillingClient.BillingResponseCode.SERVICE_DISCONNECTED:
                                case BillingClient.BillingResponseCode.SERVICE_UNAVAILABLE:
                                case BillingClient.BillingResponseCode.BILLING_UNAVAILABLE:
                                case BillingClient.BillingResponseCode.ITEM_UNAVAILABLE:
                                case BillingClient.BillingResponseCode.DEVELOPER_ERROR:
                                case BillingClient.BillingResponseCode.ERROR:
                                    Log.d(TAG, "onSkuDetailsResponse: " + responseCode + " " + debugMessage);
                                    break;
                                case BillingClient.BillingResponseCode.USER_CANCELED:
                                    Log.d(TAG, "onSkuDetailsResponse: " + responseCode + " " + debugMessage);
                                    break;
                                // These response codes are not expected.
                                case BillingClient.BillingResponseCode.FEATURE_NOT_SUPPORTED:
                                case BillingClient.BillingResponseCode.ITEM_ALREADY_OWNED:
                                case BillingClient.BillingResponseCode.ITEM_NOT_OWNED:
                                default:
                                    Log.d(TAG, "onSkuDetailsResponse: " + responseCode + " " + debugMessage);
                                    break;
                            }
                            mListener.onPurchaseFinishedListener(BillingClient.BillingResponseCode.ERROR, responseCode + " " + debugMessage, null);
                        }
                    }
                }
            );
        }
    }

    public void BillingProcess(Activity activity, SkuDetails skuDetails)
    {
        Log.d(TAG, "BillingProcess");

        if (null != skuDetails)
        {
            Log.d(TAG, skuDetails.getType());

            mListener.onPurchaseSkuDetails(skuDetails);

            BillingFlowParams.Builder billingFlowParamsBuilder = BillingFlowParams.newBuilder();
            billingFlowParamsBuilder.setSkuDetails(skuDetails);
            BillingResult br = billingClient.launchBillingFlow(activity, billingFlowParamsBuilder.build());
            if (br.getResponseCode() == BillingClient.BillingResponseCode.OK)
            {
                billingFlowInProcess.postValue(true);
            }
            else
            {
                mListener.onPurchaseFinishedListener(br.getResponseCode(), br.getDebugMessage(), null);
                Log.d(TAG, "Billing failed: + " + br.getDebugMessage());
            }
        }
        else
        {
            // Todo make billing result. Not found SkuDetails.
            mListener.onPurchaseFinishedListener(BillingClient.BillingResponseCode.ERROR, "SkuDetails not found", null);
            Log.d(TAG, "SkuDetails is null");
        }
    }

    public LiveData<Boolean> getBillingFlowInProcess()
    {
        return billingFlowInProcess;
    }

    public SkuDetails GetSkuDetails(@NonNull String sku)
    {
        LiveData<SkuDetails> skuDetailsLiveData = skuDetailsLiveDataMap.get(sku);
        if( skuDetailsLiveData != null && skuDetailsLiveData.getValue() != null )
            return skuDetailsLiveData.getValue();

        return null;
    }

    /**
     * Called by the BillingLibrary when new purchases are detected; typically in response to a
     * launchBillingFlow.
     *
     * @param billingResult result of the purchase flow.
     * @param list          of new purchases.
     */
    @Override
    public void onPurchasesUpdated(@NonNull BillingResult billingResult, @Nullable List<Purchase> list)
    {
        switch (billingResult.getResponseCode())
        {
            case BillingClient.BillingResponseCode.OK:
                if (null != list)
                {
                    processPurchaseList(list, null);
                    return;
                }
                else
                {
                    Log.d(TAG, "Null Purchase List Returned from OK response!");
                }
                break;
            case BillingClient.BillingResponseCode.USER_CANCELED:
                Log.d(TAG, "onPurchasesUpdated: User canceled the purchase");
                mListener.onPurchaseFinishedListener(BillingClient.BillingResponseCode.ERROR, "USER_CANCELED", null);
                break;
            case BillingClient.BillingResponseCode.ITEM_ALREADY_OWNED:
                Log.d(TAG, "onPurchasesUpdated: The user already owns this item");
                mListener.onPurchaseFinishedListener(BillingClient.BillingResponseCode.ERROR, "ITEM_ALREADY_OWNED", null);
                break;
            case BillingClient.BillingResponseCode.DEVELOPER_ERROR:
                Log.d(TAG, "onPurchasesUpdated: Developer error means that Google Play " +
                        "does not recognize the configuration. If you are just getting started, " +
                        "make sure you have configured the application correctly in the " +
                        "Google Play Console. The SKU product ID must match and the APK you " +
                        "are using must be signed with release keys."
                );
                mListener.onPurchaseFinishedListener(BillingClient.BillingResponseCode.ERROR, "DEVELOPER_ERROR", null);
                break;
            default:
                Log.d(TAG, "BillingResult [" + billingResult.getResponseCode() + "]: "
                        + billingResult.getDebugMessage());
                mListener.onPurchaseFinishedListener(BillingClient.BillingResponseCode.ERROR, billingResult.getResponseCode() + " " + billingResult.getDebugMessage(), null);
                break;
        }
        billingFlowInProcess.postValue(false);
    }

    public Boolean isSubscriptionSupported()
    {
        if(billingClient == null || billingSetupComplete == false)
        {
            Log.d(TAG, "isSubscriptionSupported : not yet set.");
            return false;
        }

        BillingResult response = billingClient.isFeatureSupported(BillingClient.FeatureType.SUBSCRIPTIONS);

        switch (response.getResponseCode())
        {
            case BillingClient.BillingResponseCode.OK:
                Log.d(TAG, "isSubscriptionSupported : OK");
                return true;
            case BillingClient.BillingResponseCode.SERVICE_DISCONNECTED:
                Log.d(TAG, "isSubscriptionSupported : SERVICE_DISCONNECTED");
                return false;
            default:
                Log.d(TAG, "isSubscriptionSupported : Subscriptions support check: error -> " + response.getResponseCode() + " " + response.getDebugMessage());
                return false;
        }
    }

    /**
     * It's recommended to requery purchases during onResume.
     */
    // https://developer.android.com/reference/androidx/lifecycle/Lifecycle.Event
    @OnLifecycleEvent(Lifecycle.Event.ON_RESUME)
    public void resume()
    {
        Log.d(TAG, "ON_RESUME");
        // Boolean billingInProcess = billingFlowInProcess.getValue();

        // // this just avoids an extra purchase refresh after we finish a billing flow
        // if (billingSetupComplete && (null == billingInProcess || !billingInProcess))
        // {
        //     refreshPurchasesAsync();
        // }
    }
}
