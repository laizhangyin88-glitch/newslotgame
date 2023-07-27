using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using NodeCanvas.Framework;

namespace BagelCode.BossRaiders
{
    public class BossRaidersPopupBonusCardController : MonoBehaviour
    {
        private ContextElement rewardTextElement;
        private ContextElement[] rewardIconElements;

        private const int REWARD_TYPE_COUNT = 4;    // 0 : Coin / 1 : Gem / 2 : Energy / 3 : Deal Spin

        public void OnInit()
        {
            ContextElement rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            rewardTextElement = ContextUtils.FindElement(rootElement, "Reward Text", ContextSearchingType.ChildrenSearch);

            rewardIconElements = new ContextElement[REWARD_TYPE_COUNT];
            rewardIconElements[0] = ContextUtils.FindElement(rootElement, "Icon Coin", ContextSearchingType.ChildrenSearch);
            rewardIconElements[1] = ContextUtils.FindElement(rootElement, "Icon Gem", ContextSearchingType.ChildrenSearch);
            rewardIconElements[2] = ContextUtils.FindElement(rootElement, "Icon Energy", ContextSearchingType.ChildrenSearch);
            rewardIconElements[3] = ContextUtils.FindElement(rootElement, "Icon Spin", ContextSearchingType.ChildrenSearch);
        }

        public void SetReward(Blackboard bonusBB)
        {
            if (bonusBB == null) return;

            RewardType rewardType = bonusBB.GetValue<RewardType>("type");
            long rewardAmount = bonusBB.GetValue<long>("amount");

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
                rewardIconElements[i].gameObject.SetActive(i == index);
        }
    }
}