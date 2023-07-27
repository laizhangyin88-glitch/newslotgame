#include "PurchaseManagerWindows.h"

PLUGIN_API action_str_t initializePurchaseFn = nullptr;
PLUGIN_API action_str_int_str_t purchaseFn = nullptr;
PLUGIN_API action_str_t consumeUnclaimedPurchaseFn = nullptr;
PLUGIN_API action_str_str_t reportConsumableProductsAsFulfilledFn = nullptr;
PLUGIN_API action_str_str_str_t getAndUpdateMicrosoftStoreIdKeyFn = nullptr;

extern "C" {
	void _stdcall initializePurchase(const wchar_t *name) {
		if (initializePurchaseFn != nullptr) {
			initializePurchaseFn(name);
		}
	}

	void _stdcall purchase(const wchar_t *storeId, int productId, const wchar_t *callback) {
		if (purchaseFn != nullptr) {
			purchaseFn(storeId, productId, callback);
		}
	}

	void _stdcall consumeUnclaimedPurchase(const wchar_t *callback) {
		if (consumeUnclaimedPurchaseFn != nullptr) {
			consumeUnclaimedPurchaseFn(callback);
		}
	}

	void _stdcall reportConsumableProductsAsFulfilled(const wchar_t *storeId, const wchar_t *callback) {
		if (reportConsumableProductsAsFulfilledFn != nullptr) {
			reportConsumableProductsAsFulfilledFn(storeId, callback);
		}
	}

	void _stdcall getAndUpdateMicrosoftStoreIdKey(const wchar_t *azureAdCollectionsToken, const wchar_t *userId, const wchar_t *callback) {
		if (getAndUpdateMicrosoftStoreIdKeyFn != nullptr) {
			getAndUpdateMicrosoftStoreIdKeyFn(azureAdCollectionsToken, userId, callback);
		}
	}
}
