using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode.BossRaiders
{
    public class BossRaidersWheel : BossRaidersWheelBase
    {
        public override void OnInit(ContextElement root, Animator animator, int wheelCount)
        {
            base.OnInit(root, animator, wheelCount);

            wheelElements = new ContextElement[wheelItemCount];

            ContextElement baseElement = ContextUtils.FindElement(rootElement, "Base", ContextSearchingType.ChildrenSearch);
            for (int i = 0; i < wheelItemCount; ++i)
                wheelElements[i] = ContextUtils.FindElement(baseElement, string.Format("{0:00}", i + 1), ContextSearchingType.ChildrenSearch);

            highlightElement = ContextUtils.FindElement(rootElement, "Highlight", ContextSearchingType.ChildrenSearch);
        }

        public override void SetWheelData(long multi)
        {
            List<Blackboard> WheelCandidateList = BossRaidersUtils.WheelCandidateList;
            if (WheelCandidateList == null || WheelCandidateList.Count != wheelItemCount) return;

            for (int i = 0; i < wheelItemCount; ++i)
            {
                BossRaidersWheelType type = WheelCandidateList[i].GetValue<BossRaidersWheelType>("type");
                if (type == BossRaidersWheelType.BONUS)
                    continue;
                BossRaidersHitType hitType = WheelCandidateList[i].GetValue<BossRaidersHitType>("hitType");
                int attackHP = WheelCandidateList[i].GetValue<int>("baseAttackHp");

                MetaContextElementUtils.SimpleSetTextGlobal(wheelElements[i], "Text", "TEXT_COMMA_NUMBER", ContextSearchingType.ChildrenSearch, NumberUtils.GetMultiplierNumeratorValue(attackHP, multi));
            }
        }
    }
}