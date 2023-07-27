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

public class GetClubNewsFeedBadgeBB : ActionTask<Blackboard>
{
    public BBParameter<string> saveMeClubID;
    public BBParameter<string> saveLastClubFeedRecordID;
    public BBParameter<int> saveShowNewBadge;

    protected override string info
    {
        get { return "Get Club News Feed Badge BB"; }
    }

    protected override void OnExecute()
    {
        saveShowNewBadge.value = 0;
        saveLastClubFeedRecordID.value = "0";

        var meClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubId");
        saveMeClubID.value = meClubID.value.ToString();

        if(meClubID.value > 0)
        {
            var lastClubFeedRecordID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubBadgeInfo/latestFeedRecordId");

            if(lastClubFeedRecordID.value > 0)
            {
                saveLastClubFeedRecordID.value = lastClubFeedRecordID.value.ToString();

                long lastShowClubID = System.Convert.ToInt64( PlayerPrefs.GetString("NEWS_FEED_CLUB_ID", "0") );
                if(lastShowClubID == 0 || lastShowClubID != meClubID.value)
                {
                    saveShowNewBadge.value = 1;
                }
                else
                {
                    long lastShowNewsFeedBadgeID = System.Convert.ToInt64( PlayerPrefs.GetString("LAST_CLUB_NEWS_FEED_ID", "0") );

                    if(lastClubFeedRecordID.value > lastShowNewsFeedBadgeID)
                        saveShowNewBadge.value = 1;
                }
            }
        }

        EndAction();
    }
}

}
    
