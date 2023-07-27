using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]

public class RequestClubFeedList : ActionTask <Blackboard> 
{
    public BBParameter<string>  feedIDValue;
    public BBParameter<int>     reqFeedCount;
    public BBParameter<int>     clubTier;
    public BBParameter<int>     mgEventID;
    public BBParameter<ClubFeedFilterType> filterType;

    public BBParameter<Blackboard> saveAsResponse;

    protected override string info
    { 
        get 
        { 
            return string.Format("Request Club Feed List {0}, {1}", reqFeedCount, filterType);
        } 
    }
    protected override void OnExecute()
    {
        var feedID = BlackboardUtils.FindVariable<long>(agent, feedIDValue.value);

        long reqBlockSeq = 0;

        BagelCodeClientAPI.RequestClubNewsFeedList(filterType.value, feedID.value, reqFeedCount.value, mgEventID.value, out reqBlockSeq, true,
        (response) =>
        {
            if(agent != null)
            {
                var bb = BlackboardUtils.GetOrCreateBlackboard(agent, "clubNewsFeedResponse");

                // Test Dummy Data
                // ClubFeedContentClubTierInfo clubTierInfo = new ClubFeedContentClubTierInfo();
                // clubTierInfo.leagueTier = 3;
                // clubTierInfo.tierChange = ClubLeagueTierChangeType.REMAIN;

                // ClubFeed clubTierFeed = new ClubFeed();
                // clubTierFeed.id = 999;
                // clubTierFeed.createdTimestamp = TimeUtils.GetTimeStamp();
                // clubTierFeed.type = ClubFeedType.CLUB_TIER_INFO;
                // clubTierFeed.like = 10;
                // clubTierFeed.userLiked = false;
                // clubTierFeed.content = clubTierInfo;

                // response.feedList.Add(clubTierFeed);

                // RewardInfoValueCredit creditRewardInfo = new RewardInfoValueCredit();
                // creditRewardInfo.credit = 120000;
                // RewardInfo leagueRewardInfo = new RewardInfo();
                // leagueRewardInfo.type = RewardType.CREDIT;
                // leagueRewardInfo.value = creditRewardInfo;

                // ClubFeedContentClubLeagueReward leagueReward = new ClubFeedContentClubLeagueReward();
                // leagueReward.rank = 1;
                // leagueReward.reward = leagueRewardInfo;

                // ClubFeed clubLeagueRewardFeed = new ClubFeed();
                // clubLeagueRewardFeed.id = 999;
                // clubLeagueRewardFeed.createdTimestamp = TimeUtils.GetTimeStamp();
                // clubLeagueRewardFeed.type = ClubFeedType.CLUB_LEAGUE_REWARD;
                // clubLeagueRewardFeed.like = 10;
                // clubLeagueRewardFeed.userLiked = false;
                // clubLeagueRewardFeed.content = leagueReward;

                // response.feedList.Add(clubLeagueRewardFeed);
                /////////////////////////////////

                BlackboardUtils.ClearBlackboard(bb);
                ClientAPI2Blackboard.Serialize(bb, response);

                // request feedid > savefeedid
                long lastFeedID = 0;
                long lastRewardID = 0;
                if(response.feedList.Count > 0)
                {
                    lastFeedID = response.feedList[0].id;
                    lastRewardID = lastFeedID;
                }
                else
                {
                    lastFeedID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubBadgeInfo/latestFeedRecordId").value;
                    lastRewardID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubBadgeInfo/latestRewardFeedRecordId").value;
                }

                var meClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubId");
                BlackboardQueryUtils.UpdateClubBadgeLastFeed(meClubID.value, lastFeedID, lastRewardID);

                saveAsResponse.value = bb as Blackboard;

                EndAction(true);
            }
            
        },
        (error) =>
        {
            switch(error.errorCode)
            {
                case ClientModels.Error.SOMEONE_UPDATING_CLUB_ERROR:
                    GlobalErrorHandler.OpenAlertPopup("ERROR_SOMETHING_WRONG");
                    if(agent != null)
                        EndAction(true);
                    break;
                case ClientModels.Error.NOT_IN_CLUB_ERROR:
                    {
                        var meClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubId");
                        if(meClubID.value > 0)
                        {
                            bool stringError = false;
                            ErrorPopupInfo info = new ErrorPopupInfo();
                            info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CLUB_REMOVED_ALERT", out stringError);
                            info.type = ErrorPopupType.OK;
                            info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);
                            
                            ErrorPopupHandler.Instance.OpenError(info);
                        }

                        BlackboardQueryUtils.SetMyClubId(0);
                        MessageDispatcher.Dispatch("OnMetaUIEvent", new EventData("OnRemovedClub"));
                    }
                    break;
                default:
                    GlobalErrorHandler.GlobalError(error);
                    break;
            }
        });
    }
}

}
