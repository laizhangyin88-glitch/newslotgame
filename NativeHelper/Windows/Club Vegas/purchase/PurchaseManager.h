#pragma once

namespace Purchase
{
	class PurchaseManager
	{
	private:
		static PurchaseManager* instance;
		Windows::Services::Store::StoreContext^ storeContext;
		Platform::String^ microsoftStoreIdKey;
		Platform::String^ errorReason;
		PurchaseManager();

	public:
		static PurchaseManager* getInstance();
		void getAndUpdateMicrosoftStoreIdKey(Platform::String^ azureAdCollectionsToken, Platform::String^ userId, std::function<void(bool, Platform::String^)> callback);
		concurrency::task<bool> fulfill(Platform::String^ storeId);
		void consumeUnfulfilledProducts(std::function<void(bool, Platform::String^, Platform::String^)> callback);
		void purchaseProduct(Platform::String^ offerStoreID, int productIdNumber, std::function<void(bool, Platform::String^)> callback);
		void purchaseProductWithStoreID(Platform::String^ storeId, Platform::String^ currencyCode, Platform::String^ price, int productIdNumber, std::function<void(bool, Platform::String^)> callback);
	};
}
