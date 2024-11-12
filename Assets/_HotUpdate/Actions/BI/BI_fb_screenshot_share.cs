using UnityEngine;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_fb_screenshot_share : ActionTask<Blackboard>
{
    public BBParameter<string> gameID;

    public BBParameter<long> betCoins;
    public BBParameter<long> winCoins;

    public BBParameter<bool> isAuto;
    public BBParameter<bool> isEpicAlbum;

    public BBParameter<bool> isJackpot;
    public BBParameter<object> jackpotIndex;
    public BBParameter<string> contextId;
    public BBParameter<int> categoryType;

    public BBParameter<int> winTypeIndex;

    protected override void OnExecute()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();

        data["share_type"] = isEpicAlbum.value ? "epic_album" : isAuto.value ? "auto" : "manual";

        if (isEpicAlbum.value)
        {
            int gameId = BlackboardUtils.FindVariable<int>(agent, gameID.value).value;

            Blackboard woeInfo = BlackboardQueryUtils.GetWOEInfo((CategoryType) categoryType.value, gameId);

            if (woeInfo != null)
            {
                data["bet"] = woeInfo.GetValue<long>("betCredit");
                data["win"] = woeInfo.GetValue<long>("winCredit");
            }
        }
        else
        {
            data["game_id"] = System.Convert.ToInt32(gameID.value);
            data["bet"] = betCoins.value;
            data["win"] = winCoins.value;
        }

        if(isJackpot.value == true)
        {
            data["is_jackpot"] = 1;
            data["jackpot_index"] = System.Convert.ToInt64(jackpotIndex.value);
        }
        else
        {
            data["is_jackpot"] = 0;
        }

        data["context_id"] = contextId.value;

        // 0 : big , 1 : super big, 2 : mega, 3 : super mega, 4 : epic
        if(winTypeIndex.value == 4)
            data["win_type"] = "epic";
        else if(winTypeIndex.value == 3)
            data["win_type"] = "supermega";
        else if(winTypeIndex.value == 2)
            data["win_type"] = "mega";
        else
            data["win_type"] = "epic";

        Analytics.CustomEvent("client_fb_screenshot_share", data);

        EndAction();
    }
}

}
