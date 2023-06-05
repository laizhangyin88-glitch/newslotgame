#include "pch.h"
#include "push/TileHelper.h"
#include "lib/utils.h"

using namespace PushNotification;

using namespace Windows::Foundation;
using namespace Windows::UI::Notifications;
using namespace Windows::Data::Xml::Dom;
using namespace concurrency;

TileHelper^ TileHelper::instance = nullptr;

TileHelper::TileHelper() {
}

XmlDocument^ TileHelper::createContentDoc(String^ title, String^ body) {
	String^ tileMedium =
		"<binding template = 'TileMedium'>" +
		"<text>" + title + "</text>" +
		"<text hint-style = 'captionSubtle'>" + body + "</text>" +
		"</binding>";
	String^ tileWide =
		"<binding template = 'TileWide'>" +
		"<text hint-style = 'subtitle'>" + title + "</text>" +
		"<text hint-style = 'captionSubtle'>" + body + "</text>" +
		"</binding>";
	String^ content = "<tile><visual>" + tileMedium + tileWide + "</visual></tile>";
	XmlDocument^ doc = ref new XmlDocument();
	doc->LoadXml(content);
	return doc;
}

void TileHelper::createNotification(String^ title, String^ body, int expireInSec) {
	auto content = createContentDoc(title, body);
	TileNotification^ notification = ref new TileNotification(content);
	notification->ExpirationTime = DateTimeOffset(expireInSec);
	TileUpdaterForApplication->Update(notification);
}

void TileHelper::createScheduledNotification(String^ pushId, String^ title, String^ body, int delayInSec) {
	auto content = createContentDoc(title, body);
	auto deliveryTime = DateTimeOffset(delayInSec);
	ScheduledTileNotification^ tileNotification = ref new ScheduledTileNotification(content, deliveryTime);
	tileNotification->Id = pushId;
	TileUpdaterForApplication->AddToSchedule(tileNotification);
}

void TileHelper::deleteScheduledNotification(String^ pushId) {
	auto notifications = TileUpdaterForApplication->GetScheduledTileNotifications();
	for each (auto schedule in notifications) {
		if (schedule->Id == pushId) {
			TileUpdaterForApplication->RemoveFromSchedule(schedule);
			break;
		}
	}
}

void TileHelper::clearNotifications() {
	// Note that scheduled notifications that haven¡¯t yet appeared are NOT cleared by this method.
	TileUpdaterForApplication->Clear();
}
