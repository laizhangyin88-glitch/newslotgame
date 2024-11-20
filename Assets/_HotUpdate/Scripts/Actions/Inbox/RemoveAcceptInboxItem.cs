using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using ParadoxNotion;
using BagelCode.OSA_Scroll;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Inbox")]
    public class RemoveAcceptInboxItem : ActionTask<Blackboard>
    {
        protected override string info
        {
            get
            {
                return string.Format("Remove Accept Inbox Item");
            }
        }

        protected override void OnExecute()
        {
            Blackboard currentInboxInfo = agent.GetValue<Blackboard>("_currentInboxInfo");
            int inboxId = currentInboxInfo.GetValue<int>("id");
            int inboxItemId = inboxId;
            var osaItems = agent.GetComponent<OSA_InboxItems>();

            bool isGroup = currentInboxInfo.GetVariable<string>("groupId") != null;
            if (isGroup)
            {
                var groupFirstInfo = currentInboxInfo.GetValue<Blackboard>("groupFirstInfo");
                int groupCount = groupFirstInfo.GetValue<int>("groupCount");

                if(groupCount <= 1)
                {
                    inboxItemId = groupFirstInfo.GetValue<int>("id");
                    osaItems.RemoveItemFromID(inboxItemId);
                }
            }
            else
            {
                osaItems.RemoveItemFromID(inboxItemId);
            }

            BlackboardQueryUtils.RemoveInboxItem(inboxId);
            BlackboardQueryUtils.UpdateCollectAllCredit();

            EventSender.SendGlobalEvent(InboxEvent.REFRESH_INBOX_ITEM);

            EndAction(true);
        }
    }
}
