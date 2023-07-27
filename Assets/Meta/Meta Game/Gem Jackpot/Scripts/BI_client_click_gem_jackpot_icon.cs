using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{
    [Category("★ BagelCode/BI")]
    public class BI_client_click_gem_jackpot_icon : ActionTask
    {
        public BBParameter<int> selectIndex;
        public BBParameter<string> biContextID;

        protected override string info
        {
            get { return string.Format("BI click gemjackpot icon ({0})", (selectIndex.value != 0)); }
        }

        protected override void OnExecute()
        {
            bool is_freespin = selectIndex.value != 0;

            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["is_freespin"] = is_freespin;
            customData["context_id"] = biContextID.value;
            customData["slot_enter_context_id"] = GemJackpotUtils.BISlotEnterContextID;

            Analytics.CustomEvent("client_click_gem_jackpot_icon", customData);

            EndAction();
        }
    }
}