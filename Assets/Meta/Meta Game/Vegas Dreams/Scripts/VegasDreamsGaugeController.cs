using UnityEngine;
using SlotMaker;
using System.Collections.Generic;
using System;
using BagelCode.ClientModels;
using System.Collections;
using NodeCanvas.Framework;

namespace BagelCode.VegasDreams
{
    public class VegasDreamsGaugeController : EventMonoBehaviour
    {
        private ContextElement root;
        private Animator anim;
        private Blackboard bb;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private List<Animator> depots = new List<Animator>();

        public void InitProperty()
        {
            root = GetComponent<ContextElement>();
            anim = GetComponent<Animator>();
            bb = GetComponent<Blackboard>();

            root.UpdateContext(false);
            OnUpdateGauge();
            OnUpdateGaugeReward();
        }

        public void OnUpdateGauge()
        {
            var index = bb.GetValue<int>("index");
            var level = VegasDreams.Utils.GetBuildingLevel(index);

            for (int i = 1; i <= level; i++)
            {
                var obj = ContextUtils.FindElement(root, $"Star {i}", CHILDREN);
                obj.GetComponent<Animator>().SetTrigger("isAlreadyGet");
            }

            if (level == VegasDreams.Utils.BuildingMaxLevel)
            {
                MetaContextElementUtils.SimpleSetText(root, "Progress Bar/Text Progress Bar", "MAX", FULL);
                MetaContextElementUtils.SimpleSetSliderValue(root, "Progress Bar", 1, CHILDREN);
            }
            else
            {
                var initZero = bb.GetValue<bool>("initZero");
                var exp = VegasDreams.Utils.GetBuildingExp(index);
                var requireExp = VegasDreams.Utils.GetBuildingPresetData(index, level + 1, "exp");

                MetaContextElementUtils.SimpleSetTextGlobal(root, "Progress Bar/Text Progress Bar", "A_PER_B", FULL, exp, requireExp);

                if (initZero)
                {
                    MetaContextElementUtils.SimpleSetSliderValue(root, "Progress Bar", 0, CHILDREN);
                }
                else
                {
                    MetaContextElementUtils.SimpleSetSliderValue(root, "Progress Bar", (float)exp / requireExp, CHILDREN);
                }
            }
        }

        private void OnUpdateGaugeReward()
        {
            var credit = bb.GetValue<long>("earnedCredit");
            anim.SetBool("isReward", credit > 0);

            MetaContextElementUtils.SimpleSetTextGlobal(root, "Reward Base/Text Level Up Reward", "COMMA_STYLE_COIN", FULL, credit);
        }
    }
}
