using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

    [Category("★ BagelCode/Contents")]
    public class BeginBonus : ActionTask
    {
        public BBParameter<int> bonusId;

        protected override string info { get { return string.Format("Begin Bonus {0}", bonusId); } }

        protected override void OnExecute()
        {
            var cb = ContentBlackboard.Get();
            var spin = cb.GetValue<Blackboard>("spin");
            var parent = cb.GetValue<Blackboard>("current"); 

            var bonus = (Blackboard)BlackboardUtils.CreateBlackboard("bonus");
            var bonusList = BlackboardUtils.AddToBlackboardList(parent, "bonusList", bonus);
            BlackboardUtils.SetOrCreateValue<Blackboard>(cb, "bonus", bonus);
            cb.SetValue("current", bonus);

            string guid = Guid.NewGuid().ToString();
            Blackboard response = ContentBlackboardUtils.GetBonusResponse(spin, bonusId.value);
            long timestamp = MetaSystem.GetTimeStamp();

            BlackboardUtils.SetOrCreateValue<int>(bonus, "bonusIndex", bonusList.Count - 1);
            BlackboardUtils.SetOrCreateValue<ContentNodeType>(bonus, "type", ContentNodeType.Bonus);
            BlackboardUtils.SetOrCreateValue<Blackboard>(bonus, "parent", parent);
            BlackboardUtils.SetOrCreateValue(bonus, "uid", guid);
            BlackboardUtils.SetOrCreateValue<int>(bonus, "bonusId", this.bonusId.value);
            BlackboardUtils.SetOrCreateValue<Blackboard>(bonus, "response", response);
            BlackboardUtils.SetOrCreateValue<long>(bonus, "earnCredit", 0L);
            BlackboardUtils.SetOrCreateValue<long>(bonus, "singleCredit", 0L);
            BlackboardUtils.SetOrCreateValue<long>(bonus, "beginTime", timestamp);

            ContentEvent.BeginBonus(bonus);

            EndAction();
        }
    }

}
