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
    public class BeginBonusNew : ActionTask
    {
        public BBParameter<int> bonusId;
        public BBParameter<string> bonusName;
        public BBParameter<string> responseNew;
        protected override string info {
            //get { return string.Format("Begi Bonusn New {0} {1}", bonusId, bonusName); }
            get { return string.Format("Begin Bonus New {0}", bonusName); }
        }

        protected override void OnExecute()
        {
            Blackboard bonus = CreatBonus(responseNew.value, bonusName.value, bonusId.value);

            ContentEvent.BeginBonus(bonus);

            EndAction();
        }

        public static Blackboard CreatBonus(string responseNew,string name, int bonusId = 0)
        {
            var cb = ContentBlackboard.Get();

            var parent = cb.GetValue<Blackboard>("current");

            var bonus = (Blackboard)BlackboardUtils.CreateBlackboard("bonus");
            var bonusList = BlackboardUtils.AddToBlackboardList(parent, "bonusList", bonus);
            BlackboardUtils.SetOrCreateValue<Blackboard>(cb, "bonus", bonus);
            cb.SetValue("current", bonus);

            //Blackboard spin = cb.GetValue<Blackboard>("spin");
            //JSONNode node = JSONNode.Parse(spin.GetValue<string>("responseNew"));
            //Blackboard response = ContentBlackboardUtils.GetBonusResponse(spin, bonusId.value);

            BlackboardUtils.SetOrCreateValue<int>(bonus, "bonusIndex", bonusList.Count - 1);
            BlackboardUtils.SetOrCreateValue<ContentNodeType>(bonus, "type", ContentNodeType.Bonus);
            BlackboardUtils.SetOrCreateValue<Blackboard>(bonus, "parent", parent);
            BlackboardUtils.SetOrCreateValue(bonus, "uid", Guid.NewGuid().ToString());
            BlackboardUtils.SetOrCreateValue<int>(bonus, "bonusId", bonusId);
            BlackboardUtils.SetOrCreateValue<string>(bonus, "bonusName", name);
            //BlackboardUtils.SetOrCreateValue<Blackboard>(bonus, "response", response); // 获取上个spin/bonus_result中该bonus节点的数据(包含本包最终获得的金钱)
            //{"bonus_id":2101,"type":1,"earn_credit":3899,"result":{"is_cash_wheel":false,"is_jackpot":true,"jackpot_index":1,"jackpot_award_amount":3899,"context_id":"45fcdfab-d9cc-445b-8244-3cd65ed3e9ba"},"claim_type":1,"uid":"171271880676829890"}
            BlackboardUtils.SetOrCreateValue<string>(bonus, "responseNew", responseNew);
            BlackboardUtils.SetOrCreateValue<long>(bonus, "earnCredit", 0L);
            BlackboardUtils.SetOrCreateValue<long>(bonus, "singleCredit", 0L);
            BlackboardUtils.SetOrCreateValue<long>(bonus, "beginTime", MetaSystem.GetTimeStamp());

            return bonus;
        }

    }

}
