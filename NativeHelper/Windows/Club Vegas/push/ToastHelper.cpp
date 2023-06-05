#include "pch.h"
#include "push/ToastHelper.h"
#include "lib/utils.h"
#include "backgroundTask/NotificationActionBackgroundTask.h"

using namespace PushNotification;

using namespace Windows::Foundation;
using namespace Windows::UI::Notifications;
using namespace Windows::Data::Xml::Dom;
using namespace concurrency;
using namespace Windows::ApplicationModel::Background;
//using Windows.ApplicationModel.Core;

ToastHelper^ ToastHelper::instance = nullptr;

ToastHelper::ToastHelper() {
}

XmlDocument^ ToastHelper::createContentDoc(String^ title, String^ body) {
	String^ toastVisual =
		"<visual>" +
		"<binding template = 'ToastGeneric'>" +
		"<text>" + title + "</text>" +
		"<text hint-style = 'captionSubtle'>" + body + "</text>" +
		"</binding>" +
		"</visual>";
	String^ toastActions =
		"<actions>" +
		"<action activationType = 'background' arguments = 'ok' content = 'Launch App' />" +
		"<action activationType = 'background' arguments = 'cancel' content = 'Later' />" +
		"</actions>";
	String^ content =
		"<toast activationType='background' launch='args'>" +
		toastVisual +
		toastActions +
		"</toast>";
	XmlDocument^ doc = ref new XmlDocument();
	doc->LoadXml(content);
	return doc;
}

void ToastHelper::init() {
	//for (const auto& cur : BackgroundTaskRegistration::AllTasks)
	//{
	//	auto registration = dynamic_cast<BackgroundTaskRegistration^>(cur->Value);
	//	if (registration->Name == BackgroundTasks::BackgroundTaskName)
	//	{
	//		registered = true;
	//		break;
	//	}
	//}
	RegisterBackgroundTask();
}

void ToastHelper::createNotification(String^ title, String^ body, int expireInSec) {
	auto content = createContentDoc(title, body);
	ToastNotification^ notification = ref new ToastNotification(content);
	notification->ExpirationTime = DateTimeOffset(expireInSec);
	notification->Tag = TAG;
	notification->Group = GROUP;
	Notifier->Show(notification);
}

void ToastHelper::createScheduledNotification(String^ pushId, String^ title, String^ body, int delayInSec)
{
	auto content = createContentDoc(title, body);
	auto deliveryTime = DateTimeOffset(delayInSec);
	ScheduledToastNotification^ notification = ref new ScheduledToastNotification(content, deliveryTime);
	notification->Id = pushId;
	notification->Tag = TAG;
	notification->Group = GROUP;
	Notifier->AddToSchedule(notification);
}

void ToastHelper::deleteScheduledNotification(String^ pushId) {
	auto notifications = Notifier->GetScheduledToastNotifications();
	for each (auto schedule in notifications) {
		if (schedule->Id == pushId) {
			Notifier->RemoveFromSchedule(schedule);
			break;
		}
	}
}

void ToastHelper::clearNotifications() {
	// https://msdn.microsoft.com/en-us/library/windows/apps/xaml/dn631260.aspx?f=255&MSPPError=-2147217396
	ToastNotificationManager::History->RemoveGroup(GROUP);
}

void ToastHelper::RegisterBackgroundTask()
{
	// Unregister any previous exising background task
	UnregisterBackgroundTask();

	// Request access
	auto statusOperation = BackgroundExecutionManager::RequestAccessAsync();
	auto statusTask = create_task(statusOperation);
	statusTask.then([this](BackgroundAccessStatus status) {
		// If denied
		if (status != BackgroundAccessStatus::AlwaysAllowed && status != BackgroundAccessStatus::AllowedSubjectToSystemPolicy) {
			return false;
		}
		// Construct the background task
		auto builder = ref new BackgroundTaskBuilder();
		builder->Name = BackgroundTasks::BackgroundTaskName;
		builder->TaskEntryPoint = BackgroundTasks::BackgroundTaskEntryPoint;

		// Set trigger for Toast History Changed
		builder->SetTrigger(ref new ToastNotificationActionTrigger());

		// And register the background task
		auto registration = builder->Register();
		//registration->Completed += ref new BackgroundTaskCompletedEventHandler(this, &Scenario4_BackgroundActivity::OnCompleted);
		registered = true;
		return true;
	}, task_continuation_context::use_current());
}

void ToastHelper::UnregisterBackgroundTask()
{
	for (const auto& cur : BackgroundTaskRegistration::AllTasks)
	{
		auto registration = dynamic_cast<BackgroundTaskRegistration^>(cur->Value);
		
		if (registration->Name == BackgroundTasks::BackgroundTaskName) {
			registration->Unregister(true);
			registered = false;
			break;
		}
	}
}
