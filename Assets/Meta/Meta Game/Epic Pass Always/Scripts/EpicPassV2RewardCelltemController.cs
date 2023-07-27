using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;
using NodeCanvas.Framework;

namespace BagelCode.EpicPass
{
    public class EpicPassV2RewardCelltemController : MonoBehaviour
    {
        public bool isPopupReward = false;

        private ContextElement rootElement;

        private ContextElement imageRewardAreaElement;

        private ContextElement rewardInsIconElement;
        private ContextElement rewardBabIconElement;
        private ContextElement rewardSpbIconElement;
        private ContextElement rewardGemIconElement;
        private ContextElement rewardCoinIconElement;
        private ContextElement rewardScratcherIconElement;
        private ContextElement rewardFinderIconElement;
        private ContextElement rewardVIPLoungeTicketElement;
        private ContextElement rewardGiftIconElement;
        private ContextElement rewardDailySpinIconElement;
        private ContextElement rewardWildPuzzleElement;
        private Dictionary<DepotType, ContextElement> rewardDepotElementsDict;

        private ContextElement rewardTextElement;
        private ContextElement imageTagElement;

        private bool isInit = false;

        public Blackboard rewardInfoBB;

        private void InitProperty()
        {
            if (isInit) return;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            imageRewardAreaElement = ContextUtils.FindElement(rootElement, "Image Reward Area", ContextSearchingType.ChildrenSearch);

            rewardInsIconElement = ContextUtils.FindElement(imageRewardAreaElement, "Image Reward Instant Bonus", ContextSearchingType.ChildrenSearch);
            rewardBabIconElement = ContextUtils.FindElement(imageRewardAreaElement, "Image Reward Buy A Bonus", ContextSearchingType.ChildrenSearch);
            rewardSpbIconElement = ContextUtils.FindElement(imageRewardAreaElement, "Image Reward Super Bonus", ContextSearchingType.ChildrenSearch);
            rewardGemIconElement = ContextUtils.FindElement(imageRewardAreaElement, "Image Reward Gem", ContextSearchingType.ChildrenSearch);
            rewardCoinIconElement = ContextUtils.FindElement(imageRewardAreaElement, "Image Reward Coin", ContextSearchingType.ChildrenSearch);
            rewardScratcherIconElement = ContextUtils.FindElement(imageRewardAreaElement, "Image Reward Scratcher", ContextSearchingType.ChildrenSearch);
            rewardFinderIconElement = ContextUtils.FindElement(imageRewardAreaElement, "Image Reward Finder", ContextSearchingType.ChildrenSearch);
            rewardVIPLoungeTicketElement = ContextUtils.FindElement(imageRewardAreaElement, "Image Reward VIP Lounge Ticket", ContextSearchingType.ChildrenSearch);
            rewardGiftIconElement = ContextUtils.FindElement(imageRewardAreaElement, "Image Reward Mystery Gift", ContextSearchingType.ChildrenSearch);
            rewardDailySpinIconElement = ContextUtils.FindElement(imageRewardAreaElement, "Image Reward Daily Spin", ContextSearchingType.ChildrenSearch);
            rewardWildPuzzleElement = ContextUtils.FindElement(imageRewardAreaElement, "Image Reward Wild Puzzle", ContextSearchingType.ChildrenSearch);

            rewardDepotElementsDict = new Dictionary<DepotType, ContextElement>();
            foreach(DepotType type in System.Enum.GetValues(typeof(DepotType)))
            {
                if (type == DepotType.UNKNOWN)
                    continue;
                rewardDepotElementsDict.Add(type, ContextUtils.FindElement(imageRewardAreaElement, GetDepotText(type), ContextSearchingType.ChildrenSearch));
            }

            rewardTextElement = ContextUtils.FindElement(rootElement, "Text Reward Item", ContextSearchingType.ChildrenSearch);
            imageTagElement = ContextUtils.FindElement(rootElement, "Image Tag", ContextSearchingType.ChildrenSearch);

            isInit = true;
        }

        public void SetValues(Blackboard rewardInfoBB)
        {
            this.rewardInfoBB = rewardInfoBB;
        }

        public void UpdateVariables()
        {
            InitProperty();

            DisableRewardIcons();

            if (rewardInfoBB == null)
            {
                rewardTextElement.gameObject.SetActive(false);
            }
            else
            {
                Blackboard rewardResultInfoBB = rewardInfoBB;
                var rewardType = BlackboardUtils.FindVariable<RewardType>(rewardInfoBB, "type");
                if (rewardType == null)
                {
                    rewardType = BlackboardUtils.FindVariable<RewardType>(rewardInfoBB, "rewardType");
                    rewardResultInfoBB = BlackboardUtils.FindValue<Blackboard>(rewardInfoBB, "rewardInfo");
                }
                bool isInbox = BlackboardUtils.FindVariable<bool>(rewardResultInfoBB, "inboxInfo")?.value ?? false;
                switch (rewardType.value)
                {
                    case RewardType.CREDIT:
                    case RewardType.CREDIT_WITH_MULTIPLIER:
                        rewardCoinIconElement.gameObject.SetActive(true);
                        MetaContextElementUtils.SetTextGlobal(rewardTextElement, "EPIC_PASS_REWARD_COIN_TEXT", rewardResultInfoBB.GetValue<long>("credit"));
                        rewardTextElement.gameObject.SetActive(true);
                        break;
                    case RewardType.GEM:
                        rewardGemIconElement.gameObject.SetActive(true);
                        MetaContextElementUtils.SetTextGlobal(rewardTextElement, "EPIC_PASS_REWARD_GEM_TEXT", rewardResultInfoBB.GetValue<long>("gem"));
                        rewardTextElement.gameObject.SetActive(true);
                        break;
                    case RewardType.DAILY_BONUS_WHEEL_SPIN:
                        rewardDailySpinIconElement.gameObject.SetActive(true);
                        MetaContextElementUtils.SetTextGlobal(rewardTextElement, "EPIC_PASS_REWARD_DAILY_SPIN_TEXT", rewardResultInfoBB.GetValue<int>("spinCount"));
                        rewardTextElement.gameObject.SetActive(true);
                        break;
                    case RewardType.RP:
                    case RewardType.GAME_SPIN:
                    case RewardType.GAME_DEAL:
                    case RewardType.GAME_PLAY:
                    case RewardType.RANDOM:
                        rewardGiftIconElement.gameObject.SetActive(true);
                        rewardTextElement.gameObject.SetActive(false);
                        break;
                    case RewardType.SCRATCHER_FOR_INBOX:
                        rewardScratcherIconElement.gameObject.SetActive(true);
                        MetaContextElementUtils.SetTextGlobal(rewardTextElement, "EPIC_PASS_REWARD_SCRATCHER_TEXT", rewardResultInfoBB.GetValue<int>("count"));
                        rewardTextElement.gameObject.SetActive(true);
                        isInbox = true;
                        break;
                    case RewardType.VIP_LOUNGE_OPEN_TICKET:
                        rewardVIPLoungeTicketElement.gameObject.SetActive(true);
                        MetaContextElementUtils.SetTextGlobal(rewardTextElement, "EPIC_PASS_REWARD_VIP_LOUNGE_TICKET_TEXT", rewardResultInfoBB.GetValue<int>("openDays"));
                        rewardTextElement.gameObject.SetActive(true);
                        break;
                    case RewardType.TICKETED_BONUS_TICKET:
                        BonusTag tag = rewardResultInfoBB.GetValue<BonusTag>("tag");
                        switch (tag)
                        {
                            case BonusTag.INSTANT_BONUS:
                                rewardInsIconElement.gameObject.SetActive(true);
                                break;
                            case BonusTag.BUY_A_BONUS:
                                rewardBabIconElement.gameObject.SetActive(true);
                                break;
                            case BonusTag.SUPER_BONUS:
                                rewardSpbIconElement.gameObject.SetActive(true);
                                break;
                        }
                        if (isPopupReward)
                        {
                            int count = BlackboardUtils.FindVariable<int>(rewardResultInfoBB, "ticketCount")?.value ?? 1;
                            MetaContextElementUtils.SetTextGlobal(rewardTextElement, "EPIC_PASS_REWARD_POPUP_TICKETED_BONUS_TICKET_TEXT", count);
                            rewardTextElement.gameObject.SetActive(true);
                        }
                        else
                        {
                            long totalBet = rewardResultInfoBB.GetValue<long>("baseBet") + rewardResultInfoBB.GetValue<long>("extraBet");
                            MetaContextElementUtils.SetTextGlobal(rewardTextElement, "EPIC_PASS_REWARD_TICKETED_BONUS_TICKET_TEXT", totalBet);
                            rewardTextElement.gameObject.SetActive(true);
                        }
                        break;
                    case RewardType.HIDDEN_UNIVERSE_FINDER:
                        rewardFinderIconElement.gameObject.SetActive(true);
                        MetaContextElementUtils.SetTextGlobal(rewardTextElement, "EPIC_PASS_REWARD_FINDER_TEXT", rewardResultInfoBB.GetValue<int>("finder"));
                        rewardTextElement.gameObject.SetActive(true);
                        break;
                    case RewardType.DEPOT:
                        DepotType type = rewardResultInfoBB.GetValue<DepotType>("depotType");
                        if (rewardDepotElementsDict.ContainsKey(type))
                            rewardDepotElementsDict[type]?.gameObject.SetActive(true);
                        MetaContextElementUtils.SetTextGlobal(rewardTextElement, "EPIC_PASS_REWARD_DEPOT_TEXT", rewardResultInfoBB.GetValue<int>("count"));
                        break;
                    case RewardType.WILD_PUZZLE:
                        rewardWildPuzzleElement.gameObject.SetActive(true);
                        MetaContextElementUtils.SetTextGlobal(rewardTextElement, "EPIC_PASS_REWARD_WILD_PUZZLE_TEXT", rewardResultInfoBB.GetValue<int>("count"));
                        rewardTextElement.gameObject.SetActive(true);
                        break;
                    default:
                        rewardTextElement.gameObject.SetActive(false);
                        break;
                }

                if (imageTagElement != null)
                    imageTagElement.gameObject.SetActive(isInbox);
            }
        }

        private void DisableRewardIcons()
        {
            rewardInsIconElement.gameObject.SetActive(false);
            rewardBabIconElement.gameObject.SetActive(false);
            rewardSpbIconElement.gameObject.SetActive(false);
            rewardGemIconElement.gameObject.SetActive(false);
            rewardCoinIconElement.gameObject.SetActive(false);
            rewardScratcherIconElement.gameObject.SetActive(false);
            rewardFinderIconElement.gameObject.SetActive(false);
            rewardVIPLoungeTicketElement.gameObject.SetActive(false);
            rewardGiftIconElement.gameObject.SetActive(false);
            rewardDailySpinIconElement.gameObject.SetActive(false);
            rewardWildPuzzleElement.gameObject.SetActive(false);
            
            foreach (KeyValuePair<DepotType, ContextElement> item in rewardDepotElementsDict)
                item.Value?.gameObject.SetActive(false);

            if (imageTagElement != null)
                imageTagElement.gameObject.SetActive(false);
        }

        public string GetRewardText()
        {
            if (rewardTextElement != null)
                return MetaContextElementUtils.GetText(rewardTextElement);
            return "";
        }

        private string GetDepotText(DepotType type)
        {
            return "Image Reward Depot " + TextDecoUtils.EnumTypeToText<DepotType>(
                (int)type, TextDecoUtils.TextFormat.PASCAL_CASE, " ");
        }
    }
}
