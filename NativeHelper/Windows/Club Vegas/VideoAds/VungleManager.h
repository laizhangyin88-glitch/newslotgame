#pragma once

namespace VideoAds
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	ref class VungleManager sealed
	{
	private:
		VungleManager();
		static VungleManager^ instance;

		std::map<Platform::String^, bool> placementMap;
		VungleSDK::VungleAd^ sdkInstance;

		void OnInitCompleted(Platform::Object^ sender, VungleSDK::ConfigEventArgs^ args);
		void OnOnAdPlayableChanged(Platform::Object^ sender, VungleSDK::AdPlayableEventArgs^ args);
		void OnAdStart(Platform::Object^ sender, VungleSDK::AdEventArgs^ e);
		void OnAdEnd(Platform::Object^ sender, VungleSDK::AdEndEventArgs^ e);
		void Diagnostic(Platform::Object^ sender, VungleSDK::DiagnosticLogEvent^ e);

	public:
		static VungleManager^ getInstance();
		void Init();
		bool IsVideoAdsAvailable(Platform::String^ placement);
		void LoadPlacement(Platform::String^ placement);
		void PlayPlacement(Platform::String^ placement, Platform::String^ _unityObjectForOnAdEnd, Platform::String^ _unityMethodNameForOnAdEnd);
	};
}
