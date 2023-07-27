using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Lobby")]
    public class GetVipDealEvent : ActionTask<Blackboard>
    {
        public BBParameter<bool> saveAs;

        protected override string info
        {
            get
            {
                return string.Format("Check VipDeal");
            }
        }

        protected override void OnExecute()
        {
            if (!BlackboardQueryUtils.IsVipDealTierLock())
            {
                var vipDealInfo = BlackboardQueryUtils.GetActiveVipDealInfo();
                if (vipDealInfo != null)
                    saveAs.value = true;
                else
                    saveAs.value = false;
            }
            else
                saveAs.value = false;

            EndAction();
        }
    }
}