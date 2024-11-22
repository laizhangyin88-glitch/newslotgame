using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]

public class VideoAdsClaim : ActionTask <Blackboard> 
{
    protected override string info
    { 
        get 
        { 
            return string.Format("Video Ads Claim");
        } 
    }
    protected override void OnExecute()
    {
        int timeBonusEventID = BlackboardUtils.GetOrCreateVariable<int>(MainBlackboard.Get(), "timeBonusEventID").value;

        BagelCodeClientAPI.VideoAdsCliam(
        (response) =>
        {
            if (agent != null)
            {
                ClientAPI2Blackboard.Serialize(agent, response);

                var timebonusConsecutive        = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "me/timebonusConsecutive");
                var lastCollectTimestamp        = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "lastCollectTimestamp");
                var lastVideoAdsClaimTimestamp  = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "me/lastVideoAdsClaimTimestamp");

                lastVideoAdsClaimTimestamp.value = response.lastVideoAdsClaimTimestamp;
                timebonusConsecutive.value       = response.timebonusConsecutive;
                lastCollectTimestamp.value       = response.lastCollectTimestamp;

                EndAction(true);
            }
        },
        (error) =>
        {
            switch(error.errorCode)
            {
                // case ClientModels.Error.COOLTIME_NOT_YET_ERROR:
                //     {
                //         bool stringError = false;
                //         ErrorPopupInfo info = new ErrorPopupInfo();

                //         info.type = ErrorPopupType.OK;
                //         info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_COOLTIME_NOT_YET", out stringError);
                //         info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);

                //         ErrorPopupHandler.Instance.OpenError(info);

                //         EndAction(false);
                //     }
                //     break;
                default:
                    GlobalErrorHandler.GlobalError(error);
                    break;
            }
        });
    }
}

}
