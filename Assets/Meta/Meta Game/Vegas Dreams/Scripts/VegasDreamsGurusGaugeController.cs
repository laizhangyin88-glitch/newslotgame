using UnityEngine;
using SlotMaker;
using System.Collections.Generic;
using System;
using BagelCode.ClientModels;
using System.Collections;
using NodeCanvas.Framework;

namespace BagelCode.VegasDreams
{
    public class VegasDreamsGurusGaugeController : EventMonoBehaviour
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
            if (VegasDreams.Utils.GurusBuilding == null)
            {
                MetaContextElementUtils.SimpleSetText(root, "Text Point", "-", CHILDREN);
                MetaContextElementUtils.SimpleSetText(root, "Progress Bar/Text Progress Bar", "-", FULL);
                MetaContextElementUtils.SimpleSetSliderValue(root, "Progress Bar", 0f, CHILDREN);
            }
            else
            {
                var initZero = bb.GetValue<bool>("initZero");

                var level = VegasDreams.Utils.GurusBuilding.GetValue<int>("level");
                var exp = VegasDreams.Utils.GurusBuilding.GetValue<long>("exp");
                var requireExp = VegasDreams.Utils.GurusBuildingPreset.GetValue<long>("exp");

                MetaContextElementUtils.SimpleSetText(root, "Text Point", level.ToString(), CHILDREN);
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
