using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode
{
    public static partial class BlackboardQueryUtils
    {
        public static void SetPushNoticeData(string type, string comment, long ts, string templateId, string couponCode)
        {
            var meBB = BlackboardUtils.FindVariable<Blackboard>(null, "/me");

            if (ts != 0L)
                ts = TimeUtils.GetTimeStamp() - ts;

            if(meBB == null)
            {
                var bb = SlotMaker.BlackboardUtils.GetOrCreateBlackboard(SlotMaker.MainBlackboard.Get(), "pushInfo");

                var typeVariable        = bb.AddVariable("type", typeof(string));
                var commentVariable     = bb.AddVariable("comment", typeof(string));
                var tsVariable          = bb.AddVariable("ts", typeof(long));
                var templeIdVariable    = bb.AddVariable("templateId", typeof(string));
                var couponCodeVariable  = bb.AddVariable("couponCode", typeof(string));

                typeVariable.value       = type;
                commentVariable.value    = comment;
                tsVariable.value         = ts;
                templeIdVariable.value   = templateId;
                couponCodeVariable.value = couponCode;
            }
            else
            {
                BI_Click_PN(type, comment, ts, templateId, couponCode);
            }
        }

        public static void BI_Click_PN(string type, string comment, long ts, string templateId, string couponCode)
        {
            string pnId = string.Empty;
            Variable pnIdVariable = BlackboardUtils.FindVariable<string>("/lastNotificationId");
            if (pnIdVariable != null && pnIdVariable.value != null)
            {
                pnId = pnIdVariable.value as string;
            }

            string onesignalId = string.Empty;
            Variable onesignalIdVariable = BlackboardUtils.FindVariable<string>("/oneSignalId");
            if (onesignalIdVariable != null && onesignalIdVariable.value != null)
            {
                onesignalId = onesignalIdVariable.value as string;
            }

            Analytics.CustomEvent("client_click_pn", new Dictionary<string, object>
            {
                { "message_type", type },
                { "comment", comment },
                { "distance_from_receiving_pn", ts },
                { "template_id", templateId},
                { "pn_id", pnId },
                { "onesignal_id", onesignalId },
                { "coupon_code", couponCode }
            });

            BlackboardUtils.DestroyBlackboard(MainBlackboard.Get(), "pushInfo");
        }

        public static string GetDevicePushSetting()
        {
    #if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            return NativeHelper.Instance.GetPushNotificationSubscribed() ? "ON" : "OFF";
    #else
            return null;
    #endif
        }

        public static void BI_Device_Push_Setting(string contextID)
        {
            Analytics.CustomEvent("client_device_push_setting", new Dictionary<string, object>
            {
                { "context_id", contextID },
                { "device_push_setting", GetDevicePushSetting() }
            });
        }

        public static bool IsPipModeEnabled()
        {
            var isPipModeEnabled = BlackboardUtils.FindVariable<bool>( MainBlackboard.Get(), "isPipModeEnabled" );

            if(isPipModeEnabled != null)
                return isPipModeEnabled.value;

            return false;
        }

        public static bool IsPipModeSettingsEnabled()
        {
            var variable = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "/userOptions/enablePipMode");

            if (variable != null)
                return variable.value;

            return false;
        }

        public static bool IsPipTriggerButtonEnabled()
        {
            return BlackboardUtils.FindVariable<bool>("/isPipTriggerButtonEnabled")?.value ?? false;
        }
    }
}

