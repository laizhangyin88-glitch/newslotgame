
#include "pch.h"
#include "VungleManager.h"
#include "sstream"
#include "iostream"

#include "lib\utils.h"
#include "..\Il2CppOutputProject\Source\CppPlugins\NativeHelperWindows.h"

using namespace VideoAds;

using namespace Windows::ApplicationModel::Resources;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::ApplicationModel::Core;

//Using VungleSDK namespace
using namespace VungleSDK;

Platform::String^ unityObjectForOnAdEnd = nullptr;
Platform::String^ unityMethodNameForOnAdEnd = nullptr;
VungleManager^ VungleManager::instance = nullptr;

VungleManager::VungleManager()
{
}

VungleManager^ VungleManager::getInstance() {
	if (VungleManager::instance == nullptr) {
		VungleManager::instance = ref new VungleManager();
	}
	return VungleManager::instance;
}


//Event handler for OnInitComleted event
void VideoAds::VungleManager::OnInitCompleted(Platform::Object^ sender, VungleSDK::ConfigEventArgs^ args)
{
	/* logging
	//Run asynchronously on the UI thread
	CoreApplication::MainView->Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal,
		ref new Windows::UI::Core::DispatchedHandler(
			[this, args]
	{
		size_t converted;
		std::stringstream placementsInfo;
		placementsInfo << std::endl << "OnInitCompleted: ";
		const wchar_t* wcInitialized = args->Initialized.ToString()->Data();
		char cInitialized[512];
		wcstombs_s(&converted, cInitialized, 512, wcInitialized, 512);
		placementsInfo << cInitialized;

		if (args->Initialized == true)
		{
			for (int i = 0; i < args->Placements->Length; i++)
			{
				placementsInfo << "\n\tPlacement" << (i + 1) << ": ";
				const wchar_t* wcPlacement = args->Placements[i]->ReferenceId->Data();
				char cPlacement[512];
				wcstombs_s(&converted, cPlacement, 512, wcPlacement, 512);
				placementsInfo << cPlacement;

				if (args->Placements[i]->IsAutoCached == true) {
					placementsInfo << " (Auto-Cached)";
				}
			}
		}
		else
		{
			placementsInfo << "\n\t";
			const wchar_t* wcEmessage = args->ErrorMessage->Data();
			char cEmessage[512];
			wcstombs_s(&converted, cEmessage, 512, wcEmessage, 512);
			placementsInfo << cEmessage;
		}
		placementsInfo << std::endl;

		OutputDebugStringA(placementsInfo.str().c_str());


	}));
	*/

	for (auto iter = placementMap.begin(); iter != placementMap.end(); ++iter)
	{
		this->LoadPlacement(iter->first);
	}
}

// Event handler called when e->AdPlayable is changed
void VideoAds::VungleManager::OnOnAdPlayableChanged(Platform::Object^ sender, VungleSDK::AdPlayableEventArgs^ args)
{
	// args->AdPlayable - true if an ad is available to play, false otherwise
	// args->Placement  - placement ID in string

	// placementMap.insert(std::make_pair(args->Placement, args->AdPlayable));
	placementMap[args->Placement] = args->AdPlayable;

	/* logging
	bool adPlayable = args->AdPlayable;
	const wchar_t* wide_chars = args->Placement->Data();
	char chars[512];
	size_t converted;
	wcstombs_s(&converted, chars, 512, wide_chars, 512);
	std::string placement = chars;
	std::stringstream dmess;
	dmess << std::endl << "OnAdPlayable: " << placement << " - " << adPlayable << std::endl;
	OutputDebugStringA(dmess.str().c_str());
	*/
}

// Event Handler called before playing an ad
void VideoAds::VungleManager::OnAdStart(Platform::Object^ sender, VungleSDK::AdEventArgs^ e)
{
	// e.Id        - Vungle app ID in string
	// e.Placement - placement ID in string

	/* logging
	std::stringstream dmess;
	size_t converted;
	const wchar_t* wcId = e->Id->Data();
	char cId[512];
	wcstombs_s(&converted, cId, 512, wcId, 512);
	const wchar_t* wcPlacement = e->Placement->Data();
	char cPlacement[512];
	wcstombs_s(&converted, cPlacement, 512, wcPlacement, 512);
	dmess << std::endl << "OnAdStart(" << cId << "): " << cPlacement << std::endl;
	OutputDebugStringA(dmess.str().c_str());
	*/
}

// Event handler called when the user leaves ad and control is return to the hosting app
void VideoAds::VungleManager::OnAdEnd(Platform::Object^ sender, VungleSDK::AdEndEventArgs^ e)
{
	// e->Id                  - Vungle app ID in string
	// e->Placement           - placement ID in string
	// e->IsCompletedView     - true when 80% or more of the video was watched
	// e->CallToActionClicked - true when the user has clicked download button on end card
	// e->WatchedDuration     - duration of video watched
	// e->VideoDuration       - DEPRECATED

	RunOnUnityAppThread([]() {
		unitySendMessage(unityObjectForOnAdEnd->Data(), unityMethodNameForOnAdEnd->Data(), L"");
	});

	/* logging
	std::stringstream dmess;
	size_t converted;
	const wchar_t* wcId = e->Id->Data();
	char cId[512];
	wcstombs_s(&converted, cId, 512, wcId, 512);

	const wchar_t* wcPlacement = e->Placement->Data();
	char cPlacement[512];
	wcstombs_s(&converted, cPlacement, 512, wcPlacement, 512);

	dmess << std::endl << "OnVideoEnd(" << cId << "): " << "\n\tPlacement: " << cPlacement << "\n\tIsCompletedView: " << e->IsCompletedView;
	dmess << "\n\tCallToActionClicked: " << e->CallToActionClicked << "\n\tWatchedDuration: " << e->WatchedDuration.Duration << std::endl;

	OutputDebugStringA(dmess.str().c_str());
	*/
}

// Event handler called when SDK wants to print diagnostic logs
void VideoAds::VungleManager::Diagnostic(Platform::Object^ sender, VungleSDK::DiagnosticLogEvent^ e)
{
	// std::stringstream dmess;
	// size_t converted;
	// const wchar_t* wcLevel = e->Level.ToString()->Data();
	// char cLevel[512];
	// wcstombs_s(&converted, cLevel, 512, wcLevel, 512);

	// const wchar_t* wcType = e->Type.Name->Data();
	// char cType[512];
	// wcstombs_s(&converted, cType, 512, wcType, 512);

	// const wchar_t* wcException = e->Exception.ToString()->Data();
	// char cException[512];
	// wcstombs_s(&converted, cException, 512, wcException, 512);

	// const wchar_t* wcMessage = e->Message->Data();
	// char cMessage[512];
	// wcstombs_s(&converted, cMessage, 512, wcMessage, 512);

	// dmess << std::endl << "Diagnostics: " << cLevel << " " << cType << " " << cException << " " << cMessage;

	// OutputDebugStringA(dmess.str().c_str());
}

void VideoAds::VungleManager::Init()
{
	if (sdkInstance != nullptr) return;
	
	ResourceLoader^ rl = ResourceLoader::GetForViewIndependentUse();
	Platform::String ^vungleAppId = rl->GetString("vungle_app_id");

	//Obtain Vungle SDK instance
	sdkInstance = AdFactory::GetInstance(vungleAppId);

	//Register event handlers
	sdkInstance->OnAdPlayableChanged += ref new EventHandler<VungleSDK::AdPlayableEventArgs^>(this, &VideoAds::VungleManager::OnOnAdPlayableChanged);
	sdkInstance->OnAdStart += ref new EventHandler<VungleSDK::AdEventArgs^>(this, &VideoAds::VungleManager::OnAdStart);
	sdkInstance->OnAdEnd += ref new EventHandler<VungleSDK::AdEndEventArgs^>(this, &VideoAds::VungleManager::OnAdEnd);
	sdkInstance->OnInitCompleted += ref new EventHandler<VungleSDK::ConfigEventArgs^>(this, &VideoAds::VungleManager::OnInitCompleted);
	sdkInstance->Diagnostic += ref new EventHandler<VungleSDK::DiagnosticLogEvent^>(this, &VideoAds::VungleManager::Diagnostic);

	// Club Vegas vungle key example.
	//  timebonus: "TIMEBONUSREWARDVIDEO-2201094",
	//  challenge: "CHALLENGEREWARDVIDEO-3445873",
	//  in_app_message: "INAPPMESSAGEREWARDVIDEO-3208923",
	//  collecting_game: "COLLECTINGGAMEREWARDVIDEO-6756252",
	//  daily_bonus: "DAILYBONUSREWARDVIDEO-0357136",
	//  shop_vip_bonus: "SHOPREWARDVIDEO-6829385"
	//  epic_pass: "SEASONPASSREWARDVIDEO-6765610"
	//  gem_jackpot: "GEMJACKPOTREWARDVIDEO-9721165"
}

bool VideoAds::VungleManager::IsVideoAdsAvailable(Platform::String^ placement)
{
	if (placementMap.count(placement) > 0 && placementMap.at(placement) == true)
		return true;

	if (sdkInstance != nullptr)
		sdkInstance->LoadAd(placement);

	return false;
}

void VideoAds::VungleManager::LoadPlacement(Platform::String^ placement)
{
	if(IsVideoAdsAvailable(placement)) return;

	placementMap[placement] = false;

	if (sdkInstance != nullptr)
		sdkInstance->LoadAd(placement);
}

void VideoAds::VungleManager::PlayPlacement(Platform::String^ placement, Platform::String^ _unityObjectForOnAdEnd, Platform::String^ _unityMethodNameForOnAdEnd)
{
	unityObjectForOnAdEnd = _unityObjectForOnAdEnd;
	unityMethodNameForOnAdEnd = _unityMethodNameForOnAdEnd;

	if (IsVideoAdsAvailable(placement))
	{
		sdkInstance->PlayAdAsync(ref new AdConfig, placement);
	}
	else
	{
		// Call AdEnd immediately
		unitySendMessage(unityObjectForOnAdEnd->Data(), unityMethodNameForOnAdEnd->Data(), L"");
	}
}
