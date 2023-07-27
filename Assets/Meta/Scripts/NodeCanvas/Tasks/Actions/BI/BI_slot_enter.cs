using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_slot_enter : ActionTask
{
    protected override void OnExecute()
    {
        var me = BlackboardUtils.FindVariable<Blackboard>(null, "/me").value;

        var game_id = BlackboardUtils.FindVariable<int>(null, "/enterGameInfo/gameId");
        var from_type = BlackboardUtils.FindVariable<string>(null, "/enterGameInfo/fromType");
        var notice_id = BlackboardUtils.FindVariable<int>(null, "/enterGameInfo/noticeId");
        var slb_id = BlackboardUtils.FindVariable<string>(null, "/enterGameInfo/slbId");
        var context_id = BiEventUtils.GetSlotEnterContextID();
        
        long timestamp = BagelCode.TimeUtils.GetTimeStamp();

        string assetDownloadTime = PlayerPrefs.GetString("asset_download_time", "");

        Dictionary<string, object> customData = new Dictionary<string, object>();

        customData["bet_zone_id"] = "free";
        customData["enter_type"] = from_type.value;
        customData["game_id"] = game_id.value;
        customData["asset_download_time"] = (timestamp - System.Convert.ToInt64(assetDownloadTime));

        if (notice_id != null && notice_id.value > 0)
        {
            customData["notice_id"] = notice_id.value;
        }

        if (slb_id != null && !string.IsNullOrEmpty(slb_id.value))
        {
            customData["slb_id"] = slb_id.value;
        }
        
        if (!string.IsNullOrEmpty(context_id))
        {
            customData["context_id"] = context_id;
        }

        Analytics.CustomEvent("client_slot_enter", customData);
        AdjustManager.Instance.SendEvent("slot_enter");

        EndAction();
    }
}

}
