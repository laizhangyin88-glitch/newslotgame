using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]

public class VipBonusCollect : ActionTask <Blackboard> 
{
    [BlackboardOnly]
    public BBParameter<bool> isSuccess;

	protected override string info 
	{ 
		get 
		{ 
			return string.Format("{0} = Vip Bonus Collect", isSuccess);
		} 
	}
    
	protected override void OnExecute()
	{
        BagelCodeClientAPI.VipDailyBonusCollect(
        (response) =>
        {
            ClientAPI2Blackboard.Serialize(agent, response);

            BlackboardQueryUtils.AddCoins(response.earnCredit);
            BlackboardQueryUtils.UpdateVIPLoungeInfo(response.vipLoungeInfo);
            var vipDailyBonusTimestamp = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "me/vipDailybonusTimestamp");
            vipDailyBonusTimestamp.value = response.vipDailybonusTimestamp;

            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["earn_coin"] = response.earnCredit;
            if (VipLounge.VipLounge.Utils.IsEnded)
                customData["vip_lounge_extra_reward"] = null;
            else
                customData["vip_lounge_extra_reward"] = response.earnCredit - NumberUtils.GetDevideNumeratorValue(response.earnCredit, BlackboardQueryUtils.GetVIPLoungeClubVegasRewardActiveNumerator("SHOP_BONUS"));
            BiEventUtils.AppendLevelMultiplierEventData(customData, FreebieLevelUtils.GetFreebieTypeToString(FreebieLevelUtils.FreebieType.VIP_BONUS));

            Analytics.CustomEvent("client_vip_bonus", customData);

            isSuccess.value = true;
            EndAction(true);
        },
        (error) =>
        {
            switch(error.errorCode)
            {
                case ClientModels.Error.COOLTIME_NOT_YET_ERROR:
                    {
                        bool stringError = false;
                        ErrorPopupInfo info = new ErrorPopupInfo();

                        info.type = ErrorPopupType.OK;
                        info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_COOLTIME_NOT_YET", out stringError);
                        info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);

                        ErrorPopupHandler.Instance.OpenError(info);

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
