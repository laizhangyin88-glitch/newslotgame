using UnityEngine;
using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using SlotMaker.Json;
using BagelCode.ClientModels;
using System.Runtime.InteropServices.WindowsRuntime;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class RedeemProduct : ActionTask <Blackboard> 
{
    public BBParameter<string> valueA;

    [BlackboardOnly]
    public BBParameter<bool> isSuccess;

    protected override string info 
    {
        get 
        {
            return string.Format("Request Redeem Product {0}", valueA);
        }
    }
    
    protected override void OnExecute()
    {
#if NEW_NET
           // isSuccess.value = true;
            EndAction(true);
            return;
#endif


        var productId = BlackboardUtils.FindVariable<int>(agent, string.Format("{0}/id", valueA.value) );

        if (productId != null)
        {
            BagelCodeClientAPI.RedeemBonus(productId.value,
            (response) =>
            {
                BlackboardUtils.DestroyBlackboard(MainBlackboard.Get(), "purchaseResponse");
                var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "purchaseResponse");

                ClientAPI2Blackboard.Serialize(bb, response);
                BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                BlackboardQueryUtils.ApplyPurchaseItems(response.itemUseResultList, response.userSyncInfo);

                isSuccess.value = true;
                EndAction(true);
            },
            (error) =>
            {
                switch(error.errorCode)
                {
                    case ClientModels.Error.DAILY_BOOST_ALREADY_EXIST_ERROR:
                        {
                            bool stringError = false;
                            ErrorPopupInfo info = new ErrorPopupInfo();

                            info.type = ErrorPopupType.OK;
                            info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_DAILY_BOOST_ALREADY_EXIST", out stringError);
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
        else
        {
            Debug.LogError("No ItemId found." + agent.gameObject.name);
        }
    }
}

}
