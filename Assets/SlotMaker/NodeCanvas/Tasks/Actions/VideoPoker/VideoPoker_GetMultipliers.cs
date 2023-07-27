using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Cards.Tasks.Actions
{
    [Category("★ SlotMaker/Cards")]
    public class VideoPoker_GetMultipliers : ActionTask 
    {
        public BBParameter<long> betCredit;
        public BBParameter<List<long>> saveAs;

        protected override void OnExecute()
        {
            List<long> multipliers = new List<long>();
            List<Blackboard> handMetaInfoList = BlackboardUtils.FindVariable<List<Blackboard>>("./game/handMetaInfoPerBet").value;

            for (int i = 0; i < handMetaInfoList.Count; ++i)
            {
                Blackboard bb = handMetaInfoList[i];
                long betPerHand = bb.GetValue<long>("betPerHand");
                
                if (betPerHand == betCredit.value)
                {
                    List<Blackboard> metaInfoListPerHandList = bb.GetValue<List<Blackboard>>("handMetaInfoPerHand");
                    for (int j = 0; j < metaInfoListPerHandList.Count; ++j)
                    {
                        multipliers.Add(metaInfoListPerHandList[j].GetValue<long>("multiplier"));
                    }
                }
            }
            saveAs.value = multipliers;

            EndAction();
        }
    }
}