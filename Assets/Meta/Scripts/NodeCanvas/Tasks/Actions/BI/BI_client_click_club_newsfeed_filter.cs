using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_client_click_club_newsfeed_filter : ActionTask<Blackboard>
{
    public BBParameter<ClubFeedFilterType> filterType;

    protected override string info
    {
        get { return string.Format("BI Client Click Newsfeed Tab ({0})", filterType); }
    }

    protected override void OnExecute()
    {
        Dictionary<string, object> customData = new Dictionary<string, object>();

        customData["type"] = GetFilterTypeToBIString(filterType.value);

        Analytics.CustomEvent("client_click_club_newsfeed_filter", customData);

        EndAction();
    }

    private string GetFilterTypeToBIString(ClubFeedFilterType clubFilterType)
    {
        switch(clubFilterType)
        {
            case ClubFeedFilterType.ALL:
                return "all";
            case ClubFeedFilterType.EPICS:
                return "rewards";
            case ClubFeedFilterType.REQUESTS:
                return "requests";
            case ClubFeedFilterType.MESSAGES:
                return "messages";
        }

        return "unknown";
    }
}

}
