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

public class GetClubFeedLogInfoBB : ActionTask<Blackboard>
{
    public BBParameter<int> cellIndex;
    public BBParameter<List<GameObject>> bgList; // 0, 1
    public BBParameter<string>  clubFeedInfoValue;

    public BBParameter<string> message;
    public BBParameter<string> leftTimeAgoText;
    public BBParameter<int> cellStyle;

    protected override string info
    {
        get { return "Get Club Feed Log Info BB"; }
    }

    protected override void OnExecute()
    {
        var clubFeedInfo = BlackboardUtils.FindVariable<Blackboard>(agent, clubFeedInfoValue.value);

        if(clubFeedInfo != null)
        {
            ClubFeedType feedType = clubFeedInfo.value.GetValue<ClubFeedType>("type");

            switch(feedType)
            {
                case ClubFeedType.COLEADER_PROMOTE:
                    {
                        var userName = clubFeedInfo.value.GetValue<string>("name");
                        message.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_NEWS_FEED_PROMOTE_TEXT", userName);
                    }
                    break;
                case ClubFeedType.COLEADER_DEMOTE:
                    {
                        var userName = clubFeedInfo.value.GetValue<string>("name");
                        message.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_NEWS_FEED_DEMOTE_TEXT", userName);
                    }
                    break;
                case ClubFeedType.JOIN:
                    {
                        var userName = clubFeedInfo.value.GetValue<string>("name");
                        message.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_NEWS_FEED_JOIN_TEXT", userName);
                    }
                    break;
                case ClubFeedType.LEVEL_UP:
                    {
                        var clubLevel = clubFeedInfo.value.GetValue<int>("level");
                        message.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_NEWS_FEED_LEVEL_UP_TEXT", clubLevel);
                    }
                    break;
                case ClubFeedType.LEADER_CHANGE:
                    {
                        var userName = clubFeedInfo.value.GetValue<string>("name");
                        message.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_NEWS_FEED_LEADER_CHANGE_TEXT", userName);
                    }
                    break;
                default:
                    message.value = "";
                    break;
            }

            var createdTimestamp = BlackboardUtils.GetOrCreateVariable<long>(clubFeedInfo.value, "createdTimestamp").value;
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
