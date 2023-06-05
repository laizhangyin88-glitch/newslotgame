#include "pch.h"
#include "push/WnsWrapper.h"

using namespace PushNotification;

using namespace Windows::Foundation;
using namespace Windows::UI::Notifications;
using namespace Windows::Data::Xml::Dom;
using namespace Windows::Networking::PushNotifications;
using namespace concurrency;

WnsWrapper^ WnsWrapper::instance = nullptr;

WnsWrapper::WnsWrapper() {
	uri = nullptr;
}

void WnsWrapper::ObtainUri() {
	IAsyncOperation<PushNotificationChannel^>^ channelOperation = PushNotificationChannelManager::CreatePushNotificationChannelForApplicationAsync();
	auto channelTask = create_task(channelOperation);
	channelTask.then([this](PushNotificationChannel^ channel) {
		this->Uri = channel->Uri;
		channel->PushNotificationReceived += ref new TypedEventHandler<PushNotificationChannel^, PushNotificationReceivedEventArgs^>(this, &WnsWrapper::OnNotify);
	}, task_continuation_context::use_current());
}

void WnsWrapper::OnNotify(PushNotificationChannel^ sender, PushNotificationReceivedEventArgs^ e) {
	String^ notificationContent = nullptr;
	switch (e->NotificationType) {
	case PushNotificationType::Tile:
		notificationContent = e->BadgeNotification->Content->GetXml();
		break;
	case PushNotificationType::Toast:
		notificationContent = e->BadgeNotification->Content->GetXml();
		break;
	}

	// Setting the cancel property prevents the notification from being delivered. It's especially important to do this for toasts:
	// if your application is already on the screen, there's no need to display a toast from push notifications.
	e->Cancel = true;
};
