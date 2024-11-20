using System;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using BagelCode.ClientModels;
using System.Collections.Generic;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Inbox")]
    public class UpdateGroupRewardInfo : ActionTask<Blackboard>
    {
        [BlackboardOnly]
        public BBParameter<Blackboard> inboxInfo;

        protected override string info
        {
            get { return "Update Group Reward Info"; }
        }

        protected override void OnExecute()
        {
            bool isGroup = inboxInfo.value.GetVariable<string>("groupId") != null;
            if (isGroup)
            {
                var title = BlackboardUtils.FindVariable<string>(inboxInfo.value, "title");
                var message = BlackboardUtils.FindVariable<string>(inboxInfo.value, "message");

                var groupFirst = inboxInfo.value.GetValue<Blackboard>("groupFirstInfo");
                var groupCountVar = groupFirst.GetVariable<int>("groupCount");

                var messageVar = agent.GetVariable<string>("_message");

                messageVar.value = StringTableUtils.GetString(StringTable.StringTableType.Global,
                    "INBOX_ITEM_TEXT_MESSAGE", title.value, message.value);

                if (groupCountVar.value >= 2)
                {
                    inboxInfo.value = inboxInfo.value.GetVariable<Blackboard>("nextInboxInfo").value;
                }

                string groupCountText = StringTableUtils.GetString(StringTable.StringTableType.Global, "INBOX_ITEM_SCRATCHER_GROUP", groupCountVar.value - 1);
                if (messageVar.value.Contains("{group_count}"))
                    messageVar.value = messageVar.value.Replace("{group_count}", groupCountText);

                var rootElement = agent.gameObject.GetComponent<ContextElement>();
                var messageElement = ContextUtils.FindElement(rootElement, "Text Message", ContextSearchingType.ChildrenSearch);
                if (messageElement is ContextTextMeshProUGUI remainingCountText)
                    remainingCountText.SetText(messageVar.value);
            }
            else
            {
                var anim = agent.gameObject.GetComponent<Animator>();
                if (anim != null) anim.SetTrigger("Disappear");
            }

            EndAction();
        }
    }
}