using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using NodeCanvas.Framework;

namespace BagelCode.BossRaiders
{
    public class BossRaidersPopupBonusController : MonoBehaviour
    {
        private ContextElement rootElement;
        private Animator rootAnimator;

        private ContextElement rewardElement;
        private ContextElement collectButtonElement;
        private ContextElement rewardTextElement;

        private ContextElement[] rewardIconElements;
        private ContextElement[] effectsCollectElements;

        private BossRaidersPopupBonusCardController[] cardControllers;

        private RewardType rewardType = RewardType.BOSS_RAIDERS_ENERGY;
        private long rewardAmount;

        private List<Blackboard> bonusInfoList;
        private int bonusIndex = 0;
        private int selectedIndex = 0;

        private bool isMeta = true;
        private bool isClicked = false;
        private bool isAuto = false;

        private const int REWARD_TYPE_COUNT = 4;    // 0 : Coin / 1 : Gem / 2 : Energy / 3 : Deal Spin
        private const int CARD_COUNT = 3;

        public void OnInit(bool _isMeta)
        {
            isMeta = _isMeta;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootAnimator = gameObject.GetComponent<Animator>();

            rewardElement = ContextUtils.FindElement(rootElement, "Reward", ContextSearchingType.ChildrenSearch);
            collectButtonElement = ContextUtils.FindElement(rewardElement, "Button Purchase", ContextSearchingType.ChildrenSearch);
            rewardTextElement = ContextUtils.FindElement(rewardElement, "Reward Text", ContextSearchingType.ChildrenSearch);

            Blackboard rewardBB = null;
            if (isMeta)
                rewardBB = BossRaidersUtils.WheelResultInfo;
            else
            {
                Blackboard dealBB = BlackboardUtils.GetOrCreateVariable<Blackboard>(MainBlackboard.Get(), BossRaidersUtils.BOSS_RAIDERS_DEAL_INFO).value;
                rewardBB = dealBB.GetValue<Blackboard>("wheelResultInfo");
                StartCoroutine(AutoSelect());
                StartCoroutine(AutoCollect());
            }

            MetaContextElementUtils.SimpleSetTextGlobal(collectButtonElement, "Text", "BOSS_RAIDERS_POPUP_BONUS_BUTTON_TEXT", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SetClickable(
                collectButtonElement,
                "OnClickCollect",
                rootElement,
                null
            );

            bonusInfoList = rewardBB.GetValue<List<Blackboard>>("bonusInfoList");
            bonusIndex = rewardBB.GetValue<int>("bonusIndex");

            InitReward();
            GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_BONUS).Play();
        }

        private void InitReward()
        {
            rewardIconElements = new ContextElement[REWARD_TYPE_COUNT];
            effectsCollectElements = new ContextElement[REWARD_TYPE_COUNT];
            cardControllers = new BossRaidersPopupBonusCardController[REWARD_TYPE_COUNT];

            rewardIconElements[0] = ContextUtils.FindElement(rewardElement, "Icon Coin", ContextSearchingType.ChildrenSearch);
            rewardIconElements[1] = ContextUtils.FindElement(rewardElement, "Icon Gem", ContextSearchingType.ChildrenSearch);
            rewardIconElements[2] = ContextUtils.FindElement(rewardElement, "Icon Energy", ContextSearchingType.ChildrenSearch);
            rewardIconElements[3] = ContextUtils.FindElement(rewardElement, "Icon Spin", ContextSearchingType.ChildrenSearch);

            effectsCollectElements[0] = ContextUtils.FindElement(rootElement, "Collect Coin", ContextSearchingType.ChildrenSearch);
            effectsCollectElements[1] = ContextUtils.FindElement(rootElement, "Collect Gem", ContextSearchingType.ChildrenSearch);
            effectsCollectElements[2] = ContextUtils.FindElement(rootElement, "Collect Energy", ContextSearchingType.ChildrenSearch);
            effectsCollectElements[3] = ContextUtils.FindElement(rootElement, "Collect Spin", ContextSearchingType.ChildrenSearch);

            for (int i = 0; i < CARD_COUNT; ++i)
            {
                cardControllers[i] = ContextUtils.FindElement(rootElement, string.Format("Card Open {0}", i + 1), ContextSearchingType.ChildrenSearch)?.GetComponent<BossRaidersPopupBonusCardController>();
                cardControllers[i]?.OnInit();
            }

            if (bonusInfoList != null && bonusInfoList.Count > 0)
            {
                rewardType = bonusInfoList[bonusIndex].GetValue<RewardType>("type");
                rewardAmount = bonusInfoList[bonusIndex].GetValue<long>("amount");
            }

            switch (rewardType)
            {
                case RewardType.CREDIT:
                    SetActiveRewardObject(0);
                    MetaContextElementUtils.SetTextGlobal(rewardTextElement, "BOSS_RAIDERS_POPUP_BONUS_REWARD_COIN_TEXT", rewardAmount);
                    break;
                case RewardType.GEM:
                    SetActiveRewardObject(1);
                    MetaContextElementUtils.SetTextGlobal(rewardTextElement, "BOSS_RAIDERS_POPUP_BONUS_REWARD_GEM_TEXT", rewardAmount);
                    break;
                case RewardType.BOSS_RAIDERS_ENERGY:
                    SetActiveRewardObject(2);
                    MetaContextElementUtils.SetTextGlobal(rewardTextElement, "BOSS_RAIDERS_POPUP_BONUS_REWARD_ENERGY_TEXT", rewardAmount);
                    break;
                case RewardType.BOSS_RAIDERS_DEAL_SPIN:
                    SetActiveRewardObject(3);
                    MetaContextElementUtils.SetTextGlobal(rewardTextElement, "BOSS_RAIDERS_POPUP_BONUS_REWARD_SPIN_TEXT", (int)rewardAmount);
                    break;
            }
        }

        private void SetActiveRewardObject(int index)
        {
            for (int i = 0; i < REWARD_TYPE_COUNT; ++i)
            {
                bool isActive = i == index;
                rewardIconElements[i].gameObject.SetActive(isActive);
                effectsCollectElements[i].gameObject.SetActive(isActive);
            }
        }

        private void SetAnimator(string key, bool isActive)
        {
            if (rootAnimator != null)
                rootAnimator.SetBool(key, isActive);
        }

        private void SetRewardCard(int index)
        {
            if (bonusInfoList == null) return;

            List<Blackboard> tempList = new List<Blackboard>(bonusInfoList);
            Blackboard resultBB = tempList[bonusIndex];
            tempList.RemoveAt(bonusIndex);
            tempList.Insert(index - 1, resultBB);
            for (int i = 0; i < tempList.Count; ++i)
                cardControllers[i]?.SetReward(tempList[i]);
        }

        public void OnClickCard(int index)
        {
            isClicked = true;
            // index : 1 ~ 3
            if (selectedIndex == 0)
                selectedIndex = index;
            else
                return;
            SetRewardCard(index);
            SetAnimator(string.Format("PickCard{0}", index), true);
            GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_BONUS_CARD).Play();
        }

        public void OnRewardCard()
        {
            GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_BONUS_REWARD_CARD).Play();
        }

        public void OnCollectRewardCard()
        {
            switch (rewardType)
            {
                case RewardType.CREDIT:
                    GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_BONUS_COLLECT_COIN).Play();
                    break;
                case RewardType.GEM:
                    GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_BONUS_COLLECT_GEM).Play();
                    break;
                case RewardType.BOSS_RAIDERS_ENERGY:
                    GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_BONUS_COLLECT_ENERGY).Play();
                    break;
                case RewardType.BOSS_RAIDERS_DEAL_SPIN:
                    GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_BONUS_COLLECT_ENERGY).Play();
                    break;
            }
        }

        private IEnumerator AutoSelect()
        {
            float timer = 0f;
            while (timer < 3f)
            {
                if (isClicked) yield break;
                timer += Time.deltaTime;
                yield return null;
            }

            isAuto = true;
            OnClickCard(1);
        }

        private IEnumerator AutoCollect()
        {
            yield return new WaitUntil(() => rootAnimator.GetCurrentAnimatorStateInfo(0).IsName("Reward Idle"));
            
            float timer = 0f;
            float delay = isAuto ? 0.5f : 3f;

            while (timer < delay)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            EventSender.SendEvent(gameObject, "OnClickCollect");
        }
    }
}