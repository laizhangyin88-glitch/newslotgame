#include "pch.h"
#include "FacebookManager.h"
#include "lib/utils.h"

#define FBAppIDName L"FBApplicationId"
#define FBStoreAppIDName L"FBWindowsAppId"

using namespace Windows::ApplicationModel::Resources; 
using namespace Windows::Foundation::Collections;
using namespace Platform;
using namespace Platform::Collections;
using namespace concurrency;
using namespace winsdkfb;
using namespace winsdkfb::Graph;
using namespace Windows::Data::Json;

// Refer to http://microsoft.github.io/winsdkfb/

String^ SUCCESS_RESPONSE = ref new String(L"Success");
String^ ERROR_RESPONSE = ref new String(L"Error");

String^ FB_APP_ID;
String^ WIN_APP_ID;

task<String^> returnErrorResponse() {
    return task<String^>([]() -> String^ { return ERROR_RESPONSE; });
}


FBJsonClassFactory^ fact = ref new FBJsonClassFactory([](String^ JsonText) -> Object^ {
    return JsonObject::Parse(JsonText);
});

task<IJsonValue^> friendIdListRequest() {
    FBSingleValue^ sval = ref new FBSingleValue(L"/me/friends", nullptr, fact);
    return create_task(sval->GetAsync()).then([](FBResult^ result) -> IJsonValue^ {
        if (result->Succeeded) {
            JsonObject^ responseJson = static_cast<JsonObject^>(result->Object);
            JsonArray^ friendList = responseJson->GetNamedArray(L"data");
            JsonArray^ friendIdList = ref new JsonArray();
            for (int i = 0; i < friendList->Size; ++i) {
                friendIdList->Append(JsonValue::CreateStringValue(friendList->GetObjectAt(i)->GetNamedString("id")));
            }
            return friendIdList;
        } else {
            return nullptr;
        }
    });
}

task<bool> loginFacebook() {
    FBSession^ sess = FBSession::ActiveSession;
    sess->FBAppId = FB_APP_ID;
    sess->WinAppId = WIN_APP_ID;
    if (sess->LoggedIn)	{
        return task<bool>([]() { return true; });
    }

    // Add permissions required by the app
    Vector<String^>^ permissionList = ref new Vector<String^>();
    permissionList->Append(L"public_profile");
    // permissionList->Append(L"user_friends");
    FBPermissions^ permissions = ref new FBPermissions(permissionList->GetView());

    // Unsupported browser error when SessionLoginBehavior::WebView is inserted.
    // return create_task(sess->LoginAsync(permissions, SessionLoginBehavior::WebAuth)).then([](FBResult^ result)
    return create_task(sess->LoginAsync(permissions)).then([](FBResult^ result)
    {
        return result->Succeeded;
    });
}

task<String^> doAfterLogin(std::function<task<String^>()> method) {
    return loginFacebook().then([method](bool succeeded) {
        if (!succeeded) {
            return returnErrorResponse();
        }
        return method();
    });
}

task<IJsonValue^> userPictureRequest() {
    PropertySet^ parameters = ref new PropertySet();
    parameters->Insert(L"fields", L"picture.width(152).height(152)");
    
    FBSingleValue^ sval = ref new FBSingleValue(L"/me", parameters, fact);
    return create_task(sval->GetAsync())
        .then([](FBResult^ result) -> IJsonValue^
    {
        if (result->Succeeded) {
            try {
                JsonObject^ responseJson = static_cast<JsonObject^>(result->Object);
                JsonObject^ pictureJson = responseJson->GetNamedObject(L"picture")->GetNamedObject(L"data");
                if (pictureJson->GetNamedBoolean(L"is_silhouette")) {
                    return nullptr;
                }
                return pictureJson->GetNamedValue(L"url");
            } catch (Exception^ ex) {
                return nullptr;
            }
        } else {
            return nullptr;
        }
    });
}

task<String^> getFacebookUserInfoAux() {
    auto tasks = friendIdListRequest() && userPictureRequest();
    return tasks.then([](std::vector<IJsonValue^> results) -> String^ {
        IJsonValue^ friendIdList = results[0];
        IJsonValue^ picture = results[1];
        FBSession^ sess = FBSession::ActiveSession;
        FBUser^ user = sess->User;
        if (friendIdList == nullptr || user == nullptr) {
            return ERROR_RESPONSE;
        }
        JsonObject^ userInfo = ref new JsonObject();
        userInfo->SetNamedValue(L"id", JsonValue::CreateStringValue(user->Id));
        userInfo->SetNamedValue(L"name", JsonValue::CreateStringValue(user->Name));
        if (picture == nullptr) {
            picture = JsonValue::CreateStringValue(L"");
        }
        userInfo->SetNamedValue(L"picture", picture);
        userInfo->SetNamedValue(L"gender", JsonValue::CreateStringValue(user->Gender));
        userInfo->SetNamedValue(L"friendList", friendIdList);
        return userInfo->Stringify();
    });
}

//task<String^> inviteFacebookFriendAux(String^ mesasge) {
//    FBSession^ sess = FBSession::ActiveSession;
//    // Refer to https://developers.facebook.com/docs/games/services/gamerequests#launchingrequestdialog
//    PropertySet^ parameters = ref new PropertySet();
//    parameters->Insert(L"message", mesasge);
//    return create_task(sess->ShowRequestsDialogAsync(parameters)).then([=](FBResult^ result) {
//        return result->Succeeded ? SUCCESS_RESPONSE :ERROR_RESPONSE;
//    });
//}

task<String^> shareFacebookAux(String^ linkUrl, String^ title, String^ description, String^ imageUrl) {
    FBSession^ sess = FBSession::ActiveSession;
    // Refer to https://developers.facebook.com/docs/sharing/web
    PropertySet^ parameters = ref new PropertySet();
    //parameters->Insert(L"caption", title);
    parameters->Insert(L"link", linkUrl + L"?image=" + imageUrl);
    //parameters->Insert(L"link", linkUrl + L"?image=" + imageUrl + L"&title=" + title + L"&description=" + description);
    //parameters->Insert(L"description", description);
    
    return create_task(sess->ShowFeedDialogAsync(parameters)).then([=](FBResult^ result)
    {
        if (result->Succeeded)
        {
            return SUCCESS_RESPONSE;
        }
        else
        {
            return SUCCESS_RESPONSE;

            // Always return error Bug.. error code is 4201. OAuthException.
            //return ERROR_RESPONSE;
        }
    });
}


// // Public methods:

void initFacebook() {
    ResourceLoader^ rl = ResourceLoader::GetForViewIndependentUse();
    FB_APP_ID = rl->GetString(FBAppIDName);
    // May find it from: Windows::Security::Authentication::Web::WebAuthenticationBroker::GetCurrentApplicationCallbackUri()->DisplayUri;
    WIN_APP_ID = rl->GetString(FBStoreAppIDName);
    FBSession^ sess = FBSession::ActiveSession;
    sess->FBAppId = FB_APP_ID;
    sess->WinAppId = WIN_APP_ID;
    // Log the app activation
    FBSDKAppEvents::ActivateApp();
}

task<String^> getFacebookUserInfo() {
    return doAfterLogin(getFacebookUserInfoAux);
}

task<String^> shareFacebook(String^ linkUrl, String^ title, String^ description, String^ imageUrl) {
    return doAfterLogin([linkUrl, title, description, imageUrl]{ return shareFacebookAux(linkUrl, title, description, imageUrl); });
}

void logoutFacebook() {
    FBSession^ sess = FBSession::ActiveSession;
    sess->LogoutAsync();
}

const wchar_t* getFacebookAppId() {
    ResourceLoader^ rl = ResourceLoader::GetForViewIndependentUse();
    return toUnityString(rl->GetString(FBAppIDName)->Data());
}

const wchar_t* getFacebookAppIdName() {
    ResourceLoader^ rl = ResourceLoader::GetForViewIndependentUse();
    return toUnityString(rl->GetString(FBStoreAppIDName)->Data());
}

const wchar_t* getFacebookId() {
    FBSession^ sess = FBSession::ActiveSession;
    if (sess->LoggedIn) {
    return toUnityString(sess->User->Id->Data());
    }
}

const wchar_t* getFacebookAccessToken() {
    FBSession^ sess = FBSession::ActiveSession;
    if (sess->LoggedIn) {
        return toUnityString(sess->AccessTokenData->AccessToken->Data());
    }
}

