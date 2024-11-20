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

public class GetClubFeedWinBonusInfoBB : ActionTask<Blackboard>
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
        get { return "Get Club Feed Win Bonus Info BB"; }
    }

    protected override void OnExecute()
    {
        var clubFeedInfo = BlackboardUtils.FindVariable<Blackboard>(agent, clubFeedInfoValue.value);

        if(clubFeedInfo != null)
        {
            var feedUserID = clubFeedInfo.value.GetValue<string>("userId");
            var meID = BlackboardUtils.FindVariable<string>(null, "/me/userId");

            bool isMeFeed = feedUserID == meID.value;
            bool error = false;
            if(isMeFeed)
            {
                messageText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_NEWS_FEED_EPIC_WIN_BONUS_ME_TEXT", out error);
                likeButtonText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_LIKE", out error);
            }
            else
            {
                var winBonus = clubFeedInfo.value.GetValue<long>("winBonus");
                winBonus = FreebieLevelUtils.GetLevelMultiplierNumeratorValue(winBonus, FreebieLevelUtils.FreebieType.EPIC_WIN_SHARE);
                messageText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_NEWS_FEED_EPIC_WIN_BONUS_TEXT", out error);
                likeButtonText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_LIKE_WITH_COIN", out error, winBonus);
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
}

}
