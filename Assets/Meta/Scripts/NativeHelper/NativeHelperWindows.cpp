#include "NativeHelperWindows.h"

PLUGIN_API action_str_t initializeFn = nullptr;
PLUGIN_API action_t registerForNotificationFn = nullptr;
PLUGIN_API func_str_t getNotificationTokenFn = nullptr;
PLUGIN_API func_str_t getDeviceIdFn = nullptr;
PLUGIN_API func_str_t getServerBaseUrlFn = nullptr;
PLUGIN_API func_str_t getChattingUrlFn = nullptr;
PLUGIN_API func_str_t getAppDownloadUrlFn = nullptr;
PLUGIN_API action_int_str_str_int_bool_t setLocalPushFn = nullptr;
PLUGIN_API action_int_t deleteLocalPushFn = nullptr;
PLUGIN_API action_t togglePushFn = nullptr;
PLUGIN_API func_str_t getAdjustEnvFn = nullptr;
PLUGIN_API func_str_t getAdjustAppTokenFn = nullptr;
PLUGIN_API func_str_t getAdjustAppSecretFn = nullptr;
PLUGIN_API func_str_t getAdjustInfo1Fn = nullptr;
PLUGIN_API func_str_t getAdjustInfo2Fn = nullptr;
PLUGIN_API func_str_t getAdjustInfo3Fn = nullptr;
PLUGIN_API func_str_t getAdjustInfo4Fn = nullptr;
PLUGIN_API action_uint_str_t getProfileImageFn = nullptr;
PLUGIN_API func_bool isPinningAllowedFn = nullptr;
PLUGIN_API action_str_t isPinnedFn = nullptr;
PLUGIN_API action_str_t setPinFn = nullptr;
action_str_str_str_t unitySendMessageFn = nullptr;

extern "C" {
	void _stdcall initialize(const wchar_t *name) {
		if (initializeFn != nullptr) {
			initializeFn(name);
		}
	}

	void _stdcall registerForNotification() {
		if (registerForNotificationFn != nullptr) {
			registerForNotificationFn();
		}
	}

	const wchar_t* _stdcall getNotificationToken() {
		if (getNotificationTokenFn != nullptr) {
			return getNotificationTokenFn();
		}
		return nullptr;
	}

	const wchar_t* _stdcall getDeviceId() {
		if (getDeviceIdFn != nullptr) {
			return getDeviceIdFn();
		}
		return nullptr;
	}

	const wchar_t* _stdcall getServerBaseUrl() {
		if (getServerBaseUrlFn != nullptr) {
			return getServerBaseUrlFn();
		}
		return nullptr;
	}

	const wchar_t* _stdcall getChattingUrl() {
		if (getChattingUrlFn != nullptr) {
			return getChattingUrlFn();
		}
		return nullptr;
	}

	const wchar_t* _stdcall getAppDownloadUrl() {
		if (getAppDownloadUrlFn != nullptr) {
			return getAppDownloadUrlFn();
		}
		return nullptr;
	}

	void _stdcall setLocalPush(int pushID, const wchar_t *sender, const wchar_t *message, int delay, bool doNotDisturb) {
		if (setLocalPushFn != nullptr) {
			setLocalPushFn(pushID, sender, message, delay, doNotDisturb);
		}
	}

	void _stdcall deleteLocalPush(int pushID) {
		if (deleteLocalPushFn != nullptr) {
			deleteLocalPushFn(pushID);
		}
	}

	void _stdcall togglePush() {
		if (togglePushFn != nullptr) {
			togglePushFn();
		}
	}

	const wchar_t* _stdcall getAdjustEnv() {
		if (getAdjustEnvFn != nullptr) {
			return getAdjustEnvFn();
		}
		return nullptr;
	}

	const wchar_t* _stdcall getAdjustAppToken() {
		if (getAdjustAppTokenFn != nullptr) {
			return getAdjustAppTokenFn();
		}
		return nullptr;
	}

	const wchar_t* _stdcall getAdjustAppSecret() {
		if (getAdjustAppSecretFn != nullptr) {
			return getAdjustAppSecretFn();
		}
		return nullptr;
	}

	const wchar_t* _stdcall getAdjustInfo1() {
		if (getAdjustInfo1Fn != nullptr) {
			return getAdjustInfo1Fn();
		}
		return nullptr;
	}

	const wchar_t* _stdcall getAdjustInfo2() {
		if (getAdjustInfo2Fn != nullptr) {
			return getAdjustInfo2Fn();
		}
		return nullptr;
	}

	const wchar_t* _stdcall getAdjustInfo3() {
		if (getAdjustInfo3Fn != nullptr) {
			return getAdjustInfo3Fn();
		}
		return nullptr;
	}

	const wchar_t* _stdcall getAdjustInfo4() {
		if (getAdjustInfo4Fn != nullptr) {
			return getAdjustInfo4Fn();
		}
		return nullptr;
	}

	void _stdcall getProfileImage(unsigned int minSide, const wchar_t *callback) {
		if (getProfileImageFn != nullptr) {
			getProfileImageFn(minSide, callback);
		}
	}

	bool _stdcall isPinningAllowed() {
		if (isPinningAllowedFn != nullptr) {
			return isPinningAllowedFn();
		}
		return false;
	}

	void _stdcall isPinned(const wchar_t* callback) {
		if (isPinnedFn != nullptr) {
			isPinnedFn(callback);
		}
	}

	void _stdcall setPin(const wchar_t* callback) {
		if (setPinFn != nullptr) {
			setPinFn(callback);
		}
	}

	void _stdcall setUnitySendMessage(action_str_str_str_t _unitySendMessageFn) {
		unitySendMessageFn = _unitySendMessageFn;
	}

	PLUGIN_API void _stdcall unitySendMessage(const wchar_t *object, const wchar_t *method, const wchar_t *arg) {
		if (unitySendMessageFn != nullptr) {
			unitySendMessageFn(object, method, arg);
		}
	}
}
