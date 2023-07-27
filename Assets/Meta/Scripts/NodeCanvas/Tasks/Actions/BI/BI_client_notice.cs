using UnityEngine;
using System.Text;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_client_notice : ActionTask<Blackboard>
{
    public BBParameter<bool>   isClick;
    public BBParameter<string> noticeInfoValue;
    public BBParameter<long>   endTimestamp;
    public BBParameter<string> contextID;

    protected override string info
    {
        get { return string.Format("BI Client Notice({0}, {1})", isClick.value, noticeInfoValue); }
    }

    protected override void OnExecute()
    {
        var noticeInfoBB = BlackboardUtils.FindVariable<Blackboard>(agent, noticeInfoValue.value);
        if(noticeInfoBB != null)
        {
            var noticeID = BlackboardUtils.FindVariable<int>(noticeInfoBB.value, "id");
            var title = BlackboardUtils.FindVariable<string>(noticeInfoBB.value, "title");
            var message = BlackboardUtils.FindVariable<string>(noticeInfoBB.value, "message");
            var priority = BlackboardUtils.FindVariable<int>(noticeInfoBB.value, "priority");
            var imageUrl = BlackboardUtils.FindVariable<string>(noticeInfoBB.value, "imageUrl");
            var startTimestamp = BlackboardUtils.FindVariable<long>(noticeInfoBB.value, "startTimestamp");
            // var endTimestamp = BlackboardUtils.FindVariable<long>(noticeInfoBB.value, "endTimestamp");
            var abTestTag = BlackboardUtils.FindVariable<int>(noticeInfoBB.value, "abTestTag");
            var noticeType = BlackboardUtils.FindVariable<NoticeTypes>(noticeInfoBB.value, "type");

            if (string.IsNullOrEmpty(contextID.value))
                contextID.value = BiEventUtils.GenerateContextID();

            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["type"] = isClick.value ? "click" : "trigger";
            customData["notice_id"] = noticeID.value;
            customData["context_id"] = contextID.value;
            customData["notice_type"] = noticeType.value.ToString();

            var actionBB = BlackboardUtils.FindVariable<Blackboard>(noticeInfoBB.value, "action");
            if(actionBB != null)
                BiEventUtils.AppendNoticeActionData(customData, actionBB.value);

            customData["priority"] = priority.value;
            customData["ab_test_tag"] = abTestTag.value;
            customData["title"] = title.value;
            customData["message"] = message.value;
            customData["image_url"] = imageUrl.value;

            var constraintsBB = BlackboardUtils.FindVariable<Blackboard>(noticeInfoBB.value, "constraints");
            if(constraintsBB != null)
            {
                var showEndTimer = BlackboardUtils.FindVariable<bool>(constraintsBB.value, "showEndTimer");
                var useUserTimer = BlackboardUtils.FindVariable<bool>(constraintsBB.value, "useUserTimer");
                var userTimeMin = BlackboardUtils.FindVariable<int>(constraintsBB.value, "userTimer");
                var coolTimeSec = BlackboardUtils.FindVariable<int>(constraintsBB.value, "cooltimeSec");

                customData["cooltime_sec"] = coolTimeSec.value;
                customData["show_end_timer"] = showEndTimer.value;
                customData["use_user_timer"] = useUserTimer.value;
                customData["user_timer_min"] = userTimeMin.value;

                if(useUserTimer.value)
                {
                    long userTimerMS = userTimeMin.value * 60000L;
                    startTimestamp.value = endTimestamp.value - userTimerMS;
                }
                else
                {
                    endTimestamp.value = BlackboardUtils.FindVariable<long>(noticeInfoBB.value, "endTimestamp").value;
                }
            }

            customData["start_timestamp"] = startTimestamp.value;
            customData["end_timestamp"] = endTimestamp.value;

            Analytics.CustomEvent("client_notice", customData);
        }

        EndAction();
    }
}

}
