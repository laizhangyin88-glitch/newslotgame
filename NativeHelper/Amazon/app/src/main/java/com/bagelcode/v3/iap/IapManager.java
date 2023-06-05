package com.bagelcode.v3.iap;

import android.content.Context;
import android.util.Log;

import com.amazon.device.iap.PurchasingListener;
import com.amazon.device.iap.PurchasingService;
import com.amazon.device.iap.model.FulfillmentResult;
import com.amazon.device.iap.model.ProductDataResponse;
import com.amazon.device.iap.model.PurchaseResponse;
import com.amazon.device.iap.model.PurchaseUpdatesResponse;
import com.amazon.device.iap.model.Receipt;
import com.amazon.device.iap.model.RequestId;
import com.amazon.device.iap.model.UserData;
import com.amazon.device.iap.model.UserDataResponse;

/**
 * This is a sample of how an application may handle InAppPurchasing. The major
 * functions includes
 * <ul>
 * <li>Simple user and purchase history management</li>
 * <li>Grant Orange purchases</li>
 * <li>Enable/disable purchases from GUI</li>
 * <li>Save persistent order data into SQLite Database and SharedPreference</li>
 * </ul>
 */
public class IapManager {

  public interface OnIapPurchaseFinishedListener {
    void onIapPurchaseFinished(String error, String userId, String receipt);
  }

  private OnIapPurchaseFinishedListener lastPurchaseCallback;

  public void purchase(String sku, OnIapPurchaseFinishedListener callBack)
  {
    this.lastPurchaseCallback = callBack;
    // Request purchase
    final RequestId requestId = PurchasingService.purchase(sku);
    Log.d(TAG, "purchase: requestId (" + requestId + ")");
  }

  private static final String TAG = "IapManager";

  public static final int SUCCESSFUL = 0;
  public static final int ALREADY_PURCHASED = 1;
  public static final int INVALID_SKU = 2;
  public static final int FAILED = 3;
  public static final int NOT_SUPPORTED = 4;
  public static final int UNHANDLED_CASE = 5;
  public static final int NOT_SUPPORTED_PRODUCT_TYPE = 6;
  public static final int STRINGIFY_FAILED = 7;

  public IapManager(Context context) {
    Log.d(TAG, "onCreate: registering PurchasingListener");
    PurchasingService.registerListener(context, mPurchaseingListener);
    Log.d(TAG, "IS_SANDBOX_MODE:" + PurchasingService.IS_SANDBOX_MODE);
  }

  private PurchasingListener mPurchaseingListener = new PurchasingListener() {
    @Override
    public void onUserDataResponse(final UserDataResponse response) {
      Log.e(TAG, "onUserDataResponse: SHOULD NOT BE CALLED");
    }

    @Override
    public void onProductDataResponse(final ProductDataResponse response) {
      Log.e(TAG, "onProductDataResponse: SHOULD NOT BE CALLED");
    }

    /**
     * This is the callback for {@link PurchasingService#getPurchaseUpdates}.
     *
     * We will receive Consumable receipts from this callback if the consumable
     * receipts are not marked as "FULFILLED" in Amazon Appstore. So for every
     * single Consumable receipts in the response, we need to call
     * {@link IapManager#handleReceipt} to fulfill the purchase.
     *
     */
    @Override
    public void onPurchaseUpdatesResponse(final PurchaseUpdatesResponse response) {
      Log.d(TAG, "onPurchaseUpdatesResponse: requestId (" + response.getRequestId()
        + ") purchaseUpdatesResponseStatus ("
        + response.getRequestStatus()
        + ")");
      final PurchaseUpdatesResponse.RequestStatus status = response.getRequestStatus();
      switch (status) {
        case SUCCESSFUL:
          for (final Receipt receipt : response.getReceipts()) {
            // How to handle??? notify to unity????
            PurchasingService.notifyFulfillment(receipt.getReceiptId(), FulfillmentResult.FULFILLED);
          }
          if (response.hasMore()) {
            PurchasingService.getPurchaseUpdates(false);
          }
          break;
        case FAILED:
        case NOT_SUPPORTED:
          Log.d(TAG, "onProductDataResponse: failed, should retry request");
          break;
      }
    }

    /**
     * This is the callback for {@link PurchasingService#purchase}. For each
     * time the application sends a purchase request
     * {@link PurchasingService#purchase}, Amazon Appstore will call this
     * callback when the purchase request is completed. If the RequestStatus is
     * Successful or AlreadyPurchased then application needs to call
     * {@link IapManager#handleReceipt} to handle the purchase
     * fulfillment. If the RequestStatus is INVALID_SKU, NOT_SUPPORTED, or
     * FAILED, notify corresponding method of {@link IapManager} .
     */
    @Override
    public void onPurchaseResponse(final PurchaseResponse response) {
      final String requestId = response.getRequestId().toString();
      final PurchaseResponse.RequestStatus status = response.getRequestStatus();
      Log.d(TAG, "onPurchaseResponse: requestId (" + requestId
        + ") purchaseRequestStatus ("
        + status
        + ")");

      OnIapPurchaseFinishedListener callback = lastPurchaseCallback;
      lastPurchaseCallback = null;

      if (callback == null) {
        Log.e(TAG, "Arguments are not set properly");
        return;
      }

      int responseCode;
      switch (status) {
        case SUCCESSFUL:
          final Receipt receipt = response.getReceipt();
          final UserData userData = response.getUserData();
          Log.d(TAG, "onPurchaseResponse: receipt json:" + receipt.toJSON() + ", user: " + userData.getUserId());
          handleReceipt(userData, receipt, callback);
          return;
        case ALREADY_PURCHASED:
          // This is not applicable for consumable item. It is only application for entitlement and subscription.
          Log.e(TAG, "onPurchaseResponse: already purchased, should never get here for a consumable.");
          responseCode = ALREADY_PURCHASED;
          break;
        case INVALID_SKU:
          Log.d(TAG, "onPurchaseResponse: invalid SKU!");
          responseCode = INVALID_SKU;
          break;
        case FAILED:
          Log.d(TAG, "onPurchaseResponse: failed so remove purchase request from local storage");
          responseCode = FAILED;
          break;
        case NOT_SUPPORTED:
          Log.d(TAG, "onPurchaseResponse: failed so remove purchase request from local storage");
          responseCode = NOT_SUPPORTED;
          break;
        default:
          responseCode = UNHANDLED_CASE;
          break;
      }
      if (responseCode != SUCCESSFUL) {
        callback.onIapPurchaseFinished(String.valueOf(responseCode), null, null);
        return;
      }
    }
  };

  /**
   * This method contains the business logic to fulfill the customer's
   * purchase based on the receipt received from InAppPurchase SDK's
   * {@link PurchasingListener#onPurchaseResponse} or
   * {@link PurchasingListener#onPurchaseUpdatesResponse} method.
   *
   * @param receipt
   */
  public void handleConsumablePurchase(final UserData userData, final Receipt receipt, final OnIapPurchaseFinishedListener callback) {
    if (receipt.isCanceled()) {
      callback.onIapPurchaseFinished("CANCELED", null, null);
      return;
    }
    callback.onIapPurchaseFinished(null, userData.getUserId(), receipt.getReceiptId());
    PurchasingService.notifyFulfillment(receipt.getReceiptId(), FulfillmentResult.FULFILLED);
  }

  public void handleSubscriptionPurchase(final UserData userData, final Receipt receipt, final OnIapPurchaseFinishedListener callback) {
    if (receipt.isCanceled()) {
      callback.onIapPurchaseFinished("CANCELED", null, null);
      return;
    }
    callback.onIapPurchaseFinished(null, userData.getUserId(), receipt.getReceiptId());
    PurchasingService.notifyFulfillment(receipt.getReceiptId(), FulfillmentResult.FULFILLED);
  }

  /**
   * Method to handle the receipt
   *
   * @param receipt
   */
  private void handleReceipt(final UserData userData, final Receipt receipt, final OnIapPurchaseFinishedListener callback) {
    switch (receipt.getProductType()) {
      case CONSUMABLE:
        handleConsumablePurchase(userData, receipt, callback);
        break;
      case SUBSCRIPTION:
        handleSubscriptionPurchase(userData, receipt, callback);
        break;
      case ENTITLED:
        Log.e(TAG, "NOT SUPPORTED PRODUCT TYPE");
        callback.onIapPurchaseFinished(String.valueOf(NOT_SUPPORTED_PRODUCT_TYPE), null, null);
        break;
    }
  }
}
