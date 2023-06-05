#pragma once

using namespace Windows::ApplicationModel::Background;
using namespace Windows::UI::Notifications;

namespace BackgroundTasks
{
	static Platform::String^ BackgroundTaskName = "NotificationActionBackgroundTask";
	static Platform::String^ BackgroundTaskEntryPoint = "BackgroundTasks.NotificationActionBackgroundTask";

	ref class NotificationActionBackgroundTask sealed : IBackgroundTask
	{
	public:
		virtual void Run(IBackgroundTaskInstance^ taskInstance);
	};
}
