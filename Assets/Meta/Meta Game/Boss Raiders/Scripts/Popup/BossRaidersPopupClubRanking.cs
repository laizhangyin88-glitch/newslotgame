using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using NodeCanvas.Framework;

namespace BagelCode.BossRaiders
{
    public class BossRaidersPopupClubRanking : MonoBehaviour
    {
        private ContextElement rootElement;

        private ContextElement titleRankTextElement;
        private ContextElement rewardGroupElement;

        private ContextElement[] topRankTextElements;
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
            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            ContextElement titleAreaElement = ContextUtils.FindElement(rootElement, "Title Area", ContextSearchingType.ChildrenSearch);
            ContextElement currentClubElement = ContextUtils.FindElement(titleAreaElement, "Current Club Ranking", ContextSearchingType.ChildrenSearch);
            titleRankTextElement = ContextUtils.FindElement(currentClubElement, "Title Text", ContextSearchingType.ChildrenSearch);
            rewardGroupElement = ContextUtils.FindElement(rootElement, "Reward Group", ContextSearchingType.ChildrenSearch);

            InitTopRankElement();

            ContextElement buttonCloseElement = ContextUtils.FindElement(rootElement, "Button Close", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SetClickable(
                buttonCloseElement,
                "OnClose",
                rootElement,
                null
            );

            InitTexts();
            UpdateClubRankListData();
        }

        private void InitTopRankElement()
        {
            topRankTextElements = new ContextElement[TOP_RANK_COUNT];
            topRankRewardTextElements = new ContextElement[TOP_RANK_COUNT];
            topRankBackgroundElements = new ContextElement[TOP_RANK_COUNT];

            nextRankTextElements = new ContextElement[NEXT_RANK_COUNT];
            nextRankRewardTextElements = new ContextElement[NEXT_RANK_COUNT];

            ContextElement[] rankRootElements = new ContextElement[TOP_RANK_COUNT];
            rankRootElements[0] = ContextUtils.FindElement(rootElement, "Reward 1st", ContextSearchingType.ChildrenSearch);
            rankRootElements[1] = ContextUtils.FindElement(rootElement, "Reward 2nd", ContextSearchingType.ChildrenSearch);
            rankRootElements[2] = ContextUtils.FindElement(rootElement, "Reward 3rd", ContextSearchingType.ChildrenSearch);

            ContextElement rankLightElement = ContextUtils.FindElement(rootElement, "My Rank Position Light", ContextSearchingType.ChildrenSearch);

            for (int i = 0; i < TOP_RANK_COUNT; ++i)
            {
                topRankTextElements[i] = ContextUtils.FindElement(rankRootElements[i], "Text", ContextSearchingType.ChildrenSearch);
                topRankRewardTextElements[i] = ContextUtils.FindElement(rankRootElements[i], "Text Reward", ContextSearchingType.ChildrenSearch);
                topRankBackgroundElements[i] = ContextUtils.FindElement(rankLightElement, string.Format("Top Rank {0}", i + 1), ContextSearchingType.ChildrenSearch);
                topRankBackgroundElements[i].gameObject.SetActive(false);
            }

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
        }

        private void InitTexts()
        {
            if (titleRankTextElement != null)
                MetaContextElementUtils.SetTextGlobal(titleRankTextElement, "BOSS_RAIDERS_POPUP_CLUB_RANK", BossRaidersUtils.GetClubRank(), BossRaidersUtils.ClubRank, BossRaidersUtils.GetPercentile());

            Dictionary<int, long> dicRewardRank = BossRaidersUtils.FinalRewardRankPayTypeInfo;
            Dictionary<int, long> dicRewardPercent = BossRaidersUtils.FinalRewardPercentilePayTypeInfo;
            List<int> rewardRankKeys = new List<int>(dicRewardRank.Keys);
            List<int> rewardPercentKeys = new List<int>(dicRewardPercent.Keys);

            if (rewardRankKeys != null && rewardRankKeys.Count > 0)
            {
                int lastRewardRankKey = 0;
                for (int i = 0; i < TOP_RANK_COUNT; ++i)
                {
                    MetaContextElementUtils.SetTextGlobal(topRankRewardTextElements[i], "BOSS_RAIDERS_POPUP_CLUB_RANK_REWARD_TEXT", dicRewardRank[rewardRankKeys[i]]);
                    lastRewardRankKey = rewardRankKeys[i];
                }

                for (int i = 0; i < NEXT_RANK_COUNT; ++i)
                {
                    int idx = i + TOP_RANK_COUNT;
                    MetaContextElementUtils.SetTextGlobal(nextRankRewardTextElements[i], "BOSS_RAIDERS_POPUP_CLUB_RANK_REWARD_TEXT", dicRewardRank[rewardRankKeys[idx]]);
                    MetaContextElementUtils.SetTextGlobal(nextRankTextElements[i], "BOSS_RAIDERS_POPUP_CLUB_RANK_TEXT", lastRewardRankKey + 1, rewardRankKeys[idx]);
                    lastRewardRankKey = rewardRankKeys[idx];
                }
            }

            if (rewardPercentKeys != null && rewardPercentKeys.Count > 0)
            {
                int firstRank = 0;
                MetaContextElementUtils.SetTextGlobal(percentileTextElements[0], "BOSS_RAIDERS_POPUP_CLUB_RANK_PERCENTILE_TEXT_0",
                    firstRank + 1, rewardPercentKeys[0],
                    rewardPercentKeys[0] + 1, rewardPercentKeys[1],
                    rewardPercentKeys[1] + 1, rewardPercentKeys[2],
                    rewardPercentKeys[2] + 1, rewardPercentKeys[3],
                    rewardPercentKeys[3] + 1, rewardPercentKeys[4]);
                MetaContextElementUtils.SetTextGlobal(percentileTextElements[1], "BOSS_RAIDERS_POPUP_CLUB_RANK_PERCENTILE_TEXT_1",
                    rewardPercentKeys[4] + 1, rewardPercentKeys[5],
                    rewardPercentKeys[5] + 1, rewardPercentKeys[6],
                    rewardPercentKeys[6] + 1, rewardPercentKeys[7],
                    rewardPercentKeys[7] + 1, rewardPercentKeys[8],
                    rewardPercentKeys[8] + 1, rewardPercentKeys[9]);

                MetaContextElementUtils.SetTextGlobal(percentileGemElements[0], "BOSS_RAIDERS_POPUP_CLUB_RANK_PERCENTILE_REWARD_TEXT_0",
                    dicRewardPercent[rewardPercentKeys[0]],
                    dicRewardPercent[rewardPercentKeys[1]],
                    dicRewardPercent[rewardPercentKeys[2]],
                    dicRewardPercent[rewardPercentKeys[3]],
                    dicRewardPercent[rewardPercentKeys[4]]);
                MetaContextElementUtils.SetTextGlobal(percentileGemElements[1], "BOSS_RAIDERS_POPUP_CLUB_RANK_PERCENTILE_REWARD_TEXT_1",
                    dicRewardPercent[rewardPercentKeys[5]],
                    dicRewardPercent[rewardPercentKeys[6]],
                    dicRewardPercent[rewardPercentKeys[7]],
                    dicRewardPercent[rewardPercentKeys[8]],
                    dicRewardPercent[rewardPercentKeys[9]]);
            }
        }

        private void UpdateClubRankListData()
        {
            long currentRankReward = GetClubReward(BossRaidersUtils.ClubRank, BossRaidersUtils.FinalRewardRankPayTypeInfo);
            long currentPercentileReward = GetClubReward(BossRaidersUtils.Percentile, BossRaidersUtils.FinalRewardPercentilePayTypeInfo);

            if (currentRankReward > currentPercentileReward)
                SetLightActive(BossRaidersUtils.FinalRewardRankPayTypeInfo, currentRankReward, true);
            else
                SetLightActive(BossRaidersUtils.FinalRewardPercentilePayTypeInfo, currentPercentileReward, false);
        }

        private long GetClubReward(int checkValue, Dictionary<int, long> dic)
        {
            if (dic == null || checkValue < 0) return 0;

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
