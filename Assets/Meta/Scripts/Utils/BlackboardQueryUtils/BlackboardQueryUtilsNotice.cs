using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{

public static partial class BlackboardQueryUtils
{
    private const string NOTICE_LIST = "noticeList";
    private static string noticeUserTiggeredTimeKey = "NoticeUserTriggeredTime:{0}";
    private static string noticeGlobalTriggeredTimeKey = "NoticeGlobalTriggeredTime:{0}";
    public readonly static string noticeExposureCountKey = "noticeExposureCount:{0}";

    public static void RemoveNoticeItem(Blackboard removeInfo)
    {
        if(removeInfo == null) return;

        var noticeList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), NOTICE_LIST);

        if(noticeList == null) return;

        Variable<int> removeNoticeId = BlackboardUtils.FindVariable<int>(removeInfo, "id");

        if(removeNoticeId != null)
        {
            for(int i=0; i<noticeList.value.Count; ++i)
            {
                Variable<int> id = BlackboardUtils.FindVariable<int>(noticeList.value[i], "id");

                if(id.value == removeNoticeId.value)
                {
                    GameObject.Destroy(noticeList.value[i].gameObject);
                    noticeList.value.RemoveAt(i);
                    break;
                }
            }
        }
    }

    private static bool CheckNoticeType(NoticeTypes[] typeArray, NoticeTypes type)
    {
        for (int i = 0; i < typeArray.Length; ++i)
        {
            if (typeArray[i] == type) return true;
        }
        return false;
    }

    public static List<Blackboard> GetPopupNoticeList()
    {
        List<Blackboard> itemList = new List<Blackboard>();

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        if(!BlackboardQueryUtils.UserOptionsPushNotification() && !NativeHelper.Instance.GetPushNotificationSubscribed())
        {
            itemList = BlackboardQueryUtils.GetNoticeListFromType(NoticeTypes.POPUP_PUSH_OFF);
        }
        else if (!NativeHelper.Instance.GetPushNotificationSubscribed())
        {
            itemList = BlackboardQueryUtils.GetNoticeListFromType(NoticeTypes.POPUP_PUSH_DEVICE_OFF);
        }
        else if (!BlackboardQueryUtils.UserOptionsPushNotification())
        {
            itemList = BlackboardQueryUtils.GetNoticeListFromType(NoticeTypes.POPUP_PUSH_SETTING_OFF);
        }
#endif
        itemList.AddRange( BlackboardQueryUtils.GetNoticeListFromType(NoticeTypes.POPUP, NoticeTypes.TEXT_POPUP) );

        itemList = GetNoticeMaxExposureAndCooltime(itemList);

        return itemList;
    }

    public static List<Blackboard> GetNoticeMaxExposureAndCooltime(List<Blackboard> itemList)
    {
        if (itemList == null) return new List<Blackboard>();

        var maxExposureCount = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "/values/misc/MAX_NOTICE_EXPOSURE_COUNT");
        if (maxExposureCount != null && maxExposureCount.value <= itemList.Count)
        {
            itemList = itemList.GetRange(0, maxExposureCount.value);
        }

        SetCooltime(itemList);
        return itemList;
    }

    public static List<Blackboard> GetNoticeListFromType(params NoticeTypes[] noticeTypes)
    {
        List<Blackboard> itemList = new List<Blackboard>();
        var noticeList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), NOTICE_LIST);
        if (noticeList == null || noticeTypes.Length < 1) return itemList;

        long currentTimestamp = TimeUtils.GetTimeStamp();

        for (int i = 0; i < noticeList.value.Count; ++i)
        {
            if(!IsValidNotice(noticeList.value[i]))
                continue;

            var type = BlackboardUtils.FindVariable<NoticeTypes>(noticeList.value[i], "type");

            if (CheckNoticeType(noticeTypes, type.value))
            {
                var coolTime = BlackboardUtils.FindVariable<int>(noticeList.value[i], "constraints/cooltimeSec");
                if (coolTime.value != 0)
                {
                    long globalCoolTime = (long)coolTime.value * 1000L;
                    string triggeredTimeKey = string.Format(noticeGlobalTriggeredTimeKey, noticeList.value[i].GetValue<int>("id"));
                    long triggerTimestamp = PlayerPrefsUtils.GetOrCreateInt64(triggeredTimeKey, 0);

                    if (triggerTimestamp == 0 || (triggerTimestamp + globalCoolTime) < currentTimestamp)
                    {
                        // Change cooltime set logic. We will manual set. Use a SetCooltime function.
                        // PlayerPrefsUtils.SetInt64(triggeredTimeKey, currentTimestamp);
                    }
                    else
                    {
                        continue; // Is CoolTime
                    }
                }
                itemList.Add(noticeList.value[i]);
            }
        }

        return itemList;
    }

    public static void SetCooltime(List<Blackboard> noticeList)
    {
        for (int i = 0; i < noticeList.Count; ++i)
            SetCooltime(noticeList[i]);
    }

    public static void SetCooltime(Blackboard noticeBB)
    {
        var coolTime = BlackboardUtils.FindVariable<int>(noticeBB, "constraints/cooltimeSec");
        if (coolTime.value != 0)
        {
            long globalCoolTime = (long)coolTime.value * 1000L;
            string triggeredTimeKey = string.Format(noticeGlobalTriggeredTimeKey, noticeBB.GetValue<int>("id"));
            PlayerPrefsUtils.SetInt64(triggeredTimeKey, TimeUtils.GetTimeStamp());
        }
    }

    public static Blackboard GetNoticeFromID(int findID)
    {
        var noticeList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), NOTICE_LIST);

        if(noticeList == null) return null;

        for(int i=0; i<noticeList.value.Count; ++i)
        {
            Variable<int> id = BlackboardUtils.FindVariable<int>(noticeList.value[i], "id");

            if(id.value == findID)
            {
                return noticeList.value[i];
            }
        }
        return null;
    }

    private static bool HasUserTimer(Blackboard noticeInfo)
    {
        if (noticeInfo == null) return false;

        var isUserTimer = BlackboardUtils.FindVariable<bool>(noticeInfo, "constraints/useUserTimer");
        var userTimerMin = BlackboardUtils.FindVariable<int>(noticeInfo, "constraints/userTimer");

        if (isUserTimer.value && userTimerMin.value != 0)
        {
            return true;
        }

        return false;
    }

    public static void SetNoticeUserTimer(Blackboard noticeInfo)
    {
        string key = string.Format(noticeUserTiggeredTimeKey, noticeInfo.GetValue<int>("id"));

        long userTriggeredTimestamp = PlayerPrefsUtils.GetOrCreateInt64(key, 0);

        if (HasUserTimer(noticeInfo) && userTriggeredTimestamp == 0L)
        {
            PlayerPrefsUtils.SetInt64(key, TimeUtils.GetTimeStamp());
        }
    }

    public static long GetNoticeEndTimestamp(Blackboard noticeInfo)
    {
        string key = string.Format(noticeUserTiggeredTimeKey, noticeInfo.GetValue<int>("id"));

        long endTimestamp = noticeInfo.GetValue<long>("endTimestamp");
        long userTriggeredTimestamp = PlayerPrefsUtils.GetOrCreateInt64(key, 0);

        if (HasUserTimer(noticeInfo) && userTriggeredTimestamp != 0L)
        {
            var userTimerMin = BlackboardUtils.FindVariable<int>(noticeInfo, "constraints/userTimer");
            long userEndTimestamp = userTriggeredTimestamp + (long)userTimerMin.value * 60000L;

            if (endTimestamp == 0 || userEndTimestamp < endTimestamp)
            {
                endTimestamp = userEndTimestamp;
            }
        }

        return endTimestamp;
    }

    public static bool IsValidNotice(Blackboard noticeInfo)
    {
        bool isValid = true;

        var action = BlackboardUtils.FindVariable<Blackboard>(noticeInfo, "action");
        if(action == null || action.value == null) return isValid;

        ActionType actionType = action.value.GetValue<ActionType>("type");

        switch(actionType)
        {
            case ActionType.PIN_TO_TASKBAR:
                if( NativeHelper.Instance.IsPinningAllowed()
                    && false == BlackboardUtils.GetOrCreateVariable<bool>(MainBlackboard.Get(), "isWindowsPinned", true).value)
                    isValid = true;
                else
                    isValid = false;
                break;
            default:
                isValid = true;
                break;
        }

        return isValid;
    }
}

}
