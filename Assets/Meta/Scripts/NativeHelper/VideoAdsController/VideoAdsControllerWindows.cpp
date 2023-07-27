#include "VideoAdsControllerWindows.h"

PLUGIN_API action_str_t initializeVideoAdsControllerFn;
PLUGIN_API action_str_t loadPlacementFn;
PLUGIN_API func_bool_action_str_t isVideoAdsAvailableFn;
PLUGIN_API action_str_str_t showRewardedVideoFn;
PLUGIN_API func_long_action_str_t getPlacementRewardFn;

extern "C" {
	void _stdcall initializeVideoAdsController(const wchar_t *name) {
		if (initializeVideoAdsControllerFn != nullptr) {
			initializeVideoAdsControllerFn(name);
		}
	}

	void _stdcall loadPlacement(const wchar_t *placement) {
		if (loadPlacementFn != nullptr) {
			loadPlacementFn(placement);
		}
	}

	bool _stdcall isVideoAdsAvailable(const wchar_t *placement) {
		if (isVideoAdsAvailableFn != nullptr) {
			return isVideoAdsAvailableFn(placement);
		}
        return false;
	}

	void _stdcall showRewardedVideo(const wchar_t *placement, const wchar_t *methodName) {
		if (showRewardedVideoFn != nullptr) {
			showRewardedVideoFn(placement, methodName);
		}
	}

	long _stdcall getPlacementReward(const wchar_t *placement) {
		if (getPlacementRewardFn != nullptr) {
			return getPlacementRewardFn(placement);
		}
		return 0;
	}

}
