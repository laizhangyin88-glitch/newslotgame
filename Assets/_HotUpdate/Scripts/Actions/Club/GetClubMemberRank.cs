using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

    [Category("★ BagelCode/Club")]

    public class GetClubMemberRank : ActionTask<Blackboard>
    {
        private ContextElement memberAreaElement;
        private ContextElement clubRankImageElement;
        private ContextElement clubRankTextElement;

        private GameObject clubTierIcon;

        public StringTable.StringTableType tableType;

        private bool isInit = false;

        protected override string info
        {
            get { return "Get Club Member Rank"; }
        }

        private void InitProperty()
        {
            if (isInit) return;

            var rootElement = agent.gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            memberAreaElement = ContextUtils.FindElement(rootElement, "Member", ContextSearchingType.ChildrenSearch);
            clubRankImageElement = ContextUtils.FindElement(memberAreaElement, "Image", ContextSearchingType.ChildrenSearch);
            clubRankTextElement = ContextUtils.FindElement(memberAreaElement, "Text Rank", ContextSearchingType.ChildrenSearch);

            isInit = true;
        }

        protected override void OnExecute()
        {
            InitProperty();

            var clubTier = BlackboardUtils.FindVariable<int>(agent, "clubInfoResponse/tierInfo/leagueTier");
            var clubRank = BlackboardUtils.FindVariable<int>(agent, "clubInfoResponse/leagueInfo/rank");
            var clubUnitNum = BlackboardUtils.FindVariable<int>(agent, "clubInfoResponse/leagueInfo/unitNum");
            var clubTierChangeType = BlackboardUtils.FindVariable<TierChangeType>(agent, "clubInfoResponse/leagueInfo/tierChange");
            var clubTierGroupName = BlackboardQueryUtils.GetTierGroupName(clubTier.value);
            var clubTierGroupSpriteName = BlackboardQueryUtils.GetTierGroupSpriteName(clubTier.value);

            switch (clubTierChangeType.value)
            {
                case TierChangeType.PROMOTE:
                    MetaContextElementUtils.SetText(clubRankTextElement, StringTableUtils.GetString(tableType, "CLUB_INFO_LEAGUE_RANK_PROMOTE_TEXT", clubTierGroupSpriteName, clubTierGroupName, clubRank.value, clubUnitNum.value));
                    break;
                case TierChangeType.DEMOTE:
                    MetaContextElementUtils.SetText(clubRankTextElement, StringTableUtils.GetString(tableType, "CLUB_INFO_LEAGUE_RANK_DEMOTE_TEXT", clubTierGroupSpriteName, clubTierGroupName, clubRank.value, clubUnitNum.value));
                    break;
                default:
                    MetaContextElementUtils.SetText(clubRankTextElement, StringTableUtils.GetString(tableType, "CLUB_INFO_LEAGUE_RANK_REMAIN_TEXT", clubTierGroupSpriteName, clubTierGroupName, clubRank.value, clubUnitNum.value));
                    break;
            }

            EndAction();
        }
    }
}
