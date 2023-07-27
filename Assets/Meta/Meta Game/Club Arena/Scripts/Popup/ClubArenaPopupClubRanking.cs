using System.Collections.Generic;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using ParadoxNotion.Services;
using ParadoxNotion;

namespace BagelCode.ClubArena
{
    public class ClubArenaPopupClubRanking : MonoBehaviour
    {
        private ContextElement rootElement;
        private Blackboard rootBB;
        private Animator rootAnimator;

        private ContextElement titleTextElement;
        private ContextElement titleRankTextElement;
        private ContextElement rewardGroupElement;

        private ContextElement[] topRankRewardTextElements;
        private ContextElement[] nextRankTextElements;
        private ContextElement[] nextRankRewardTextElements;
        private ContextElement[] percentileTextElements;
        private ContextElement[] percentileGemElements;
        // Background light element
        private ContextElement[] topRankBackgroundElements;
        private ContextElement[] nextRankBackgroundElements;
        private ContextElement[] percentileBackgroundElements;

        private readonly int TOP_RANK_COUNT = 3;
        private readonly int NEXT_RANK_COUNT = 4;
        private readonly int PERCENTILE_COUNT = 2;
        private readonly int PERCENTILE_CELL_COUNT = 10;

        public void OnInit()
        {
            InitProperty();
            InitTexts();
            UpdateClubRankListData();
        }

        private void InitProperty()
        {
            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootBB = gameObject.GetComponent<Blackboard>();
            rootAnimator = gameObject.GetComponent<Animator>();

            ContextElement titleAreaElement = ContextUtils.FindElement(rootElement, "Title Area", ContextSearchingType.ChildrenSearch);
            titleTextElement = ContextUtils.FindElement(titleAreaElement, "Title Text", ContextSearchingType.ChildrenSearch);
            ContextElement currentClubElement = ContextUtils.FindElement(titleAreaElement, "Current Club Ranking", ContextSearchingType.ChildrenSearch);
            titleRankTextElement = ContextUtils.FindElement(currentClubElement, "Title Text", ContextSearchingType.ChildrenSearch);
            rewardGroupElement = ContextUtils.FindElement(rootElement, "Reward Group", ContextSearchingType.ChildrenSearch);

            ContextElement[] rankRootElements = new ContextElement[TOP_RANK_COUNT];
            rankRootElements[0] = ContextUtils.FindElement(rootElement, "Reward 1st", ContextSearchingType.ChildrenSearch);
            rankRootElements[1] = ContextUtils.FindElement(rootElement, "Reward 2nd", ContextSearchingType.ChildrenSearch);
            rankRootElements[2] = ContextUtils.FindElement(rootElement, "Reward 3rd", ContextSearchingType.ChildrenSearch);

            ContextElement rankLightElement = ContextUtils.FindElement(rootElement, "My Rank Position Light", ContextSearchingType.ChildrenSearch);

            topRankRewardTextElements = new ContextElement[TOP_RANK_COUNT];
            topRankBackgroundElements = new ContextElement[TOP_RANK_COUNT];
            for (int i = 0; i < TOP_RANK_COUNT; ++i)
            {
                topRankRewardTextElements[i] = ContextUtils.FindElement(rankRootElements[i], "Text Reward", ContextSearchingType.ChildrenSearch);
                topRankBackgroundElements[i] = ContextUtils.FindElement(rankLightElement, string.Format("Top Rank {0}", i + 1), ContextSearchingType.ChildrenSearch);
                topRankBackgroundElements[i].gameObject.SetActive(false);
            }

            nextRankTextElements = new ContextElement[NEXT_RANK_COUNT];
            nextRankRewardTextElements = new ContextElement[NEXT_RANK_COUNT];
            ContextElement[] rankNextRootElements = new ContextElement[NEXT_RANK_COUNT];
            nextRankBackgroundElements = new ContextElement[NEXT_RANK_COUNT];
            for (int i = 0; i < NEXT_RANK_COUNT; ++i)
            {
                rankNextRootElements[i] = ContextUtils.FindElement(rewardGroupElement, string.Format("Rank Cell {0}", i + 1), ContextSearchingType.ChildrenSearch);
                nextRankTextElements[i] = ContextUtils.FindElement(rankNextRootElements[i], "Text Rank", ContextSearchingType.ChildrenSearch);
                nextRankRewardTextElements[i] = ContextUtils.FindElement(rankNextRootElements[i], "Text Reward", ContextSearchingType.ChildrenSearch);
                nextRankBackgroundElements[i] = ContextUtils.FindElement(rankLightElement, string.Format("Rank Cell {0}", i + 1), ContextSearchingType.ChildrenSearch);
                nextRankBackgroundElements[i].gameObject.SetActive(false);
            }

            percentileTextElements = new ContextElement[PERCENTILE_COUNT];
            percentileGemElements = new ContextElement[PERCENTILE_COUNT];

            for (int i = 0; i < PERCENTILE_COUNT; ++i)
            {
                percentileTextElements[i] = ContextUtils.FindElement(rewardGroupElement, string.Format("Text Top Percentile Text {0}", i + 1), ContextSearchingType.ChildrenSearch);
                percentileGemElements[i] = ContextUtils.FindElement(rewardGroupElement, string.Format("Text Top Percentile Gem {0}", i + 1), ContextSearchingType.ChildrenSearch);
            }

            percentileBackgroundElements = new ContextElement[PERCENTILE_CELL_COUNT];
            for (int i = 0; i < PERCENTILE_CELL_COUNT; ++i)
            {
                percentileBackgroundElements[i] = ContextUtils.FindElement(rankLightElement, string.Format("Per Cell {0:00}", i + 1), ContextSearchingType.ChildrenSearch);
                percentileBackgroundElements[i].gameObject.SetActive(false);
            }

            ContextElement buttonCloseElement = ContextUtils.FindElement(rootElement, "Button Close", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SetClickable(
                buttonCloseElement,
                OnClickClose
            );

            MetaSystem.SubscribeBackButton(this.GetHashCode(), OnClickClose);
        }

        private void InitTexts()
        {
            Dictionary<int, long> dicRewardRank = ClubArenaUtils.FinalRewardRank;
            Dictionary<int, long> dicRewardPercent = ClubArenaUtils.FinalRewardPercentile;
            List<int> rewardRankKeys = new List<int>(dicRewardRank.Keys);
            List<int> rewardPercentKeys = new List<int>(dicRewardPercent.Keys);

            // Title
            MetaContextElementUtils.SetTextGlobal(titleTextElement, "CLUB_ARENA_POPUP_CLUB_RANK_TITLE");
            MetaContextElementUtils.SetTextGlobal(titleRankTextElement, "CLUB_ARENA_POPUP_CLUB_RANK", ClubArenaUtils.GetClubRank(), ClubArenaUtils.ClubRank, ClubArenaUtils.GetClubPercentile());

            // Rank
            if (rewardRankKeys != null && rewardRankKeys.Count > 0)
            {
                int lastRewardRankKey = 0;
                for (int i = 0; i < TOP_RANK_COUNT; ++i)
                    MetaContextElementUtils.SetTextGlobal(topRankRewardTextElements[i], "CLUB_ARENA_POPUP_CLUB_RANK_REWARD_TEXT", dicRewardRank[rewardRankKeys[i]]);
                lastRewardRankKey = rewardRankKeys[TOP_RANK_COUNT - 1];
                for (int i = 0; i < NEXT_RANK_COUNT; ++i)
                {
                    int idx = i + TOP_RANK_COUNT;
                    MetaContextElementUtils.SetTextGlobal(nextRankRewardTextElements[i], "CLUB_ARENA_POPUP_CLUB_RANK_REWARD_TEXT", dicRewardRank[rewardRankKeys[idx]]);
                    MetaContextElementUtils.SetTextGlobal(nextRankTextElements[i], "CLUB_ARENA_POPUP_CLUB_RANK_TEXT", lastRewardRankKey + 1, rewardRankKeys[idx]);
                    lastRewardRankKey = rewardRankKeys[idx];
                }
            }

            // Percentile
            if (rewardPercentKeys != null && rewardPercentKeys.Count > 0)
            {
                int firstRank = 0;
                MetaContextElementUtils.SetTextGlobal(percentileTextElements[0], "CLUB_ARENA_POPUP_CLUB_RANK_PERCENTILE_TEXT_0",
                    firstRank + 1, rewardPercentKeys[0],
                    rewardPercentKeys[0] + 1, rewardPercentKeys[1],
                    rewardPercentKeys[1] + 1, rewardPercentKeys[2],
                    rewardPercentKeys[2] + 1, rewardPercentKeys[3],
                    rewardPercentKeys[3] + 1, rewardPercentKeys[4]);
                MetaContextElementUtils.SetTextGlobal(percentileTextElements[1], "CLUB_ARENA_POPUP_CLUB_RANK_PERCENTILE_TEXT_1",
                    rewardPercentKeys[4] + 1, rewardPercentKeys[5],
                    rewardPercentKeys[5] + 1, rewardPercentKeys[6],
                    rewardPercentKeys[6] + 1, rewardPercentKeys[7],
                    rewardPercentKeys[7] + 1, rewardPercentKeys[8],
                    rewardPercentKeys[8] + 1, rewardPercentKeys[9]);

                MetaContextElementUtils.SetTextGlobal(percentileGemElements[0], "CLUB_ARENA_POPUP_CLUB_RANK_PERCENTILE_REWARD_TEXT_0",
                    dicRewardPercent[rewardPercentKeys[0]],
                    dicRewardPercent[rewardPercentKeys[1]],
                    dicRewardPercent[rewardPercentKeys[2]],
                    dicRewardPercent[rewardPercentKeys[3]],
                    dicRewardPercent[rewardPercentKeys[4]]);
                MetaContextElementUtils.SetTextGlobal(percentileGemElements[1], "CLUB_ARENA_POPUP_CLUB_RANK_PERCENTILE_REWARD_TEXT_1",
                    dicRewardPercent[rewardPercentKeys[5]],
                    dicRewardPercent[rewardPercentKeys[6]],
                    dicRewardPercent[rewardPercentKeys[7]],
                    dicRewardPercent[rewardPercentKeys[8]],
                    dicRewardPercent[rewardPercentKeys[9]]);
            }
        }

        private void OnClickClose()
        {
            MetaSystem.UnSubscribeBackButton(this.GetHashCode());
            ClubArenaUtils.BIClientClickClubArenaPopup("close", rootBB.GetValue<string>("_biContextID"));

            Variable<GameObject> caller = BlackboardUtils.FindVariable<GameObject>(rootBB, "caller");
            if (caller != null && caller.value != null)
            {
                MessageRouter router = Common.GetOrAddMessageRouter(caller.value);
                router.Dispatch(MessageRouter.ON_CUSTOM_EVENT, new EventData("OnPopupClose"), caller.value);
            }

            PopupManager.Instance.Close(gameObject);
            rootAnimator?.SetTrigger("Close");
        }

        private void UpdateClubRankListData()
        {
            long currentRankReward = GetClubReward(ClubArenaUtils.ClubRank, ClubArenaUtils.FinalRewardRank);
            long currentPercentileReward = GetClubReward(ClubArenaUtils.Percentile, ClubArenaUtils.FinalRewardPercentile);

            if (currentRankReward > currentPercentileReward)
                SetLightActive(ClubArenaUtils.FinalRewardRank, currentRankReward, true);
            else
                SetLightActive(ClubArenaUtils.FinalRewardPercentile, currentPercentileReward, false);
        }

        private long GetClubReward(int checkValue, Dictionary<int, long> dic)
        {
            if (dic == null) return 0;

            long returnValue = 0;

            List<int> rewardKeys = new List<int>(dic.Keys);

            if (rewardKeys != null && rewardKeys.Count > 0)
            {
                for (int i = 0; i < rewardKeys.Count; ++i)
                {
                    if (checkValue <= rewardKeys[i])
                    {
                        returnValue = dic[rewardKeys[i]];
                        break;
                    }
                }
            }

            return returnValue;
        }

        public void SetActive(bool isActive)
        {
            rootAnimator?.SetBool("Active", isActive);
        }

        private void SetLightActive(Dictionary<int, long> dic, long reward, bool isRank)
        {
            if (dic == null) return;

            List<int> rewardKeys = new List<int>(dic.Keys);

            for (int i = 0; i < rewardKeys.Count; ++i)
            {
                if (reward == dic[rewardKeys[i]])
                {
                    if (isRank)
                    {
                        if (i < TOP_RANK_COUNT)
                            topRankBackgroundElements[i].gameObject.SetActive(true);
                        else
                            nextRankBackgroundElements[i - TOP_RANK_COUNT].gameObject.SetActive(true);

                        break;
                    }
                    else
                    {
                        percentileBackgroundElements[i].gameObject.SetActive(true);
                    }
                }
            }
        }
    }
}
