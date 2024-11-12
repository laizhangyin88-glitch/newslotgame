using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using SlotMaker.Json;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/ClientAPI")]
    public class RequestVipDealSelectComplete : ActionTask <Blackboard>
    {
        public BBParameter<string> vipDealInfoIDValue;

        protected override string info { get { return "Request Vip Deal Select Complete."; } }

        protected override void OnExecute()
        {
            var vipDealInfoID = BlackboardUtils.FindVariable<int>(agent, vipDealInfoIDValue.value);

            BagelCodeClientAPI.VipDealSelectComplete(vipDealInfoID.value,
            (response) =>
            {
                BlackboardUtils.SetOrCreateValue<bool>(MainBlackboard.Get(), "vipDealInfo/isViewed", true);

                EndAction(true);
            },
            (error) =>
            {
                GlobalErrorHandler.GlobalError(error);
            });
        }
    }
}

