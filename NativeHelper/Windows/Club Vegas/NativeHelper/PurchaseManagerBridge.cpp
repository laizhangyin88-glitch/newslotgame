#include "pch.h"
#include "NativeHelper/PurchaseManagerBridge.h"
#include "purchase/PurchaseManager.h"
#include "lib/utils.h"
#include <concurrent_vector.h>

#define GENERATED_PROJECT 1
#include "..\Il2CppOutputProject\Source\CppPlugins\PurchaseManagerWindows.h"
#include "..\Il2CppOutputProject\Source\CppPlugins\NativeHelperWindows.h"

using namespace Platform;
using namespace concurrency;

String^ purchaseManagerUnityObject = nullptr;

void _stdcall initializePurchase(const wchar_t* _purchaseManagerUnityObject) {
	purchaseManagerUnityObject = ref new String(_purchaseManagerUnityObject);
	RunOnWindowsUIThread([]() {
		Purchase::PurchaseManager::getInstance();
	});
}

void _stdcall purchase(const wchar_t* storeId, int productId, const wchar_t* _callback) {
	String^ offerToken = ref new String(storeId);
	String^ callback = ref new String(_callback);
	RunOnWindowsUIThread([offerToken, productId, callback]() {
		Purchase::PurchaseManager::getInstance()->purchaseProduct(offerToken, productId, [callback](bool success, String^ receipt) {
			RunOnUnityAppThread([callback, success, receipt]() {
				if (success) {
					unitySendMessage(purchaseManagerUnityObject->Data(), callback->Data(), receipt->Data());
				} else {
					unitySendMessage(purchaseManagerUnityObject->Data(), callback->Data(), String::Concat("Error", receipt)->Data());
				}
			});
		});
	});
}


void _stdcall consumeUnclaimedPurchase(const wchar_t* _callback) {
	String^ callback = ref new String(_callback);
	RunOnWindowsUIThread([callback]() {
		Purchase::PurchaseManager::getInstance()->consumeUnfulfilledProducts([callback](bool success, Platform::String^ errMsg, Platform::String^ unfulfilledPurchaseInfo) {
			RunOnUnityAppThread([callback, success, errMsg, unfulfilledPurchaseInfo]() {
				if (success && unfulfilledPurchaseInfo != nullptr) {
					unitySendMessage(purchaseManagerUnityObject->Data(), callback->Data(), unfulfilledPurchaseInfo->Data());
				} else {
					unitySendMessage(purchaseManagerUnityObject->Data(), callback->Data(), errMsg->Data());
				}
			});
		});
	});
}

void _stdcall reportConsumableProductsAsFulfilled(const wchar_t* _storeId, const wchar_t* _callback) {
	String^ storeId = ref new String(_storeId);
	String^ callback = ref new String(_callback);
	RunOnWindowsUIThread([storeId, callback]() {
		Purchase::PurchaseManager::getInstance()->fulfill(storeId).then([callback](bool success) {
			RunOnUnityAppThread([callback, success]() {
				if (success) {
					unitySendMessage(purchaseManagerUnityObject->Data(), callback->Data(), L"Success");
				} else {
					unitySendMessage(purchaseManagerUnityObject->Data(), callback->Data(), L"Fail");
				}
			});
		}, task_continuation_context::use_current());
	});
}

void _stdcall getAndUpdateMicrosoftStoreIdKey(const wchar_t* _azureAdCollectionsToken, const wchar_t* _userId, const wchar_t* _callback) {
	String^ azureAdCollectionsToken = ref new String(_azureAdCollectionsToken);
	String^ userId = ref new String(_userId);
	String^ callback = ref new String(_callback);
	RunOnWindowsUIThread([azureAdCollectionsToken, userId, callback]() {
		Purchase::PurchaseManager::getInstance()->getAndUpdateMicrosoftStoreIdKey(azureAdCollectionsToken, userId, [callback](bool success, Platform::String^ msStoreIdKey) {
			RunOnUnityAppThread([callback, success, msStoreIdKey]() {
				unitySendMessage(purchaseManagerUnityObject->Data(), callback->Data(), msStoreIdKey->Data());
			});
		});
	});
}

void initPurchaseManager() {
	initializePurchaseFn = initializePurchase;
	purchaseFn = purchase;
	consumeUnclaimedPurchaseFn = consumeUnclaimedPurchase;
	reportConsumableProductsAsFulfilledFn = reportConsumableProductsAsFulfilled;
	getAndUpdateMicrosoftStoreIdKeyFn = getAndUpdateMicrosoftStoreIdKey;
}
