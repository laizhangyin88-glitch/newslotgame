using System;
using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Tasks.Actions.BI
{

    [Category("★ BagelCode/BI")]
    public class BI_client_click_profile_page : ActionTask<Blackboard>
    {
        public BBParameter<string> type;
        public BBParameter<string> userIDValue;

        protected override string info
        {
            get
            {
                return string.Format("client_click_profile_me({0})", type);
            }
        }
        
        protected override void OnExecute()
        {
            var userID = BlackboardUtils.FindVariable<string>(agent, userIDValue.value);
            var meID = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "me/userId");

            if(meID.value == userID.value)
            {
                Dictionary<string, object> customData = new Dictionary<string, object>();

                customData["type"] = type.value;
                Analytics.CustomEvent("client_click_profile_page", customData);
            }

            EndAction();
        }
    }

}
