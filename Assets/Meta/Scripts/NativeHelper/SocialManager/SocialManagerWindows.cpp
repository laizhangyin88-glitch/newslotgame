#include "SocialManagerWindows.h"

PLUGIN_API action_str_t initializeSocialFn = nullptr;
PLUGIN_API action_str_t loginFBFn = nullptr;
PLUGIN_API action_str_str_str_str_str_t shareFBFn = nullptr;
PLUGIN_API action_str_str_str_t inviteFBFn = nullptr;
PLUGIN_API action_t logoutFBFn = nullptr;
PLUGIN_API func_str_t getFacebookAppIdFn = nullptr;
PLUGIN_API func_str_t getFacebookAppIdNameFn = nullptr;
PLUGIN_API func_str_t getFacebookIdFn = nullptr;
PLUGIN_API func_str_t getFacebookAccessTokenFn = nullptr;

extern "C" {
	void _stdcall initializeSocial(const wchar_t* name) {
		if (initializeSocialFn != nullptr) {
			initializeSocialFn(name);
		}
	}

	void _stdcall loginFB(const wchar_t* callback) {
		if (loginFBFn != nullptr) {
			loginFBFn(callback);
		}
	}

	void _stdcall shareFB(const wchar_t* linkUrl, const wchar_t* title, const wchar_t* description, const wchar_t* imageUrl, const wchar_t* callback) {
		if (shareFBFn != nullptr) {
			shareFBFn(linkUrl, title, description, imageUrl, callback);
		}
	}

	void _stdcall inviteFB(const wchar_t* linkUrl, const wchar_t* imageUrl, const wchar_t* callback) {
		if (inviteFBFn != nullptr) {
			inviteFBFn(linkUrl, imageUrl, callback);
		}
	}

	void _stdcall logoutFB() {
		if (logoutFBFn != nullptr) {
			logoutFBFn();
		}
	}

	const wchar_t* _stdcall getFacebookAppId() {
		if (getFacebookAppIdFn != nullptr) {
			return getFacebookAppIdFn();
		}

		return nullptr;
	}

	const wchar_t* _stdcall getFacebookAppIdName() {
		if (getFacebookAppIdNameFn != nullptr) {
			return getFacebookAppIdNameFn();
		}

		return nullptr;
	}

	const wchar_t* _stdcall getFacebookId() {
		if (getFacebookIdFn != nullptr) {
			return getFacebookIdFn();
		}

		return nullptr;
	}

	const wchar_t* _stdcall getFacebookAccessToken() {
		if (getFacebookAccessTokenFn != nullptr) {
			return getFacebookAccessTokenFn();
		}

		return nullptr;
	}
}
