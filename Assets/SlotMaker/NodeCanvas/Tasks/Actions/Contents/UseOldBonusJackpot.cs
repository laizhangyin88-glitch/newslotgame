using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using SimpleJSON;
using BagelCode.Protobuf;

namespace BagelCode.Tasks.Actions.Contents
{

    [Category("★ BagelCode/Contents")]
    public class UseOldBonusJackpot : ActionTask
    {
        public BBParameter<long> earnCredit;
        protected override string info
        {
            //get { return string.Format("Begi Bonusn New {0} {1}", bonusId, bonusName); }
            get { return string.Format("creat old bonus jackpot node, earnCredit = {0}", earnCredit); }
        }

        protected override void OnExecute()
        {
            CreatJackpotInfo(earnCredit.value);
            EndAction();
        }

        public static Blackboard CreatJackpotInfo(long earnCredit)
        {
            var cb = ContentBlackboard.Get();
            var bonus = cb.GetValue<Blackboard>("bonus");
/*
        {
            "bonus_id": 2101,
            "type": 1,
            "earn_credit": 3899,
            "result": {
                "is_cash_wheel": false,
                "is_jackpot": true,
                "jackpot_index": 1,
                "jackpot_award_amount": 3899,
                "context_id": "45fcdfab-d9cc-445b-8244-3cd65ed3e9ba"
            },
            "claim_type": 1,
            "uid": "171271880676829890"
        }
*/

            Blackboard response = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(cb, "response");
            BlackboardUtils.SetOrCreateValue<long>(response, "earnCredit", earnCredit);
            //Blackboard result = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(response, "result");
            //BlackboardUtils.SetOrCreateValue<long>(result, "jackpotAwardAmount", earnCredit);
            BlackboardUtils.SetOrCreateValue<Blackboard>(bonus, "response", response);

            return response;
        }

    }

}
