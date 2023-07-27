using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode;
using ParadoxNotion;
using NodeCanvas.Framework;

namespace BagelCode.EpicPass
{
    public class EpicPassV2RewardItemsController : MonoBehaviour
    {
        private int rewardLevel;
        public int RewardLevel { get { return rewardLevel; } }

        private ContextElement rootElement;
        private Animator rootAniamtor;

        private ContextElement levelInactiveElement;
        private ContextElement levelActiveElement;
        private ContextElement currentLevelCoverPaidElement;

        private EpicPassV2RewardItemController freeItemController;
        private EpicPassV2RewardItemController paidItemController;

        private bool isClaimedPaid = false;

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
            if (isInit) return;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootAniamtor = gameObject.GetComponent<Animator>();

            ContextElement gaugeLevelElement = ContextUtils.FindElement(rootElement, "Gauge Level", ContextSearchingType.ChildrenSearch);
            levelInactiveElement = ContextUtils.FindElement(gaugeLevelElement, "Inactive", ContextSearchingType.ChildrenSearch);
            levelActiveElement = ContextUtils.FindElement(gaugeLevelElement, "Active", ContextSearchingType.ChildrenSearch);

            var freeItemElement = ContextUtils.FindElement(rootElement, "Free Pass Rewards", ContextSearchingType.ChildrenSearch);
            freeItemController = freeItemElement.gameObject.GetComponent<EpicPassV2RewardItemController>();

            var paidItemElement = ContextUtils.FindElement(rootElement, "Epic Pass Rewards", ContextSearchingType.ChildrenSearch);
            currentLevelCoverPaidElement = ContextUtils.FindElement(paidItemElement, "Select Outline", ContextSearchingType.ChildrenSearch);
            paidItemController = paidItemElement.gameObject.GetComponent<EpicPassV2RewardItemController>();

            isInit = true;
        }

        public void UpdateVariables(Blackboard rewardsBB, int index)
        {
            InitProperty();

            rewardLevel = index + 1;

            MetaContextElementUtils.SimpleSetTextGlobal(levelInactiveElement, "Text Level", "TEXT_COMMA_NUMBER", ContextSearchingType.ChildrenSearch, rewardLevel);
            MetaContextElementUtils.SimpleSetTextGlobal(levelActiveElement, "Text Level", "TEXT_COMMA_NUMBER", ContextSearchingType.ChildrenSearch, rewardLevel);

            var freeReward = BlackboardUtils.FindVariable<Blackboard>(rewardsBB, "free");
            var paidReward = BlackboardUtils.FindVariable<Blackboard>(rewardsBB, "paid");

            if (freeReward != null)
            {
                freeItemController.SetValues(freeReward.value, rewardLevel, false);
                freeItemController.SetCallback(ClickFreeReward);
                freeItemController.UpdateVariables();
            }

            if (paidReward != null)
            {
                isClaimedPaid = BlackboardUtils.FindVariable<bool>(paidReward.value, "isClaimed")?.value ?? false;
                paidItemController.SetValues(paidReward.value, rewardLevel, true);
                paidItemController.SetCallback(ClickPaidReward);
                paidItemController.UpdateVariables();
            }

            UpdateActiveLevel();
            UpdateLevelCover();
        }

        private void UpdateActiveLevel()
        {
            bool isAvailable = rewardLevel <= EpicPassUtilsV2.Level;
            levelActiveElement.gameObject.SetActive(isAvailable);
            levelInactiveElement.gameObject.SetActive(!isAvailable);
        }

        private void OnMetaUIEvent(EventData eventData)
        {
            if (eventData.name == EpicPassUtilsV2.ON_LEVEL_UP_EVENT)
            {
                UpdateActiveLevel();
                UpdateLevelCover();
            }
        }

        private void UpdateLevelCover()
        {
            bool isAvailable = rewardLevel <= EpicPassUtilsV2.Level;
            currentLevelCoverPaidElement.gameObject.SetActive(isAvailable && !isClaimedPaid);
        }

        private void ClickFreeReward()
        {
            TriggerCollectReward("FreePassCollect");
        }

        private void ClickPaidReward()
        {
            TriggerCollectReward("EpicPassCollect");
        }

        private void TriggerCollectReward(string triggerName)
        {
            rootAniamtor?.SetTrigger(triggerName);
        }
    }
}