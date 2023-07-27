using System.Collections.Generic;
using SlotMaker;
using NodeCanvas.Framework;

namespace BagelCode.BossRaiders
{
    public class BossRaidersSceneClubLeadersController
    {
        private ContextElement rootElement;

        private ContextElement buttonClubReaders;
        private ContextElement[] clubRewardLeadersElements;

        private BossRaidersStageClear stageClearController;

        private const int USER_LEADERS_COUNT = 3;

        public void OnInit(ContextElement _rootElement)
        {
            rootElement = _rootElement;

            InitProperty();
            InitText();
        }

        private void InitProperty()
        {
            buttonClubReaders = ContextUtils.FindElement(rootElement, "Button Leaders", ContextSearchingType.ChildrenSearch);
            ContextElement clubRewardTextElement = ContextUtils.FindElement(rootElement, "Text Reward", ContextSearchingType.ChildrenSearch);
            stageClearController = new BossRaidersStageClear();
            stageClearController.OnInit(clubRewardTextElement,
                StringTableUtils.GetString(StringTable.StringTableType.Global, "BOSS_RAIDERS_LEAGUE_POINT", BossRaidersUtils.GetResultGem(BossRaidersUtils.CurrentReward), BossRaidersUtils.CurrentLeaguePoint));
            clubRewardLeadersElements = new ContextElement[USER_LEADERS_COUNT];

            for (int i = 0; i < USER_LEADERS_COUNT; ++i)
            {
                clubRewardLeadersElements[i] = ContextUtils.FindElement(rootElement, string.Format("0{0}", i + 1), ContextSearchingType.ChildrenSearch);
            }

            MetaContextElementUtils.SetClickable(
                buttonClubReaders,
                "OnClickClubReaders",
                rootElement,
                null
            );
        }

        private void InitText()
        {
            MetaContextElementUtils.SimpleSetTextGlobal(buttonClubReaders, "Text", "BOSS_RAIDERS_CLUB_LEADERS", ContextSearchingType.ChildrenSearch);
        }

        public void UpdateBossRewardPoint()
        {
            stageClearController?.UpdateBossRewardPoint(StringTableUtils.GetString(StringTable.StringTableType.Global, "BOSS_RAIDERS_LEAGUE_POINT", BossRaidersUtils.GetResultGem(BossRaidersUtils.CurrentReward), BossRaidersUtils.CurrentLeaguePoint));
        }

        public void UpdateClubLeaders()
        {
            List <Blackboard> userList = BossRaidersUtils.ClubLeadersContributionInfoList;

            if (userList == null) return;

            int userListCount = userList.Count;
            for (int i = 0; i < USER_LEADERS_COUNT; ++i)
            {
                if (userListCount > i)
                {
                    Blackboard bb = clubRewardLeadersElements[i].GetComponent<Blackboard>();
                    BlackboardUtils.SetOrCreateValue(bb, "userInfo", userList[i]);
                    BlackboardUtils.SetOrCreateValue(bb, "updateProfile", true);
                }
                else
                    break;
            }
        }
    }
}