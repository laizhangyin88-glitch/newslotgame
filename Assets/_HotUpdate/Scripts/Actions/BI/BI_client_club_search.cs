using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_client_club_search : ActionTask<Blackboard>
{
    public BBParameter<string> contextID;
    public BBParameter<string> searchType;

    public BBParameter<string> searchText;

    public BBParameter<string> minClubLevelValue;
    public BBParameter<string> minPlayerLevelValue;
    public BBParameter<string> joinSearchTypeValue;
    public BBParameter<string> isAvailableForMeValue;
    public BBParameter<string> isFriendsInsideValue;
    public BBParameter<string> sourceContextID;
    public BBParameter<string> openType;

    protected override string info 
    {
        get { return string.Format("BI client_club_search ({0})", searchType); }
    }

    protected override void OnExecute()
    {
        var searchClubList = BlackboardUtils.FindVariable<List<Blackboard>>(agent, "response/clubList");

        Dictionary<string, object> customData = new Dictionary<string, object>();

        customData["context_id"] = contextID.value;
        customData["search_type"] = searchType.value;
        customData["result_count"] = searchClubList.value.Count;

        if(searchType.value == "name")
        {
            customData["search_string"] = searchText.value;
        }
        else
        {
            var minClubLevel     = BlackboardUtils.FindVariable<int>(agent, minClubLevelValue.value);
            var minPlayerLevel   = BlackboardUtils.FindVariable<int>(agent, minPlayerLevelValue.value);
            var joinSearchType   = BlackboardUtils.FindVariable<ClubJoinSearchType>(agent, joinSearchTypeValue.value);
            var isAvailableForMe = BlackboardUtils.FindVariable<bool>(agent, isAvailableForMeValue.value);
            var isFriendsInside  = BlackboardUtils.FindVariable<bool>(agent, isFriendsInsideValue.value);

            customData["min_club_level"] = minClubLevel.value;
            customData["min_player_level"] = minPlayerLevel.value;
            customData["is_available_for_me"] = isAvailableForMe.value;
            customData["is_friends_inside"] = isFriendsInside.value;

            switch(joinSearchType.value)
            {
                case ClubJoinSearchType.PUBLIC:
                    customData["club_type"] = "public";
                    break;
                case ClubJoinSearchType.PRIVATE:
                    customData["club_type"] = "private";
                    break;
                default:
                    customData["club_type"] = "all";
                    break;
            }
        }

        customData["source_context_id"] = sourceContextID.value;
        customData["type"] = openType.value;

        Analytics.CustomEvent("client_club_search", customData);

        EndAction();
    }
}

}
