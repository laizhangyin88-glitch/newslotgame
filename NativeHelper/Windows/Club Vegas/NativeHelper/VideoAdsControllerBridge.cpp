#include "pch.h"
#include "lib\utils.h"
#include "NativeHelper/VideoAdsControllerBridge.h"
#include "VideoAds/VungleManager.h"
#include <concurrent_vector.h>

#define GENERATED_PROJECT 1
#include "..\Il2CppOutputProject\Source\CppPlugins\VideoAdsControllerWindows.h"
#include "..\Il2CppOutputProject\Source\CppPlugins\NativeHelperWindows.h"

using namespace Platform;
using namespace concurrency;

String^ videoAdsControllerUnityObject = nullptr;

void _stdcall initializeVideoAdsController(const wchar_t* _videoAdsControllerUnityObject) {
	videoAdsControllerUnityObject = ref new String(_videoAdsControllerUnityObject);
	RunOnWindowsUIThread([]() {
		VideoAds::VungleManager::getInstance()->Init();
	});
}

void _stdcall loadPlacement(const wchar_t* placement) {
	String^ placementStr = ref new String(placement);
	VideoAds::VungleManager::getInstance()->LoadPlacement(placementStr);
}

bool _stdcall isVideoAdsAvailable(const wchar_t* placement) {
	String^ placementStr = ref new String(placement);
	return VideoAds::VungleManager::getInstance()->IsVideoAdsAvailable(placementStr);
}

void _stdcall showRewardedVideo(const wchar_t* placement, const wchar_t* _methodName) {
	String^ methodName = ref new String(_methodName);
	String^ placementStr = ref new String(placement);
	VideoAds::VungleManager::getInstance()->PlayPlacement(placementStr, videoAdsControllerUnityObject, methodName);
}

long _stdcall getPlacementReward(const wchar_t* placement) {
    return 0;
}

void initVideoAdsController() {
	initializeVideoAdsControllerFn = initializeVideoAdsController;
	loadPlacementFn = loadPlacement;
	isVideoAdsAvailableFn = isVideoAdsAvailable;
	showRewardedVideoFn = showRewardedVideo;
	getPlacementRewardFn = getPlacementReward;
}

