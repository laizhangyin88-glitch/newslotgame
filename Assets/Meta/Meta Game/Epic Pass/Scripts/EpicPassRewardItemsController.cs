using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode;
using ParadoxNotion;
using NodeCanvas.Framework;

namespace BagelCode.EpicPass
{
    public class EpicPassRewardItemsController : MonoBehaviour
    {
        public int rewardLevel;

        private ContextElement rootElement;
        private Animator rootAniamtor;

        private ContextElement levelTextElement;
        private ContextElement currentLevelCoverElement;

        private EpicPassRewardItemController freeItemController;
        private EpicPassRewardItemController paidItemController;

        private bool isInit = false;

        public void OnEnable()
        {
            MessageDispatcher.Register(MetaEventDefine.ON_META_UI_EVENT, OnMetaUIEvent);
        }

        public void OnDisable()
        {
            MessageDispatcher.UnRegister(MetaEventDefine.ON_META_UI_EVENT, OnMetaUIEvent);
        }

        private void InitProperty()
        {
            if(isInit) return;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootAniamtor = gameObject.GetComponent<Animator>();

            levelTextElement = ContextUtils.FindElement(rootElement, "Text Rewards Level", ContextSearchingType.ChildrenSearch);

            var freeItemElement = ContextUtils.FindElement(rootElement, "Free Pass Rewards", ContextSearchingType.ChildrenSearch);
            freeItemController = freeItemElement.gameObject.GetComponent<EpicPassRewardItemController>();

            var paidItemElement = ContextUtils.FindElement(rootElement, "Epic Pass Rewards", ContextSearchingType.ChildrenSearch);
            paidItemController = paidItemElement.gameObject.GetComponent<EpicPassRewardItemController>();

            currentLevelCoverElement = ContextUtils.FindElement(rootElement, "Rewards Level Cover", ContextSearchingType.ChildrenSearch);

            isInit = true;
        }

        public void UpdateVariables(Blackboard rewardsBB, int index)
        {
            InitProperty();

            rewardLevel = index + 1;
            MetaContextElementUtils.SetTextGlobal(levelTextElement, "TEXT_COMMA_NUMBER", rewardLevel);

            var freeReward = BlackboardUtils.FindVariable<Blackboard>(rewardsBB, "freeReward");
            var paidReward = BlackboardUtils.FindVariable<Blackboard>(rewardsBB, "paidReward");

            if(freeReward != null)
            {
                freeItemController.SetValues(freeReward.value, rewardLevel, false);
                freeItemController.UpdateVariables();
            }

            if(paidReward != null)
            {
                paidItemController.SetValues(paidReward.value, rewardLevel, true);
                paidItemController.UpdateVariables();
            }

            UpdateLevelCover();
        }

        private void UpdateLevelCover()
        {
            currentLevelCoverElement.gameObject.SetActive(rewardLevel == EpicPassUtils.Level);
        }

        private void OnMetaUIEvent(EventData eventData)
        {
            if(eventData.name == EpicPassUtils.ON_LEVEL_UP_EVENT)
            {
                UpdateLevelCover();
            }
        }
    }
}
