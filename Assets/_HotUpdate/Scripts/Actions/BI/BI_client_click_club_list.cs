using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_client_click_club_list : ActionTask<Blackboard>
{
    public BBParameter<string> clubIDValue;
    public BBParameter<string> clubNameValue;
    public BBParameter<string> clubLevelValue;
    public BBParameter<string> clubMemberCountValue;
    public BBParameter<string> clubJoinTypeValue;

    public BBParameter<int>    cellIndex;
    public BBParameter<string> listType;
    public BBParameter<string> contextID;

    protected override void OnExecute()
    {
        var clubID          = BlackboardUtils.FindVariable<long>(agent, clubIDValue.value);
        var clubName        = BlackboardUtils.FindVariable<string>(agent, clubNameValue.value);
        var clubLevel       = BlackboardUtils.FindVariable<int>(agent, clubLevelValue.value);
        var clubMemberCount = BlackboardUtils.FindVariable<int>(agent, clubMemberCountValue.value);
        var clubJoinType    = BlackboardUtils.FindVariable<ClubJoinType>(agent, clubJoinTypeValue.value);

        Dictionary<string, object> customData = new Dictionary<string, object>();

        customData["club_id"] = clubID.value;
        customData["club_name"] = clubName.value;
        customData["club_level"] = clubLevel.value;
        customData["club_member_count"] = clubMemberCount.value;
        customData["club_type"] = clubJoinType.value == ClubJoinType.PRIVATE ? "private" : "public";
        customData["index"] = cellIndex.value;
        customData["list_type"] = listType.value;

        if(!string.IsNullOrEmpty(contextID.value))
            customData["context_id"] = contextID.value;

        Analytics.CustomEvent("client_click_club_list", customData);

        EndAction();
    }
}

}
