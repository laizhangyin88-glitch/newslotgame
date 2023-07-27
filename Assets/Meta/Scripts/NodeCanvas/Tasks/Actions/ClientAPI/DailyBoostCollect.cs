using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;


namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class DailyBoostCollect : ActionTask <Blackboard> 
{
    protected override string info 
    {
        get 
        {
            return "Request Daily Boost Collect";
        }
    }

    protected override void OnExecute()
    {
        BagelCodeClientAPI.DailyBoostCollect(
        (response) =>
        {
            BlackboardUtils.DestroyBlackboard(MainBlackboard.Get(), "dailyBoostResponse");
            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "dailyBoostResponse");

            ClientAPI2Blackboard.Serialize(bb, response);

            BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
            BlackboardQueryUtils.ApplyDailyBoostInfo(response.dailyBoost);
            BlackboardQueryUtils.UpdateDailyBoostState();
            BlackboardQueryUtils.AddCoins(response.earnCredit);
            BlackboardQueryUtils.AddGems(response.earnGem);
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

                        EndAction(false);
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
