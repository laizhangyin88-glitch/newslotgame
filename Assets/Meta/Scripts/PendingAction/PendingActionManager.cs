using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BagelCode.ClientModels;
using SlotMaker;
using SlotMaker.Json;
using NodeCanvas.Framework;

namespace BagelCode
{

public class PendingActionManager : MonoWeakSingleton<PendingActionManager>
{
    public void TestPushPendingAction(DeepLinkData dld)
    {
        byte[] deepLinkData = dld.Serialize();
        string base64EncodedStr = System.Convert.ToBase64String(deepLinkData);
        PushPendingAction( base64EncodedStr );
    }

    public void PushPendingAction(string data)
    {
        if (string.IsNullOrEmpty(data)) return;
            Debug.Log("PushPendingAction : " + data);

        byte[] parsedData = System.Convert.FromBase64String(data);
        DeepLinkData dld = DeepLinkData.Deserialize(parsedData);

        string couponCode = "";

        if(dld.action != null)
        {
            byte[] deepLinkData = dld.action.Serialize();
            string actionBase64EncodedStr = System.Convert.ToBase64String(deepLinkData);

            OpenUrl( ProductSettings.Instance.deeplinkUriScheme + "://adjust?action=" + actionBase64EncodedStr);

            if(dld.action.type == ActionType.COUPON_REDEEM)
            {
                var couponData = dld.action.data as ActionDataCouponRedeem;
                couponCode = couponData != null ? couponData.code : "";
            }
        }

        SetPushBIData(dld.type, dld.comment, dld.ts, dld.templateId, couponCode);
    }

    public void OnLocalPushMessage(string message)
    {
        var tokens = message.Split(':');

        SetPushBIData(tokens[0], tokens[1], System.Convert.ToInt64(tokens[2]), "", "");
    }

    public void SetPushBIData(string type, string comment, long ts, string templateId, string couponCode)
    {
        BlackboardQueryUtils.SetPushNoticeData(type, comment, ts, templateId, couponCode);
    }
    
    public void OpenUrl(string uri)
    {
        if (string.IsNullOrEmpty(uri)) return;

        Debug.Log("OpenUrl : " + uri);

        PendingActionUriOperator uriOperator = new PendingActionUriOperator();

        if(uriOperator.ParseUri(uri))
        {
            uriOperator.Operate();
        }
    }
}

}
