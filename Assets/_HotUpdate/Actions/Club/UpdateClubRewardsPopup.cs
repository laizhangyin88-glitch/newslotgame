using System;
using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Club")]
    public class UpdateClubRewardsPopup : ActionTask<Blackboard>
    {
        public BBParameter<string> leagueInfoResponse;

        protected override string info
        {
            get { return "Update Club League Reward Info"; }
        }

        protected override void OnExecute()
        {
            var responseBB = BlackboardUtils.FindVariable<Blackboard>(agent, leagueInfoResponse.value);

            if(responseBB != null)
            {
                ContextElement agentElement = agent.gameObject.GetComponent<ContextElement>();

                ContextElement titleTierTextElement      = ContextUtils.FindElement(agentElement, "Title Area/Text Club Tier Title", ContextSearchingType.FullNameSearch); 

                ContextElement group1NextRankTextElement = ContextUtils.FindElement(agentElement, "Reward Group 01/Text Rank Cell 02", ContextSearchingType.FullNameSearch);
                ContextElement group2PrevRankTextElement = ContextUtils.FindElement(agentElement, "Reward Group 02/Text Rank Cell 01", ContextSearchingType.FullNameSearch);
                ContextElement group2NextRankTextElement = ContextUtils.FindElement(agentElement, "Reward Group 02/Text Rank Cell 02", ContextSearchingType.FullNameSearch);
                ContextElement group3PrevRankTextElement = ContextUtils.FindElement(agentElement, "Reward Group 03/Text Rank Cell 01", ContextSearchingType.FullNameSearch);
                ContextElement group3NextRankTextElement = ContextUtils.FindElement(agentElement, "Reward Group 03/Text Rank Cell 02", ContextSearchingType.FullNameSearch);
                ContextElement group4PrevRankTextElement = ContextUtils.FindElement(agentElement, "Reward Group 04/Text Rank Cell 01", ContextSearchingType.FullNameSearch);
                ContextElement group4NextRankTextElement = ContextUtils.FindElement(agentElement, "Reward Group 04/Text Rank Cell 02", ContextSearchingType.FullNameSearch);

                ContextElement group1PromoteRankElement    = ContextUtils.FindElement(agentElement, "Reward Group 01/Rank Cell Promote", ContextSearchingType.FullNameSearch);
                ContextElement group1RemainRankElement     = ContextUtils.FindElement(agentElement, "Reward Group 01/Rank Cell Remain", ContextSearchingType.FullNameSearch);

                ContextElement group4DemoteRankElement     = ContextUtils.FindElement(agentElement, "Reward Group 04/Rank Cell Demote", ContextSearchingType.FullNameSearch);
                ContextElement group4RemainRankElement     = ContextUtils.FindElement(agentElement, "Reward Group 04/Rank Cell Remain", ContextSearchingType.FullNameSearch);

                ContextElement group1PromoteElement        = ContextUtils.FindElement(agentElement, "Tier Change Anchor Top/Promote", ContextSearchingType.FullNameSearch);
                ContextElement group1RemainElement         = ContextUtils.FindElement(agentElement, "Tier Change Anchor Top/Remain", ContextSearchingType.FullNameSearch);

                ContextElement group4DemoteElement         = ContextUtils.FindElement(agentElement, "Tier Change Anchor Bottom/Demote", ContextSearchingType.FullNameSearch);
                ContextElement group4RemainElement         = ContextUtils.FindElement(agentElement, "Tier Change Anchor Bottom/Remain", ContextSearchingType.FullNameSearch);

                ContextElement group1TierElement           = ContextUtils.FindElement(agentElement, "Reward Group 01/Text Club Tier", ContextSearchingType.FullNameSearch);
                ContextElement group2TierElement           = ContextUtils.FindElement(agentElement, "Reward Group 02/Text Club Tier", ContextSearchingType.FullNameSearch);
                ContextElement group3TierElement           = ContextUtils.FindElement(agentElement, "Reward Group 03/Text Club Tier", ContextSearchingType.FullNameSearch);
                ContextElement group4TierElement           = ContextUtils.FindElement(agentElement, "Reward Group 04/Text Club Tier", ContextSearchingType.FullNameSearch);

                ContextElement group1RewardTextElement     = ContextUtils.FindElement(agentElement, "Reward Group 01/Text Reward", ContextSearchingType.FullNameSearch);
                ContextElement group2RewardTextElement     = ContextUtils.FindElement(agentElement, "Reward Group 02/Text Reward", ContextSearchingType.FullNameSearch);
                ContextElement group3RewardTextElement     = ContextUtils.FindElement(agentElement, "Reward Group 03/Text Reward", ContextSearchingType.FullNameSearch);
                ContextElement group4RewardTextElement     = ContextUtils.FindElement(agentElement, "Reward Group 04/Text Reward", ContextSearchingType.FullNameSearch);

                ContextElement descriptionTextElement      = ContextUtils.FindElement(agentElement, "Text Description", ContextSearchingType.ChildrenSearch);

                var indexPromote = responseBB.value.GetValue<int>("indexPromote");
                var indexDemote = responseBB.value.GetValue<int>("indexDemote");
                var openMaxTier = responseBB.value.GetValue<int>("maxOpenedLeagueTier");
                var leagueMaxTier = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "values/club/LEAGUE/MAX_TIER");
                var rewardChangeIndex = responseBB.value.GetValue<int>("indexRewardChange");
                var leagueUnitCount = responseBB.value.GetValue<int>("unitMaxNum");
                var clubMultiplier = ClubUtils.GetClubRewardMultiplierNumerator();

                var clubTierInfoBB = BlackboardUtils.FindVariable<Blackboard>(responseBB.value, "tierInfo");
                var clubTier = clubTierInfoBB.value.GetValue<int>("leagueTier");

                SetTextElementText(group1NextRankTextElement, (indexPromote + 1).ToString());
                SetTextElementText(group2PrevRankTextElement, (indexPromote + 2).ToString());
                SetTextElementText(group2NextRankTextElement, (rewardChangeIndex + 0).ToString());
                SetTextElementText(group3PrevRankTextElement, (rewardChangeIndex + 1).ToString());
                SetTextElementText(group3NextRankTextElement, (indexDemote + 0).ToString());
                SetTextElementText(group4PrevRankTextElement, (indexDemote + 1).ToString());
                SetTextElementText(group4NextRankTextElement, leagueUnitCount.ToString());

                int nextTier = clubTier;
                int prevTier = clubTier;

                if(clubTier == 0)
                {
                    nextTier = clubTier + 1;
                }
                else if(clubTier == leagueMaxTier.value || clubTier >= openMaxTier)
                {
                    prevTier = clubTier - 1;
                }
                else
                {
                    nextTier = clubTier + 1;
                    prevTier = clubTier - 1;
                }

                // Tier Promote <-> Remain
                group1PromoteRankElement.gameObject.SetActive(nextTier > clubTier);
                group1RemainRankElement.gameObject.SetActive(nextTier == clubTier);
                group1PromoteElement.gameObject.SetActive(nextTier > clubTier);
                group1RemainElement.gameObject.SetActive(nextTier == clubTier);

                // Tier Demote <-> Remain
                group4DemoteRankElement.gameObject.SetActive(prevTier < clubTier);
                group4RemainRankElement.gameObject.SetActive(prevTier == clubTier);
                group4DemoteElement.gameObject.SetActive(prevTier < clubTier);
                group4RemainElement.gameObject.SetActive(prevTier == clubTier);

                SetTextElementText(titleTierTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "TEXT_CLUB_TIER_SPRITE", clubTier));

                SetTextElementText(group1TierElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "TEXT_CLUB_TIER_SPRITE", nextTier));
                SetTextElementText(group2TierElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "TEXT_CLUB_TIER_SPRITE", clubTier));
                SetTextElementText(group3TierElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "TEXT_CLUB_TIER_SPRITE", clubTier));
                SetTextElementText(group4TierElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "TEXT_CLUB_TIER_SPRITE", prevTier));

                var rewardList = responseBB.value.GetValue<List<long>>("rewardCreditList");

                SetTextElementText(group1RewardTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "SIMPLE_COIN", NumberUtils.GetMultiplierNumeratorValue(rewardList[0], clubMultiplier)));
                SetTextElementText(group2RewardTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "SIMPLE_COIN", NumberUtils.GetMultiplierNumeratorValue(rewardList[indexPromote + 1], clubMultiplier)));
                SetTextElementText(group3RewardTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "SIMPLE_COIN", NumberUtils.GetMultiplierNumeratorValue(rewardList[indexDemote - 1], clubMultiplier)));
                SetTextElementText(group4RewardTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "SIMPLE_COIN", NumberUtils.GetMultiplierNumeratorValue(rewardList[rewardList.Count -1], clubMultiplier)));

                SetTextElementText(descriptionTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_LEAGUE_REWARD_DESC_TEXT", indexPromote + 1));
            }
        
            EndAction();
        }

        private void SetTextElementText(ContextElement element, string text)
        {
            IContextText textElement = element as IContextText;
            if(textElement != null)
                textElement.SetText(text);
        }
    }
}
