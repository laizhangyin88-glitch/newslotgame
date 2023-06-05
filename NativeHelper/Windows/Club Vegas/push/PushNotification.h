#pragma once

void initPushNotification();
void _stdcall registerForNotification();
const wchar_t* _stdcall getNotificationToken();
void _stdcall clearPush();
void _stdcall setLocalPush(int pushId, const wchar_t* title, const wchar_t *body, int delayInSec, bool doNotDisturb);
void _stdcall deleteLocalPush(int pushId);
