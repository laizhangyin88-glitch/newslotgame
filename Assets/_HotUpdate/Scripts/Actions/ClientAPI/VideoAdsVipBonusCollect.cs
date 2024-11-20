using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]

public class VideoAdsVipBonusCollect : ActionTask <Blackboard> 
{
    [BlackboardOnly]
    public BBParameter<bool> isSuccess;

	protected override string info 
	{ 
		get 
		{ 
			return string.Format("{0} = Video Ads Vip Bonus Collect", isSuccess);
		} 
	}
    
	protected override void OnExecute()
	{
        BagelCodeClientAPI.VideoAdsVipDailyBonusCollect(
        (response) =>
        {
            ClientAPI2Blackboard.Serialize(agent, response);

            BlackboardQueryUtils.AddCoins(response.earnCredit);

            var lastShopBonusVideoAdsClaimTimestamp = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "lastShopBonusVideoAdsClaimTimestamp");
            if(lastShopBonusVideoAdsClaimTimestamp != null)
                lastShopBonusVideoAdsClaimTimestamp.value = response.lastShopBonusVideoAdsClaimTimestamp;

            isSuccess.value = true;
            EndAction(true);
        },
        (error) =>
        {
            switch(error.errorCode)
            {
                case ClientModels.Error.VIDEO_ADS_COOLTIME_NOT_YET_ERROR:
                    {
                        bool stringError = false;
                        ErrorPopupInfo info = new ErrorPopupInfo();

                        info.type = ErrorPopupType.OK;
                        info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_COOLTIME_NOT_YET", out stringError);
                        info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);

                        ErrorPopupHandler.Instance.OpenError(info);

                        var lastShopBonusVideoAdsClaimTimestamp = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "lastShopBonusVideoAdsClaimTimestamp");
                        if(lastShopBonusVideoAdsClaimTimestamp != null)
                            lastShopBonusVideoAdsClaimTimestamp.value = TimeUtils.GetTimeStamp();

                        isSuccess.value = false;
                        EndAction(true);
                    }
                    break;
                default:
                    GlobalErrorHandler.GlobalError(error);
                    break;
            }
        });
	}
}

}
