using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]

public class RequestLuckySpinCollectCoin : ActionTask <Blackboard>
{
    public BBParameter<long> earnCredit;
    
    protected override string info
    { 
        get 
        { 
            return string.Format("Request Lucky Spin Collect Coin");
        } 
    }
    protected override void OnExecute()
    {
        BagelCodeClientAPI.LuckySpinCollectCoin(
        (response) =>
        {
            ClientAPI2Blackboard.Serialize(agent, response);
            BlackboardQueryUtils.AddCoins(earnCredit.value);

            EndAction(true);
        },
        (error) =>
        {
            switch(error.errorCode)
            {
                case ClientModels.Error.INVALID_COLLECTING_COIN_ERROR:
                    {
                        bool stringError = false;
                        ErrorPopupInfo info = new ErrorPopupInfo();

                        info.type = ErrorPopupType.OK;
                        info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_INVALID_COLLECTING_COIN", out stringError);
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
