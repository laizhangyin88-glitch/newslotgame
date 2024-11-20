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

public class GetClubFeedReceiveOfferInfoBB : ActionTask<Blackboard>
{
    public BBParameter<int> cellIndex;
    public BBParameter<List<GameObject>> bgList; // 0, 1
    public BBParameter<string>  clubFeedInfoValue;

    public BBParameter<string> messageText;
    public BBParameter<string> likeButtonText;
    public BBParameter<string> leftTimeAgoText;
    public BBParameter<int> cellStyle;

    protected override string info
    {
        get { return "Get Club Feed Receive Offer Info BB"; }
    }

    protected override void OnExecute()
    {
        var clubFeedInfo = BlackboardUtils.FindVariable<Blackboard>(agent, clubFeedInfoValue.value);

        if(clubFeedInfo != null)
        {
            var feedUserID = clubFeedInfo.value.GetValue<string>("userId");
            var feedUserName = clubFeedInfo.value.GetValue<string>("name");
            var meID = BlackboardUtils.FindVariable<string>(null, "/me/userId");

            bool isMeFeed = feedUserID == meID.value;

            var rewardCoins = GetRewardCoin(clubFeedInfo.value, isMeFeed);
            if(isMeFeed)
            {
                messageText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_NEWS_FEED_RECEIVE_OFFER_ME_TEXT", rewardCoins);
                likeButtonText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_LIKE");
            }
            else
            {
                messageText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_NEWS_FEED_RECEIVE_OFFER_TEXT", feedUserName);
                likeButtonText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_LIKE_WITH_COIN", rewardCoins);
            }

            var createdTimestamp = clubFeedInfo.value.GetValue<long>("createdTimestamp");
            leftTimeAgoText.value = ClubUtils.GetClubFeedLeftTimeText( TimeUtils.GetTimeStamp() - createdTimestamp );

            UpdateBG();
        }

        EndAction();
    }

    private void UpdateBG()
    {
        int cellStyle = cellIndex.value%2;

        for(int i=0; i<bgList.value.Count; ++i)
        {
            bgList.value[i].SetActive(i==cellStyle);
        }
    }

    private long GetRewardCoin(Blackboard clubFeedInfo, bool isMeFeed)
    {
        long rewardCoins = 0;

        var rewardBB = BlackboardUtils.FindVariable<Blackboard>(clubFeedInfo, "reward");
        if(rewardBB != null && rewardBB.value != null)
        {
            var rewardType = rewardBB.value.GetValue<RewardType>("type");

            if(rewardType == RewardType.CREDIT)
            {
                if (isMeFeed)
                    rewardCoins = rewardBB.value.GetValue<long>("credit");
                else
                    rewardCoins = FreebieLevelUtils.GetLevelMultiplierNumeratorValue(rewardBB.value.GetValue<long>("credit"), FreebieLevelUtils.FreebieType.CLUB_DEAL_BONUS);
            }
        }

        return rewardCoins;
    }
}

}
