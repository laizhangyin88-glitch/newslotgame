using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{
    [Category("★ BagelCode/BI")]
    public class BI_client_club_enter : ActionTask<Blackboard>
    {
        public BBParameter<string> contextID;
        public BBParameter<string> enterType;

        protected override void OnExecute()
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();

            if (contextID != null && !string.IsNullOrEmpty(contextID.value))
                customData["context_id"] = contextID.value;
            if (enterType != null && !string.IsNullOrEmpty(enterType.value))
                customData["type"] = enterType.value;

            Analytics.CustomEvent("client_club_enter", customData);

            EndAction();
        }
    }
}