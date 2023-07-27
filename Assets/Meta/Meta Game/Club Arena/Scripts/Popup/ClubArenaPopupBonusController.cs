using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Services;

namespace BagelCode.ClubArena
{
    public class ClubArenaPopupBonusController : MonoBehaviour
    {
        private ContextElement rootElement;
        private Blackboard rootBB;
        private Animator rootAnimator;

        private ContextElement titleTextElement;
        private ContextElement rewardElement;
        private ContextElement collectButtonElement;
        private ContextElement rewardTextElement;
        private ContextElement rewardMultiBetElement;
        private ContextElement rewardMultiBetTextElement;

        private ContextElement multiBetElement;
        private ContextElement multiBetTextElement;

        private ContextElement[] rewardIconElements;
        private ContextElement[] effectsCollectElements;

        private ClubArenaPopupBonusCardController[] cardControllers;
        private ContentJackpotCredit jackpotCredit;

        private ClubArenaBonusType rewardType = ClubArenaBonusType.UNKNOWN;
        private long rewardAmount;
        private long rewardResultAmount;
        private long rewardMultiplier;

        private List<Blackboard> bonusInfoList;
        private int bonusIndex = 0;
        private int selectedIndex = 0;

        private bool isMulti = false;

        string rewardTextFormat = "CLUB_ARENA_POPUP_BONUS_REWARD_COIN_TEXT";

        private const int REWARD_TYPE_COUNT = 3;    // 0 : Coin / 1 : Gem / 2 : Energy

        public void OnInit()
        {
            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootBB = gameObject.GetComponent<Blackboard>();
            rootAnimator = gameObject.GetComponent<Animator>();

            ContextElement titleAreaElement = ContextUtils.FindElement(rootElement, "Title Gold Area", ContextSearchingType.ChildrenSearch);
            titleTextElement = ContextUtils.FindElement(titleAreaElement, "Text", ContextSearchingType.ChildrenSearch);
            rewardElement = ContextUtils.FindElement(rootElement, "Reward", ContextSearchingType.ChildrenSearch);
            collectButtonElement = ContextUtils.FindElement(rewardElement, "Button Purchase", ContextSearchingType.ChildrenSearch);
            rewardTextElement = ContextUtils.FindElement(rewardElement, "Reward Text", ContextSearchingType.ChildrenSearch);
            rewardMultiBetElement = ContextUtils.FindElement(rewardElement, "Multi Bet Area", ContextSearchingType.ChildrenSearch);
            rewardMultiBetTextElement = ContextUtils.FindElement(rewardMultiBetElement, "Text", ContextSearchingType.ChildrenSearch);

            multiBetElement = ContextUtils.FindElement(rootElement, "Multi Bet Area", ContextSearchingType.ChildrenSearch);
            multiBetTextElement = ContextUtils.FindElement(multiBetElement, "Text", ContextSearchingType.ChildrenSearch);

            Blackboard rewardBB = ClubArenaUtils.WheelResultInfo;

            MetaContextElementUtils.SetTextGlobal(titleTextElement, "CLUB_ARENA_POPUP_BONUS_TITLE_TEXT");
            MetaContextElementUtils.SimpleSetTextGlobal(collectButtonElement, "Text", "CLUB_ARENA_POPUP_BONUS_BUTTON_TEXT", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SetTextGlobal(rewardMultiBetTextElement, "CLUB_ARENA_POPUP_BONUS_MULTI_BET_TEXT", 1);
            MetaContextElementUtils.SetTextGlobal(multiBetTextElement, "CLUB_ARENA_POPUP_BONUS_MULTI_BET_TEXT", 1);

            MetaContextElementUtils.SetClickable(
                collectButtonElement,
                OnClickCollect
            );

            bonusInfoList = rewardBB.GetValue<List<Blackboard>>("bonusInfoList");
            bonusIndex = rewardBB.GetValue<int>("bonusIndex");

            jackpotCredit = rootElement.GetComponent<ContentJackpotCredit>();

            InitReward();
            GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_BONUS_POPUP).Play();
        }

        private void InitReward()
        {
            rewardIconElements = new ContextElement[REWARD_TYPE_COUNT];
            effectsCollectElements = new ContextElement[REWARD_TYPE_COUNT];
            cardControllers = new ClubArenaPopupBonusCardController[REWARD_TYPE_COUNT];

            rewardIconElements[0] = ContextUtils.FindElement(rewardElement, "Icon Coin", ContextSearchingType.ChildrenSearch);
            rewardIconElements[1] = ContextUtils.FindElement(rewardElement, "Icon Gem", ContextSearchingType.ChildrenSearch);
            rewardIconElements[2] = ContextUtils.FindElement(rewardElement, "Icon Energy", ContextSearchingType.ChildrenSearch);

            effectsCollectElements[0] = ContextUtils.FindElement(rootElement, "Collect Coin", ContextSearchingType.ChildrenSearch);
            effectsCollectElements[1] = ContextUtils.FindElement(rootElement, "Collect Gem", ContextSearchingType.ChildrenSearch);
            effectsCollectElements[2] = ContextUtils.FindElement(rootElement, "Collect Energy", ContextSearchingType.ChildrenSearch);

            for (int i = 0; i < REWARD_TYPE_COUNT; ++i)
            {
                cardControllers[i] = ContextUtils.FindElement(rootElement, string.Format("Card Open {0}", i + 1), ContextSearchingType.ChildrenSearch)?.GetComponent<ClubArenaPopupBonusCardController>();
                cardControllers[i]?.OnInit();
            }
            rewardMultiplier = ClubArenaUtils.CurrentBetMultiplyNumerator;

            if (bonusInfoList != null && bonusInfoList.Count > 0)
            {
                rewardType = bonusInfoList[bonusIndex].GetValue<ClubArenaBonusType>("bonusType");
                rewardAmount = bonusInfoList[bonusIndex].GetValue<long>("amount");
                rewardResultAmount = NumberUtils.GetMultiplierNumeratorValue(rewardAmount, rewardMultiplier);
            }

            switch (rewardType)
            {
                case ClubArenaBonusType.COIN:
                    SetActiveRewardObject(0);
                    rewardTextFormat = "CLUB_ARENA_POPUP_BONUS_REWARD_COIN_TEXT";
                    MetaContextElementUtils.SetTextGlobal(rewardTextElement, rewardTextFormat, rewardAmount);
                    break;
                case ClubArenaBonusType.GEM:
                    SetActiveRewardObject(1);
                    rewardTextFormat = "CLUB_ARENA_POPUP_BONUS_REWARD_GEM_TEXT";
                    MetaContextElementUtils.SetTextGlobal(rewardTextElement, rewardTextFormat, rewardAmount);
                    break;
                case ClubArenaBonusType.ENERGY:
                    SetActiveRewardObject(2);
                    rewardTextFormat = "CLUB_ARENA_POPUP_BONUS_REWARD_ENERGY_TEXT";
                    MetaContextElementUtils.SetTextGlobal(rewardTextElement, rewardTextFormat, rewardAmount);
                    break;
            }

            isMulti = rewardMultiplier != ClubArenaUtils.BaseBetMultiplyNumerator;
            if (!isMulti)
            {
                multiBetElement.gameObject.SetActive(false);
                rewardMultiBetElement.gameObject.SetActive(false);
            }
            else
            {
                double multiplierNumerator = NumberUtils.GetMultiplierFromNumerator(rewardMultiplier);
                MetaContextElementUtils.SetTextGlobal(rewardMultiBetTextElement, "CLUB_ARENA_POPUP_BONUS_MULTI_BET_TEXT", multiplierNumerator);
                MetaContextElementUtils.SetTextGlobal(multiBetTextElement, "CLUB_ARENA_POPUP_BONUS_MULTI_BET_TEXT", multiplierNumerator);
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

        public void SetActiveTrigger()
        {
            if (rootAnimator != null)
                rootAnimator.SetTrigger("Active");
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
            // index : 1 ~ 3
            if (selectedIndex == 0)
                selectedIndex = index;
            else
                return;

            SetRewardCard(index);
            SetAnimator("isMulti", isMulti);
            SetAnimator(string.Format("PickCard{0}", index), true);
            GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_BONUS_PICK_CARD).Play();
        }

        public void OnRewardCard()
        {
            GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_BONUS_PICK_CARD_ZOOM).Play();
        }

        public void OnRewardMultiAction()
        {
            jackpotCredit.Reset(rewardTextElement as IContextText, rewardTextFormat, rewardAmount, rewardResultAmount, 1.0f, 1, false);
            GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_BONUS_MULTI).Play();
        }

        public void OnCollectRewardCard()
        {
            //GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_BONUS_GET_REWARD).Play();
            switch (rewardType)
            {
                case ClubArenaBonusType.COIN:
                    GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_BONUS_COLLECT_COIN).Play();
                    break;
                case ClubArenaBonusType.GEM:
                    GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_BONUS_COLLECT_GEM).Play();
                    break;
                case ClubArenaBonusType.ENERGY:
                    GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_BONUS_COLLECT_ENERGY).Play();
                    break;
            }
        }

        private void OnClickCollect()
        {
            if (isMulti)
                jackpotCredit.Reset(rewardTextElement as IContextText, rewardTextFormat, rewardResultAmount, rewardResultAmount, 0.01f, 1, false);
            SetAnimator("Collect", true);
            OnCollectRewardCard();
            StartCoroutine(SendCallerEvent("OnCloseBonus"));
        }

        private IEnumerator SendCallerEvent(string eventName)
        {
            yield return new WaitForSeconds(1.0f);

            Variable<GameObject> caller = BlackboardUtils.FindVariable<GameObject>(rootBB, "caller");
            if (caller != null && caller.value != null)
            {
                MessageRouter router = Common.GetOrAddMessageRouter(caller.value);
                router.Dispatch(MessageRouter.ON_CUSTOM_EVENT, new EventData(eventName), caller.value);
            }
        }

        private void OnOpenOtherCard()
        {
            // Animation - Bonus Pick A Card 1 ~ 3 Event Call
            GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_BONUS_OTHER_CARD).Play();
        }

        protected void OnPlaySound(string soundKey)
        {
            GSManager.Instance.GetHandler(soundKey).Play();
        }
    }
}
