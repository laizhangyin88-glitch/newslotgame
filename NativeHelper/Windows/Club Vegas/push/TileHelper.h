#pragma once

using namespace Platform;
using namespace Windows::Networking::PushNotifications;
using namespace Windows::UI::Notifications;
using namespace Windows::Data::Xml::Dom;

namespace PushNotification
{
	ref class TileHelper sealed
	{
	private:
		static TileHelper^ instance;

		TileHelper();

		TileUpdater^ tileUpdaterForApplication;
		property TileUpdater^ TileUpdaterForApplication {
			TileUpdater^ get() {
				if (tileUpdaterForApplication == nullptr) {
					tileUpdaterForApplication = TileUpdateManager::CreateTileUpdaterForApplication();
				}
				return tileUpdaterForApplication;
			}
		}
		static XmlDocument^ createContentDoc(String^ title, String^ body);

	public:
		static property TileHelper^ Instance {
			TileHelper^ get() {
				if (instance == nullptr) {
					instance = ref new TileHelper();
				}
				return instance;
			}
		}

		void createNotification(String^ title, String^ body, int expireInSec);
		void createScheduledNotification(String^ pushId, String^ title, String^ body, int delayInSec);
		void deleteScheduledNotification(String^ pushId);
		void clearNotifications();
	};
}
