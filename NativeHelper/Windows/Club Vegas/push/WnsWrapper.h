#pragma once

#include <ppltasks.h>

using namespace Platform;
using namespace Windows::Networking::PushNotifications;

namespace PushNotification
{
	ref class WnsWrapper sealed
	{
	private:
		static WnsWrapper^ instance;
		String^ uri;

		WnsWrapper();

		void OnNotify(PushNotificationChannel^ sender, PushNotificationReceivedEventArgs^ e);

	public:
		static property WnsWrapper^ Instance {
			WnsWrapper^ get() {
				if (instance == nullptr) {
					instance = ref new WnsWrapper();
				}
				return instance;
			}
		}

		property String^ Uri
		{
			String^ get() {
				return uri;
			}

		private:
			void set(String ^value) {
				uri = value;
			}
		}

		void ObtainUri();
	};
}
