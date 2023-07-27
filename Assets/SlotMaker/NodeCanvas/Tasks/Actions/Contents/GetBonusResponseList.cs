using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
    [Category("★ BagelCode/Contents")]
    public class GetBonusResponseList : ActionTask
    {
        public BBParameter<int> bonusId;

        [BlackboardOnly]
        public BBParameter<List<Blackboard>> responseList;

        protected override string info
        {
            get { return string.Format("Get All Bonus Responses by {0} and save as {1}", bonusId, responseList); }
        }

        protected override void OnExecute()
        {
            var spin = BlackboardUtils.FindVariable<Blackboard>(null, "./spin");
            if (spin == null || spin.value == null)
            {
                EndAction(false);
                return;
            }
            responseList.value = ContentBlackboardUtils.GetBonusResponseList(spin.value, bonusId.value);
            EndAction();
        }
    }
}
