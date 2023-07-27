using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BagelCode.ClientModels;
using SlotMaker;
using NodeCanvas.Framework;
using UnityEngine.UI;
using ParadoxNotion;

namespace BagelCode.VegasDreams
{
    public class VegasDreamsGurusEndController : MonoBehaviour
    {
        private ContextElement root;
        private Animator anim;
        private Blackboard bb;

        private bool isInit = false;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        public void InitProperty()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            anim = GetComponent<Animator>();
            bb = GetComponent<Blackboard>();

            root.UpdateContext(false);

            InitContents();

            isInit = true;
        }

        private void InitContents()
        {
            var gurusFinalRewardBB = BlackboardUtils.FindVariable<Blackboard>("/gurusFinalReward")?.value;
            var seasonName = gurusFinalRewardBB.GetValue<string>("seasonName");
            var rank = gurusFinalRewardBB.GetValue<int>("rank");
            var level = gurusFinalRewardBB.GetValue<int>("level");
            var rewardGem = gurusFinalRewardBB.GetValue<long>("rewardGem");

            MetaContextElementUtils.SimpleSetTextGlobal(root, "Text Ranking", "VEGAS_DREAMS_GURUS_END_TITLE", CHILDREN, seasonName);
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Text Rank", "VEGAS_DREAMS_GURUS_END_RANK", CHILDREN, rank);
            MetaContextElementUtils.SimpleSetText(root, "Text Gurus Point", level.ToString(), CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Text Coin", "COMMA_GEM", CHILDREN, rewardGem);

            var collectButtonElement = ContextUtils.FindElement(root, "Button Collect", CHILDREN);
            MetaContextElementUtils.SetClickable(collectButtonElement,
                () => 
                {
                    EventSender.SendEvent(gameObject, VegasDreams.Events.ON_CLOSE);
                });
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Button Collect/Text", "BUTTON_COLLECT", FULL);
        }
    }
}
