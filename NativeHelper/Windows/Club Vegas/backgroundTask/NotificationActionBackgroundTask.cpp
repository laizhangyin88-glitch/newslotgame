#include "pch.h"
#include "NotificationActionBackgroundTask.h"

using namespace Windows::ApplicationModel::Background;
using namespace Windows::UI::Notifications;

using namespace BackgroundTasks;

void NotificationActionBackgroundTask::Run(IBackgroundTaskInstance^ taskInstance) {
	auto details = dynamic_cast<ToastNotificationActionTriggerDetail^>(taskInstance->TriggerDetails);
	if (details == nullptr) {
		return;
	}
}
