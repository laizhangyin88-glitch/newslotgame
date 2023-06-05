#include "pch.h"

#define GENERATED_PROJECT 1
#include "..\Il2CppOutputProject\Source\CppPlugins\NativeHelperWindows.h"

#include "NativeHelper.h"
#include "lib\utils.h"
#include "push\PushNotification.h";
#include "NativeHelper\PurchaseManagerBridge.h"
#include "NativeHelper\SocialManagerBridge.h"
#include "NativeHelper\VideoAdsControllerBridge.h"
#include "lib\base64.h"
#include "lib\imageUtils.h"

using namespace Platform;
using namespace Windows::Storage;
using namespace Windows::Storage::Pickers;
using namespace concurrency;
using namespace std;
using namespace Windows::ApplicationModel::Resources;
//using namespace Windows::UI::Shell;
//using namespace Windows::Foundation::Metadata;

String^ g_uuid = nullptr;
String^ serverBaseUrl = nullptr;
String^ chattingUrl = nullptr;
String^ appDownloadUrl = nullptr;
String^ nativeHelperUnityObject = nullptr;
String^ adjustEnv = nullptr;
String^ adjustAppToken = nullptr;
String^ adjustAppSecret = nullptr;
String^ adjustInfo1 = nullptr;
String^ adjustInfo2 = nullptr;
String^ adjustInfo3 = nullptr;
String^ adjustInfo4 = nullptr;

// refer to	http://www.wadewegner.com/2012/09/getting-the-application-id-and-hardware-id-in-windows-store-applications/
const wchar_t* _stdcall getDeviceId() {
	if (g_uuid == nullptr) {
		auto easClientDeviceInformation = ref new Windows::Security::ExchangeActiveSyncProvisioning::EasClientDeviceInformation();
		Guid id = easClientDeviceInformation->Id;
		String^ idAsString = id.ToString();

		// idAsString is {XXX...} form: substring from 1 to end - 1, but no common language...
		wstring intermediateForm = wstring(idAsString->Data());
		wstring substring = intermediateForm.substr(1, ((int)idAsString->Length() - 2));
		wstring::size_type size = substring.size();
		g_uuid = ref new String(substring.c_str());
	}
	return toUnityString(g_uuid->Data());
}

const wchar_t* _stdcall getServerBaseUrl() {
	if (serverBaseUrl == nullptr) {
		ResourceLoader^ rl = ResourceLoader::GetForViewIndependentUse();
		serverBaseUrl = rl->GetString("ServerBaseURL");
	}
	return toUnityString(serverBaseUrl->Data());
}

const wchar_t* _stdcall getChattingUrl() {
	if (chattingUrl == nullptr) {
		ResourceLoader^ rl = ResourceLoader::GetForViewIndependentUse();
		chattingUrl = rl->GetString("ChattingURL");
	}
	return toUnityString(chattingUrl->Data());
}

const wchar_t* _stdcall getAdjustEnv() {
	if (adjustEnv == nullptr) {
		ResourceLoader^ rl = ResourceLoader::GetForViewIndependentUse();
		adjustEnv = rl->GetString("adjust_env");
	}
	return toUnityString(adjustEnv->Data());
}

const wchar_t* _stdcall getAdjustAppToken() {
	if (adjustAppToken == nullptr) {
		ResourceLoader^ rl = ResourceLoader::GetForViewIndependentUse();
		adjustAppToken = rl->GetString("adjust_app_token");
	}
	return toUnityString(adjustAppToken->Data());
}

const wchar_t* _stdcall getAdjustAppSecret() {
	if (adjustAppSecret == nullptr) {
		ResourceLoader^ rl = ResourceLoader::GetForViewIndependentUse();
		adjustAppSecret = rl->GetString("adjust_app_secret");
	}
	return toUnityString(adjustAppSecret->Data());
}

const wchar_t* _stdcall getAdjustInfo1() {
	if (adjustInfo1 == nullptr) {
		ResourceLoader^ rl = ResourceLoader::GetForViewIndependentUse();
		adjustInfo1 = rl->GetString("adjust_info1");
	}
	return toUnityString(adjustInfo1->Data());
}

const wchar_t* _stdcall getAdjustInfo2() {
	if (adjustInfo2 == nullptr) {
		ResourceLoader^ rl = ResourceLoader::GetForViewIndependentUse();
		adjustInfo2 = rl->GetString("adjust_info2");
	}
	return toUnityString(adjustInfo2->Data());
}
const wchar_t* _stdcall getAdjustInfo3() {
	if (adjustInfo3 == nullptr) {
		ResourceLoader^ rl = ResourceLoader::GetForViewIndependentUse();
		adjustInfo3 = rl->GetString("adjust_info3");
	}
	return toUnityString(adjustInfo3->Data());
}
const wchar_t* _stdcall getAdjustInfo4() {
	if (adjustInfo4 == nullptr) {
		ResourceLoader^ rl = ResourceLoader::GetForViewIndependentUse();
		adjustInfo4 = rl->GetString("adjust_info4");
	}
	return toUnityString(adjustInfo4->Data());
}

const wchar_t* _stdcall getAppDownloadUrl() {
	if (appDownloadUrl == nullptr) {
		ResourceLoader^ rl = ResourceLoader::GetForViewIndependentUse();
		appDownloadUrl = rl->GetString("AppDownloadURL");
	}
	return toUnityString(appDownloadUrl->Data());
}

void _stdcall initialize(const wchar_t* _nativeHelperUnityObject) {
	nativeHelperUnityObject = ref new String(_nativeHelperUnityObject);
	// clear badge & notification
	clearPush();
}

void _stdcall getProfileImage(unsigned int minSide, const wchar_t *_callback) {
	String^ callback = ref new String(_callback);
	RunOnWindowsUIThread([minSide, callback]() {
		FileOpenPicker^ openPicker = ref new FileOpenPicker();
		openPicker->ViewMode = PickerViewMode::Thumbnail;
		openPicker->SuggestedStartLocation = PickerLocationId::PicturesLibrary;
		openPicker->FileTypeFilter->Append(".jpg");
		openPicker->FileTypeFilter->Append(".jpeg");
		openPicker->FileTypeFilter->Append(".png");
		openPicker->FileTypeFilter->Append(".bmp");
		openPicker->FileTypeFilter->Append(".gif");
		openPicker->FileTypeFilter->Append(".tif");

		create_task(openPicker->PickSingleFileAsync()).then([minSide, callback](StorageFile^ file) {
			if (file == nullptr) {
				RunOnUnityAppThread([callback]() {
					unitySendMessage(nativeHelperUnityObject->Data(), callback->Data(), L"");
				});
			}
			else {
				getThumbnailFromImage(file, minSide)
					.then([callback](Array<byte>^ buf) {
					RunOnUnityAppThread([callback, buf]() {
						auto buflen = Base64encode_len(buf->Length);
						wchar_t *base64encoded = new wchar_t[buflen];
						Base64encodePlatformArray(base64encoded, buf);
						unitySendMessage(nativeHelperUnityObject->Data(), callback->Data(), base64encoded);
						delete[] base64encoded;
					});
				});
			}
		});
	});
}

bool _stdcall isPinningAllowed() {
	if( Windows::Foundation::Metadata::ApiInformation::IsTypePresent("Windows.UI.Shell.TaskbarManager"))
	{
		// Taskbar APIs exist!
		return Windows::UI::Shell::TaskbarManager::GetDefault()->IsPinningAllowed;
	}

	return false;
}

void _stdcall isPinned(const wchar_t* _callback) {
	String^ callback = ref new String(_callback);
	create_task(Windows::UI::Shell::TaskbarManager::GetDefault()->IsCurrentAppPinnedAsync()).then([callback](bool isPinned) {
		RunOnUnityAppThread([isPinned, callback]() {
			unitySendMessage(nativeHelperUnityObject->Data(), callback->Data(), isPinned ? L"True" : L"False");
		});
	});
}

void _stdcall setPin(const wchar_t* _callback) {
	String^ callback = ref new String(_callback);
	RunOnWindowsUIThread([callback]() {
		create_task(Windows::UI::Shell::TaskbarManager::GetDefault()->RequestPinCurrentAppAsync()).then([callback](bool isPinned) {
			RunOnUnityAppThread([isPinned, callback]() {
				unitySendMessage(nativeHelperUnityObject->Data(), callback->Data(), isPinned ? L"True" : L"False");
			});
		});
	});
}

void initNativeHelper() {
	initializeFn = initialize;
	registerForNotificationFn = registerForNotification;
	getNotificationTokenFn = getNotificationToken;
	getDeviceIdFn = getDeviceId;
	getServerBaseUrlFn = getServerBaseUrl;
	getChattingUrlFn = getChattingUrl;
	getAppDownloadUrlFn = getAppDownloadUrl;
	setLocalPushFn = setLocalPush;
	deleteLocalPushFn = deleteLocalPush;
	getAdjustEnvFn = getAdjustEnv;
	getAdjustAppTokenFn = getAdjustAppToken;
	getAdjustAppSecretFn = getAdjustAppSecret;
	getAdjustInfo1Fn = getAdjustInfo1;
	getAdjustInfo2Fn = getAdjustInfo2;
	getAdjustInfo3Fn = getAdjustInfo3;
	getAdjustInfo4Fn = getAdjustInfo4;
	/*extern PLUGIN_API action_t togglePushFn;
	extern PLUGIN_API action_str_str_t initAdjustFn;
	extern PLUGIN_API func_str_t getAdjustIDFn;
	extern PLUGIN_API action_str_t sendAdjustEventFn;
	extern PLUGIN_API action_str_double_str_t sendAdjustRevenueEventFn;*/
	getProfileImageFn = getProfileImage;
	isPinningAllowedFn = isPinningAllowed;
	isPinnedFn = isPinned;
	setPinFn = setPin;
}

void initUnityNativePlugins() {
	initNativeHelper();
	initPushNotification();
	initPurchaseManager();
	initSocialManager();
	initVideoAdsController();
}
