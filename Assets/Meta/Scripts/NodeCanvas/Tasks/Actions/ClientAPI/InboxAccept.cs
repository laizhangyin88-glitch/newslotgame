using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{

    [Category("★ BagelCode/ClientAPI")]
    public class InboxAccept : ActionTask<Blackboard>
    {
        public BBParameter<Blackboard> inboxInfo;

        public bool isSuccess;

        protected override string info
        {
            get
            {
                return string.Format("Accept Inbox {0}", inboxInfo);
            }
        }

        protected override void OnExecute()
        {
            isSuccess = InboxUtils.InboxAcceptRequest(agent, inboxInfo);

            EndAction();
        }
    }
}