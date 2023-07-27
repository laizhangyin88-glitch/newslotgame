using UnityEngine;
using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker.Json;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class CouponRedeem : ActionTask<Blackboard>
{
    public BBParameter<string> code;

    protected override string info 
    {
        get 
        {
            return string.Format("Redeem Coupon");
        }
    }
    
    protected override void OnExecute()
    {
        if (code.value != null)
        {
            BagelCodeClientAPI.CouponRedeem(code.value,
            (response) =>
            {
                ClientAPI2Blackboard.Serialize(agent, response);
                
                Variable<string> image = BlackboardUtils.GetOrCreateVariable<string>(MainBlackboard.Get(), "RedeemResultImage");
                image.value = response.coupon.resultImageUrl;

                EndAction(true);
            },
            (error) =>
            {
                switch (error.errorCode)
                {
                    case BagelCode.ClientModels.Error.ALREADY_USED_COUPON_ERROR:
                        {
                            Variable<string> errorStringVar = BlackboardUtils.GetOrCreateVariable<string>(MainBlackboard.Get(), "RedeemErrorString");
                            errorStringVar.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_ALREADY_USED_COUPON");
                        }
                        break;
                    case BagelCode.ClientModels.Error.COUPON_EXPIRED_ERROR:
                        {
                            Variable<string> errorStringVar = BlackboardUtils.GetOrCreateVariable<string>(MainBlackboard.Get(), "RedeemErrorString");
                            errorStringVar.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_COUPON_EXPIRED");
                        }
                        break;
                    case BagelCode.ClientModels.Error.COUPON_CODE_ALREADY_EXIST_ERROR:
                    case BagelCode.ClientModels.Error.NOT_EXIST_COUPON_CODE_ERROR:
                        {
                            Variable<string> errorStringVar = BlackboardUtils.GetOrCreateVariable<string>(MainBlackboard.Get(), "RedeemErrorString");
                            errorStringVar.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_COUPON_DEFAULT");
                        }
                        break;
                    case BagelCode.ClientModels.Error.NOT_ELIGIBLE_COUPON_ERROR:
                        {
                            Variable<string> errorStringVar = BlackboardUtils.GetOrCreateVariable<string>(MainBlackboard.Get(), "RedeemErrorString");
                            errorStringVar.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_COUPON_NOT_ELIGIBLE");
                        }
                        break;
                    default:
                        // because background works
                        // GlobalErrorHandler.GlobalError(error);       
                        break;
                }
                EndAction(true);
            });
        }
        else
        {
            EndAction(true);
        }
    }
}

}
