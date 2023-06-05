#include "pch.h"
#include "lib\utils.h"
#include "SocialManagerBridge.h"
#include "facebook\FacebookManager.h"

#define GENERATED_PROJECT 1
#include "..\Il2CppOutputProject\Source\CppPlugins\SocialManagerWindows.h"
#include "..\Il2CppOutputProject\Source\CppPlugins\NativeHelperWindows.h"

using namespace Platform;

String^ socialManagerUnityObject = nullptr;

void _stdcall initializeSocialManager(const wchar_t* _purchaseManagerUnityObject) {
    socialManagerUnityObject = ref new String(_purchaseManagerUnityObject);
}

void _stdcall loginFacebook(const wchar_t* _callback) {
    String^ callback = ref new String(_callback);
    RunOnWindowsUIThread([callback] {
        UnityPlayer::AppCallbacks::Instance->UnitySetInput(false);
        getFacebookUserInfo().then([callback](String^ resultJson) {
            RunOnWindowsUIThread([] {
                UnityPlayer::AppCallbacks::Instance->UnitySetInput(true);
            });
            RunOnUnityAppThread([callback, resultJson]() {
                unitySendMessage(socialManagerUnityObject->Data(), callback->Data(), resultJson->Data());
            });
        });
    });
}

void _stdcall _shareFacebook(const wchar_t* _linkUrl, const wchar_t* _title, const wchar_t* _description, const wchar_t* _imageUrl, const wchar_t* _callback) {
    String^ linkUrl = ref new String(_linkUrl);
    String^ title = ref new String(_title);
    String^ description = ref new String(_description);
    String^ imageUrl = ref new String(_imageUrl);
    String^ callback = ref new String(_callback);
    RunOnWindowsUIThread([linkUrl, title, description, imageUrl, callback] {
        UnityPlayer::AppCallbacks::Instance->UnitySetInput(false);
        shareFacebook(linkUrl, title, description, imageUrl).then([callback](String^ result) {
            RunOnWindowsUIThread([] {
                UnityPlayer::AppCallbacks::Instance->UnitySetInput(true);
            });
            RunOnUnityAppThread([callback, result]() {
                unitySendMessage(socialManagerUnityObject->Data(), callback->Data(), result->Data());
            });
        });
    });
}

//void _stdcall _inviteFacebook(const wchar_t* _linkUrl, const wchar_t* _imageUrl, const wchar_t* _callback) {
//    String^ message = ref new String(L"Play the VEGAS style Slots LIVE with FRIENDS!");
//    String^ callback = ref new String(_callback);
//    RunOnWindowsUIThread([callback, message] {
//        UnityPlayer::AppCallbacks::Instance->UnitySetInput(false);
//        inviteFacebookFriend(message).then([callback](String^ result) {
//            RunOnWindowsUIThread([] {
//                UnityPlayer::AppCallbacks::Instance->UnitySetInput(true);
//            });
//            RunOnUnityAppThread([callback, result]() {
//                unitySendMessage(socialManagerUnityObject->Data(), callback->Data(), result->Data());
//            });
//        });
//    });
//}

void _stdcall _logoutFacebook() {
    logoutFacebook();
}

const wchar_t* _stdcall _getFacebookId() {
    return getFacebookId();
}

const wchar_t* _stdcall _getFacebookAccessToken() {
    return getFacebookAccessToken();
}

const wchar_t* _stdcall _getFacebookAppId() {
    return getFacebookAppId();
}

const wchar_t* _stdcall _getFacebookAppIdName() {
    return getFacebookAppIdName();
}

void initSocialManager() {
    initializeSocialFn = initializeSocialManager;

    loginFBFn = loginFacebook;
    shareFBFn = _shareFacebook;
    //inviteFBFn = _inviteFacebook;
    logoutFBFn = _logoutFacebook;
    getFacebookIdFn = _getFacebookId;
    getFacebookAccessTokenFn = _getFacebookAccessToken;

    getFacebookAppIdFn = _getFacebookAppId;
    getFacebookAppIdNameFn = _getFacebookAppIdName;
    
    initFacebook();
}
