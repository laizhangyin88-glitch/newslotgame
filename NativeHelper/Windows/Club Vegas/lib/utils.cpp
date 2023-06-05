#include "pch.h"
#include "utils.h"
#include <cvt/wstring>
#include <codecvt>

using namespace Platform;
using namespace std;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;
using namespace Windows::System::Threading;

// Unity will release it.
const wchar_t* toUnityString(const wchar_t* srcData) {
	ULONG  ulSize = (wcsnlen_s(srcData, 100000) * sizeof(wchar_t)) + sizeof(wchar_t);
	wchar_t* pwszReturn = (wchar_t*)::CoTaskMemAlloc(ulSize);
	// Copy the contents.
	wcscpy_s(pwszReturn, ulSize, srcData);
	return pwszReturn;
}

DateTime DateTimeOffset(int seconds) {
	auto cal = ref new Windows::Globalization::Calendar();
	cal->AddSeconds(seconds);
	return cal->GetDateTime();
}

void RunOnWindowsUIThread(const std::function<void()>& handler) {
	Windows::ApplicationModel::Core::CoreApplication::MainView->CoreWindow->Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler(handler));
}

void RunOnUnityAppThread(const std::function<void()>& handler) {
	UnityPlayer::AppCallbacks::Instance->InvokeOnAppThread(ref new UnityPlayer::AppCallbackItem(handler), false);
}
