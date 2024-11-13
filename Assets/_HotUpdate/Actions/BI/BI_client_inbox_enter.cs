using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{
    [Category("★ BagelCode/BI")]
    public class BI_client_inbox_enter : ActionTask<Blackboard>
    {
        protected override void OnExecute()
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["context_id"] = InboxUtils.GetInboxEnterContextID();

            Analytics.CustomEvent("client_inbox_enter", customData);

            EndAction();
        }
    }
}