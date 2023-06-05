#pragma once

const wchar_t* toUnityString(const wchar_t* srcData);
Windows::Foundation::DateTime DateTimeOffset(int seconds);
void RunOnWindowsUIThread(const std::function<void()>& handler);
void RunOnUnityAppThread(const std::function<void()>& handler);
