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
    public class BI_client_click_gem_jackpot_close : ActionTask
    {
        protected override string info
        {
            get { return string.Format("BI click gemjackpot close"); }
        }

        protected override void OnExecute()
        {
            //client_click_gem_jackpot_close
            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["slot_enter_context_id"] = GemJackpotUtils.BISlotEnterContextID;
            customData["type"] = GemJackpotUtils.IsClickedNoDealButton ? "NoDeal" : "X";
            customData["remain_freespin_count"] = GemJackpotUtils.FreeSpinCount;
            customData["amount_of_reward"] = GemJackpotUtils.GetRewardCredit();

            Analytics.CustomEvent("client_click_gem_jackpot_close", customData);

            EndAction();
        }
    }
}