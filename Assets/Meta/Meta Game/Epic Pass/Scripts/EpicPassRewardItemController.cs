using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using ParadoxNotion;
using NodeCanvas.Framework;

namespace BagelCode.EpicPass
{
    public class EpicPassRewardItemController : MonoBehaviour
    {
        private ContextElement rootElement;
        private Animator rootAniamtor;

        private ContextElement rewardCoinIconElement;
        private ContextElement rewardCoinIconCoverElement;

        private ContextElement rewardGemIconElement;
        private ContextElement rewardGemIconCoverElement;

        private ContextElement rewardGiftIconElement;
        private ContextElement rewardGiftIconCoverElement;

        private ContextElement rewardVIPLoungeTicketElement;
        private ContextElement rewardVIPLoungeTicketCoverElement;

        private ContextElement rewardTextElement;

        private ContextElement collectButtonElement;
        private ContextElement collectedIconElement;
        private ContextElement collectEffectElement;

        private ContextElement lockElement;

        private bool isInit = false;

        public Blackboard epicPassRewardBB;
        public int rewardLevel;
        public bool isPaidReward;

        private string soundName;

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

            var rewardElement = ContextUtils.FindElement(rootElement, "Reward Item", ContextSearchingType.ChildrenSearch);

            rewardCoinIconElement = ContextUtils.FindElement(rewardElement, "Item Coin", ContextSearchingType.ChildrenSearch);
            rewardCoinIconCoverElement = ContextUtils.FindElement(rewardCoinIconElement, "Cover", ContextSearchingType.ChildrenSearch);

            rewardGemIconElement = ContextUtils.FindElement(rewardElement, "Item Gem", ContextSearchingType.ChildrenSearch);
            rewardGemIconCoverElement = ContextUtils.FindElement(rewardGemIconElement, "Cover", ContextSearchingType.ChildrenSearch);

            rewardGiftIconElement = ContextUtils.FindElement(rewardElement, "Item Gift", ContextSearchingType.ChildrenSearch);
            rewardGiftIconCoverElement = ContextUtils.FindElement(rewardGiftIconElement, "Cover", ContextSearchingType.ChildrenSearch);

            rewardVIPLoungeTicketElement = ContextUtils.FindElement(rewardElement, "Item VIP Lounge Ticket", ContextSearchingType.ChildrenSearch);
            rewardVIPLoungeTicketCoverElement = ContextUtils.FindElement(rewardVIPLoungeTicketElement, "Cover", ContextSearchingType.ChildrenSearch);

            rewardTextElement = ContextUtils.FindElement(rewardElement, "Text Reward Item", ContextSearchingType.ChildrenSearch);
            collectedIconElement = ContextUtils.FindElement(rewardElement, "Collected", ContextSearchingType.ChildrenSearch);
            collectEffectElement = ContextUtils.FindElement(rewardElement, "Effect Collect", ContextSearchingType.ChildrenSearch);

            lockElement = ContextUtils.FindElement(rewardElement, "Locked", ContextSearchingType.ChildrenSearch);
            collectButtonElement = ContextUtils.FindElement(rewardElement, "Button Collect", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SetClickable(
                collectButtonElement,
                ()=>
                {
                    OnCollectReward();
                }
            );

            isInit = true;
        }

        public void SetValues(Blackboard epicPassRewardBB, int rewardLevel, bool isPaidReward)
        {
            this.epicPassRewardBB = epicPassRewardBB;
            this.rewardLevel = rewardLevel;
            this.isPaidReward = isPaidReward;
        }

        public void UpdateVariables()
        {
            InitProperty();

            var rewardInfoBB = BlackboardUtils.FindVariable<Blackboard>(epicPassRewardBB, "reward");

            if(rewardInfoBB == null || rewardInfoBB.value == null)
            {
                rewardCoinIconElement.gameObject.SetActive(false);
                rewardGemIconElement.gameObject.SetActive(false);
                rewardGiftIconElement.gameObject.SetActive(false);
                rewardVIPLoungeTicketElement.gameObject.SetActive(false);

                collectedIconElement.gameObject.SetActive(false);
                rewardTextElement.gameObject.SetActive(false);
                collectButtonElement.gameObject.SetActive(false);

                if(lockElement != null)
                    lockElement.gameObject.SetActive(false);
            }
            else
            {
                int epicPassLevel = EpicPassUtils.Level;
                bool lockReward = isPaidReward && !EpicPassUtils.Paid;

                bool isClaimed = epicPassRewardBB.GetValue<bool>("isClaimed");
                var rewardType = rewardInfoBB.value.GetValue<RewardType>("type");

                bool levelLock = epicPassLevel < rewardLevel;
                bool isCollectable = !levelLock && !isClaimed && !lockReward;
                bool enableCover = isClaimed || levelLock;

                rewardCoinIconElement.gameObject.SetActive(false);
                rewardGemIconElement.gameObject.SetActive(false);
                rewardGiftIconElement.gameObject.SetActive(false);
                rewardVIPLoungeTicketElement.gameObject.SetActive(false);

                switch (rewardType)
                {
                    case RewardType.CREDIT:
                    case RewardType.CREDIT_WITH_MULTIPLIER:
                        rewardCoinIconElement.gameObject.SetActive(true);
                        MetaContextElementUtils.SetTextGlobal(rewardTextElement, "EPIC_PASS_REWARD_COIN_TEXT", rewardInfoBB.value.GetValue<long>("credit"));
                        rewardTextElement.gameObject.SetActive(true);
                        break;
                    case RewardType.GEM:
                        rewardGemIconElement.gameObject.SetActive(true);
                        MetaContextElementUtils.SetTextGlobal(rewardTextElement, "EPIC_PASS_REWARD_GEM_TEXT", rewardInfoBB.value.GetValue<long>("gem"));
                        rewardTextElement.gameObject.SetActive(true);
                        break;
                    case RewardType.RP:
                    case RewardType.DAILY_BONUS_WHEEL_SPIN:
                    case RewardType.GAME_SPIN:
                    case RewardType.GAME_DEAL:
                    case RewardType.GAME_PLAY:
                    case RewardType.RANDOM:
                    case RewardType.SCRATCHER_FOR_INBOX:
                        rewardGiftIconElement.gameObject.SetActive(true);
                        rewardTextElement.gameObject.SetActive(false);
                        break;
                    case RewardType.VIP_LOUNGE_OPEN_TICKET:
                        rewardVIPLoungeTicketElement.gameObject.SetActive(true);
                        MetaContextElementUtils.SetTextGlobal(rewardTextElement, "EPIC_PASS_REWARD_VIP_LOUNGE_TICKET_TEXT", rewardInfoBB.value.GetValue<int>("openDays"));
                        rewardTextElement.gameObject.SetActive(true);
                        break;
                }

                soundName = MetaCommonRewardUtils.GetRewardSoundName(rewardType);

                collectedIconElement.gameObject.SetActive(isClaimed);

                if(lockElement != null)
                    lockElement.gameObject.SetActive(lockReward);

                collectButtonElement.gameObject.SetActive(isCollectable);

                rewardCoinIconCoverElement.gameObject.SetActive(enableCover);
                rewardGemIconCoverElement.gameObject.SetActive(enableCover);
                rewardGiftIconCoverElement.gameObject.SetActive(enableCover);
                rewardVIPLoungeTicketCoverElement.gameObject.SetActive(enableCover);
            }

            collectEffectElement.gameObject.SetActive(false);
        }

        // Callback. RequestCollectSeasonPassReward.
        public void OnClaimed()
        {
            Refresh();
        }

        private void Refresh()
        {
            UpdateVariables();
        }

        private void OnMetaUIEvent(EventData eventData)
        {
            if(eventData.name == EpicPassUtils.ON_LEVEL_UP_EVENT)
                Refresh();
            else if(eventData.name == EpicPassUtils.ON_REFRESH_EVENT)
                Refresh();
            else if(eventData.name == EpicPassUtils.ON_REWARD_REFRESH)
                Refresh();
        }

        private void OnCollectReward()
        {
            // Play reward sound.
            if(!string.IsNullOrEmpty(soundName))
                GSManager.Instance.GetHandler(soundName).Play();

            // Show Type Effects.
            collectEffectElement.gameObject.SetActive(true);

            MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData<GameObject>("OnCollectReward", gameObject));
        }
    }
}
