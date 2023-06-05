#pragma once

void initFacebook();
concurrency::task<Platform::String^> getFacebookUserInfo();
//concurrency::task<Platform::String^> inviteFacebookFriend(Platform::String^ message);
concurrency::task<Platform::String^> shareFacebook(Platform::String^ linkUrl, Platform::String^ title, Platform::String^ description, Platform::String^ imageUrl);

concurrency::task<bool> loginFacebook();
void logoutFacebook();

const wchar_t* getFacebookAppId();
const wchar_t* getFacebookAppIdName();

const wchar_t* getFacebookId();
const wchar_t* getFacebookAccessToken();
