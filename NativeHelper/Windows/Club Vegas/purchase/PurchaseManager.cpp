#include "pch.h"
#include "lib/base64.h"
#include "purchase/PurchaseManager.h"
#include <concurrent_vector.h>
#include <vector>
#include <iostream>
#include <sstream>

using namespace Purchase;

using namespace Windows::ApplicationModel::Store;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Services::Store;
using namespace Windows::Storage;
using namespace Windows::Data::Json;
using namespace Platform;
using namespace concurrency;

PurchaseManager* PurchaseManager::instance = nullptr;
String^ purchaseIdHistoryFileName = ref new String(L"product_id_history.txt");

PurchaseManager::PurchaseManager() {
	storeContext = StoreContext::GetDefault();
	microsoftStoreIdKey = nullptr;
	errorReason = "State init";
}

PurchaseManager* PurchaseManager::getInstance() {
	if (PurchaseManager::instance == nullptr) {
		PurchaseManager::instance = new PurchaseManager();
	}
	return PurchaseManager::instance;
}

void PurchaseManager::getAndUpdateMicrosoftStoreIdKey(Platform::String^ azureAdCollectionsToken, Platform::String^ userId, std::function<void(bool, String^)> callback) {
	errorReason = "GetCustomerCollectionsIdAsync : State begin";
	IAsyncOperation<String^>^ getCustomerCollectionsIdAsync;	
	try {
		getCustomerCollectionsIdAsync = storeContext->GetCustomerCollectionsIdAsync(azureAdCollectionsToken, userId);
	} catch (Platform::Exception^ exception) {
		errorReason = "GetCustomerCollectionsIdAsync : " + exception->Message;
		callback(false, "");
		return;
	}
	create_task(getCustomerCollectionsIdAsync).then([this, callback](String^ msStoreIdKey) {
		microsoftStoreIdKey = msStoreIdKey;
		errorReason = "GetCustomerCollectionsIdAsync : Sucesss";
		callback(true, msStoreIdKey);
	}, task_continuation_context::use_current());
}

void PurchaseManager::consumeUnfulfilledProducts(std::function<void(bool, String^, String^)> callback) {
	if (microsoftStoreIdKey == nullptr) {
		callback(false, "NO_UNCLAIMED_PURCHASE", nullptr);
		return;
	}
	IVector<String ^>^ productKinds = ref new Platform::Collections::Vector<String^>();
	productKinds->Append("UnmanagedConsumable");
	IAsyncOperation<StoreProductQueryResult^>^ getUserCollectionAsync;
	try {
		getUserCollectionAsync = storeContext->GetUserCollectionAsync(productKinds);
	} catch (Platform::Exception^ exception) {
		callback(false, exception->Message, nullptr);
		return;
	}
	create_task(getUserCollectionAsync).then([this, callback](StoreProductQueryResult^ queryResult) {
		IMapView<String^, StoreProduct^>^ products = queryResult->Products;
		if (products->Size > 0) {
			IKeyValuePair<String^, StoreProduct^>^ targetPurchase = products->First()->Current;
			try {
				StorageFolder^ storageFolder = ApplicationData::Current->LocalCacheFolder;
				IAsyncOperation<StorageFile^>^ getFileAsync = storageFolder->GetFileAsync(purchaseIdHistoryFileName);
				create_task(getFileAsync).then([](StorageFile^ historyFile) {
					return FileIO::ReadLinesAsync(historyFile);
				}).then([this, targetPurchase, callback](task<IVector<String^>^> readLinesOperation) {
					try {
						IVector<String^>^ historyVector = readLinesOperation.get();
						StringMap^ productIdMap = ref new Windows::Foundation::Collections::StringMap();
						std::for_each(begin(historyVector), end(historyVector), [&productIdMap](String^ history) {
							std::wstring historyData(history->Data());
							std::wstringstream wss(historyData);
							std::wstring storeId;
							std::wstring serverProductId;
							std::getline(wss, storeId, L',');
							std::getline(wss, serverProductId, L',');
							productIdMap->Insert(ref new String(storeId.c_str()), ref new String(serverProductId.c_str()));
						});
						String^ storeId = targetPurchase->Value->StoreId;
						if (productIdMap->HasKey(storeId)) {
							String^ serverProductId = productIdMap->Lookup(storeId);
							JsonObject^ retrievedReceipt = ref new JsonObject();
							retrievedReceipt->SetNamedValue(L"receipt", JsonValue::CreateStringValue(storeId + "|" + microsoftStoreIdKey));
							retrievedReceipt->SetNamedValue(L"serverProductId", JsonValue::CreateStringValue(serverProductId));
							retrievedReceipt->SetNamedValue(L"signature", JsonValue::CreateStringValue(""));
							retrievedReceipt->SetNamedValue(L"sku", JsonValue::CreateStringValue(storeId));
							std::wstring tempString(retrievedReceipt->Stringify()->Data());
							String^ retrievedReceiptString = ref new String(base64Encode(tempString).c_str());
							callback(true, L"", retrievedReceiptString);
						}
						else {
							fulfill(targetPurchase->Value->StoreId).then([callback](bool success) {
								callback(false, L"NO_MATCHING_UNCLAIMED_PURCHASE", nullptr);
							}, task_continuation_context::use_current());
						}
					} catch (Platform::Exception^ exception) {
						fulfill(targetPurchase->Value->StoreId).then([callback](bool success) {
							callback(false, L"NO_MATCHING_UNCLAIMED_PURCHASE", nullptr);
						}, task_continuation_context::use_current());
					}
				});
			} catch (Platform::Exception^ exception) {
				fulfill(targetPurchase->Value->StoreId).then([callback](bool success) {
					callback(false, L"NO_MATCHING_UNCLAIMED_PURCHASE", nullptr);
				}, task_continuation_context::use_current());
				return;
			}
		}
		else {
			callback(true, L"NO_UNCLAIMED_PURCHASE", nullptr);
		}		
	}, task_continuation_context::use_current());
}

void PurchaseManager::purchaseProduct(String^ offerStoreID, int productIdNumber, std::function<void(bool, String^)> callback) {
	IVector<String^>^ storeIds = ref new Platform::Collections::Vector<String^>();
	storeIds->Append(offerStoreID);

	IVector<String^>^ productKinds = ref new Platform::Collections::Vector<String^>();
	// productKinds->Append("Durable");
	// productKinds->Append("Consumable");
	productKinds->Append("UnmanagedConsumable");
	IAsyncOperation<StoreProductQueryResult^>^ getUserCollectionAsync;
	
	try
	{
		getUserCollectionAsync = storeContext->GetStoreProductsAsync(productKinds, storeIds);
	}
	catch (Platform::Exception ^ exception)
	{
		callback(false, exception->Message);
	}

	create_task(getUserCollectionAsync).then([this, offerStoreID, productIdNumber, callback](StoreProductQueryResult^ queryResult)
	{
		String^ storeID = nullptr;
		String^ currencyCode = nullptr;
		String^ price = nullptr;

		for (auto itr = begin(queryResult->Products); itr != end(queryResult->Products); ++itr)
	 	{
	 		IKeyValuePair<String^, StoreProduct^>^ currentItem = dynamic_cast<IKeyValuePair<String^, StoreProduct^>^>(static_cast<Object^>(*itr));
	 		if (currentItem->Value->StoreId == offerStoreID)
	 		{
				storeID = offerStoreID;
				currencyCode = currentItem->Value->Price->CurrencyCode;
				price = currentItem->Value->Price->FormattedPrice;

	 			break;
	 		}
		}

		if (storeID == nullptr)
			callback(false, "Store ID is Null (begin)");
		else
			purchaseProductWithStoreID(storeID, currencyCode, price, productIdNumber, callback);
	}, task_continuation_context::use_current());
}

void PurchaseManager::purchaseProductWithStoreID(String^ storeId, String^ currencyCode, String^ price, int productIdNumber, std::function<void(bool, String^)> callback) {
	if (storeId == nullptr)
	{
		callback(false, "Store ID is Null");
		return;
	}

	if(microsoftStoreIdKey == nullptr)
	{
		callback(false, "Store ID Key is Null : " + errorReason);
		return;
	}

	IAsyncOperation<StorePurchaseResult^>^ requestProductPurchaseAsync;
	try {
		requestProductPurchaseAsync = storeContext->RequestPurchaseAsync(storeId);
	} catch (Platform::Exception^ exception) {
		callback(false, exception->Message);
		return;
	}
	create_task(requestProductPurchaseAsync).then([storeId, currencyCode, price, productIdNumber, callback, this](StorePurchaseResult^ result) {
		switch (result->Status) {
			case StorePurchaseStatus::AlreadyPurchased:
			case StorePurchaseStatus::Succeeded:
				callback(true, storeId + "|" + microsoftStoreIdKey + "|" + currencyCode + "|" + price);
				return;
			case StorePurchaseStatus::NotPurchased:
				callback(false, "Request product (NotPurchased)");
				return;
			case StorePurchaseStatus::NetworkError:
				callback(false, "Request product (NetworkError)");
				return;
			case StorePurchaseStatus::ServerError:
				callback(false, "Request product (ServerError)");
				return;
			default:
				callback(false, "Request product (INTERNAL_ERROR)");
				return;
		}
	}, task_continuation_context::use_current());

	try {
		StorageFolder^ storageFolder = ApplicationData::Current->LocalCacheFolder;
		IAsyncOperation<StorageFile^>^ createFileAsync = storageFolder->CreateFileAsync(purchaseIdHistoryFileName, CreationCollisionOption::OpenIfExists);
		create_task(createFileAsync).then([storeId, productIdNumber](StorageFile^ historyFile) {
			return FileIO::AppendTextAsync(historyFile, storeId + "," + productIdNumber + "\n");
		});
	} catch (Platform::Exception^ exception) {
		// eat error
	}
}

task<bool> PurchaseManager::fulfill(String^ storeId) {
	if (storeId == nullptr) {
		return create_task([]() { return false; });
	}
	UUID trackingId;
	UuidCreate(&trackingId);
	IAsyncOperation<StoreConsumableResult^>^ reportConsumableFulfillmentAsync;
	try {
		reportConsumableFulfillmentAsync = storeContext->ReportConsumableFulfillmentAsync(storeId, 1, trackingId);
	} catch (Platform::Exception^ exception) {
		return create_task([]() { return false; });
	}
	return create_task(reportConsumableFulfillmentAsync).then([](StoreConsumableResult^ result) {
		switch (result->Status) {
			case StoreConsumableStatus::Succeeded:
				return true;

			case StoreConsumableStatus::InsufficentQuantity:
			case StoreConsumableStatus::NetworkError:
			case StoreConsumableStatus::ServerError:
			default:
				return false;
		}
	}, task_continuation_context::use_current());
}
