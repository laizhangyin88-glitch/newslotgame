#pragma once

using namespace Platform;
using namespace Windows::Networking::PushNotifications;
using namespace Windows::UI::Notifications;
using namespace Windows::Data::Xml::Dom;

namespace PushNotification
{
	ref class ToastHelper sealed
	{
	private:
		static ToastHelper^ instance;

		bool registered;

		String^ const TAG = "BAGELCODE";
		String^ const GROUP = "BAGELCODE";

		ToastHelper();

		ToastNotifier^ notifier;
		property ToastNotifier^ Notifier {
			ToastNotifier^ get() {
				if (notifier == nullptr) {
					notifier = ToastNotificationManager::CreateToastNotifier();
				}
				return notifier;
			}
		}

		static XmlDocument^ createContentDoc(String^ title, String^ body);
		void RegisterBackgroundTask();
		void UnregisterBackgroundTask();

	public:
		static property ToastHelper^ Instance {
			ToastHelper^ get() {
				if (instance == nullptr) {
					instance = ref new ToastHelper();
				}
				return instance;
			}
		}

		void init();
		void createNotification(String^ title, String^ body, int expireInSec);
		void createScheduledNotification(String^ pushId, String^ title, String^ body, int delayInSec);
		void deleteScheduledNotification(String^ pushId);
		void clearNotifications();
	};
}
