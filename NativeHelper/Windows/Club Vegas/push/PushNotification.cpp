#include "pch.h"
#include "PushNotification.h"
#include "TileHelper.h"
#include "ToastHelper.h"
#include "WnsWrapper.h"
#include "lib/utils.h"

using namespace PushNotification;
using namespace Platform;

void initPushNotification() {
	ToastHelper::Instance->init();
}

void _stdcall registerForNotification() {
	RunOnWindowsUIThread([] {
		WnsWrapper::Instance->ObtainUri();
	});
}

const wchar_t* _stdcall getNotificationToken() {
	auto token = WnsWrapper::Instance->Uri;
	if (token == nullptr) {
		return nullptr;
	}
	return toUnityString(token->Data());
}

void _stdcall clearPush() {
	RunOnWindowsUIThread([] {
		ToastHelper::Instance->clearNotifications();
		TileHelper::Instance->clearNotifications();
	});
}

void _stdcall setLocalPush(int pushId, const wchar_t* title, const wchar_t *body, int delayInSec, bool doNotDisturb) {
	String^ pushIdStr = pushId.ToString();
	String^ titleStr = ref new String(title);
	String^ bodyStr = ref new String(body);
	ToastHelper::Instance->createScheduledNotification(pushIdStr, titleStr, bodyStr, delayInSec);
	TileHelper::Instance->createScheduledNotification(pushIdStr, titleStr, bodyStr, delayInSec);
}

void _stdcall deleteLocalPush(int pushId) {
	RunOnWindowsUIThread([pushId] {
		String^ pushIdStr = pushId.ToString();
		ToastHelper::Instance->deleteScheduledNotification(pushIdStr);
		TileHelper::Instance->deleteScheduledNotification(pushIdStr);
	});
}
