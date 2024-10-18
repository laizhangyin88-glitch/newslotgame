using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.EpicAlbum
{
    [Category("★ BagelCode/EpicAlbum")]
    public class UpdateMyEpicWinRecord : ActionTask<ContextElement>
    {
        public BBParameter<int> gameId;
        // public BBParameter<bool> saveAsIsUsable;

        // private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

        private const int STAR_COUNT = 3;
        
        protected override string info
        {
            get { return "Update My Epic Win Record"; }
        }

        protected override void OnExecute()
        {
            ContextElement myEpicWinRecord = ContextUtils.FindElement(agent, "My Epic Win Record", ContextSearchingType.ChildrenSearch);
            // ContextElement myLeaderboardCell = ContextUtils.FindElement(agent, "Leaderboard Cell Me", ContextSearchingType.ChildrenSearch);
            // ContextElement myPictureArea = ContextUtils.FindElement(myLeaderboardCell, "Profile Picture Normal", ContextSearchingType.ChildrenSearch);
            
            bool enableEpicAlbum = BlackboardUtils.GetOrCreateVariable<bool>(null, "/values/misc/ENABLE_EPIC_ALBUM").value;
            bool gameExistInEpicAlbum = BlackboardQueryUtils.CheckIfGameExistInEpicAlbum(gameId.value);
            Blackboard earlyAccessSlotInfo = BlackboardQueryUtils.GetEarlyAccessSlotInfo(gameId.value);
            
            //if (enableEpicAlbum && earlyAccessSlotInfo == null && gameExistInEpicAlbum)
            //{
            //    MetaContextElementUtils.SetActive(myEpicWinRecord, true);
            //    // MetaContextElementUtils.SetActive(myLeaderboardCell, true);

            //    // MetaContextElementUtils.SimpleSetText(myLeaderboardCell, "Text Name", BlackboardUtils.FindVariable<string>(null, "/me/name").value);
                
            //    // long winCredit = BlackboardQueryUtils.GetWinCreditOfGame(gameId.value);

            //    // if (winCredit > 0)
            //    // {
            //    //     var winCoinText = StringTableUtils.GetString(tableType, "LEADER_BOARD_COIN_FORMAT", winCredit);
            //    //     MetaContextElementUtils.SimpleSetText(myLeaderboardCell, "Text Coin", winCoinText);
            //    // }
            //    // else
            //    // {
            //    //     MetaContextElementUtils.SimpleSetText(myLeaderboardCell, "Text Coin", "0");
            //    // }

            //    int starCount = BlackboardQueryUtils.GetStarCountOfGame(gameId.value);

            //    for(int i=0; i<STAR_COUNT; ++i)
            //    {
            //        MetaContextElementUtils.SimpleSetActive(myEpicWinRecord, string.Format("Star On {0}", i+1), starCount > i);
            //    }

            //    // var profileUrl = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "me/profileUrl");
            //    // var tierGroup = TierUtils.GetTierGroup(TierUtils.GetMeTier());

            //    // ContextElement pictureElement = ContextUtils.FindElement(myPictureArea, "Image", ContextSearchingType.ChildrenSearch);
            //    // if(profileUrl != null && !string.IsNullOrEmpty(profileUrl.value))
            //    //     MetaContextElementUtils.SetWebImage(pictureElement, profileUrl.value, CacheType.FileCache, false, null);

            //    // MetaContextElementUtils.SetIntProperty(myPictureArea, tierGroup);

            //    // saveAsIsUsable.value = true;
            //}
            //else
            {
                // MetaContextElementUtils.SetActive(myLeaderboardCell, false);
                MetaContextElementUtils.SetActive(myEpicWinRecord, false);

                // saveAsIsUsable.value = false;
            }

            EndAction(true);
        }
    }
}
