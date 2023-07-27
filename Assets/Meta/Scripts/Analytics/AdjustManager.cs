using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Services;
using SlotMaker;
using SlotMaker.Json;

namespace BagelCode
{

public class AdjustManager : MonoWeakSingleton<AdjustManager>
{
    private const string PREV_ADJUST_DATA_KEY = "PREV_ADJUST_DATA";
    private float repeatDelay = 60f;

    private static Dictionary<string, string> eventTokenDict = new Dictionary<string, string>();
    // {
    //     {"fb_connect", "i3eeq8"},
    //     {"email_connect", "klq0y8"},
    //     {"apple_connect", "etqmfi"},
    //     {"level_up:02", "7becvs"},
    //     {"level_up:03", "m3uyd8"},
    //     {"level_up:05", "icbpet"},
    //     {"level_up:10", "qdk3fu"},
    //     {"level_up:20", "nr2qku"},
    //     {"level_up:30", "5o5obh"},
    //     {"level_up:40", "bm27h9"},
    //     {"level_up:50", "jpeh28"},
    //     {"level_up:60", "ett5wk"},
    //     {"level_up:70", "7pd34w"},
    //     {"level_up:80", "jcyy3g"},
    //     {"level_up:90", "5k3tpm"},
    //     {"level_up:100", "apyckg"},
    //     {"level_up:110", "lu0san"},
    //     {"level_up:120", "bu6j5h"},
    //     {"level_up:130", "spcrsa"},
    //     {"level_up:140", "82hoth"},
    //     {"level_up:150", "a73iql"},
    //     {"level_up:160", "n1btpn"},
    //     {"level_up:170", "7wv1tq"},
    //     {"level_up:180", "t9d6r2"},
    //     {"level_up:190", "2p2g6g"},
    //     {"level_up:200", "1u1c3x"},
    //     {"lobby", "85a4qa"},
    //     {"login", "tr71xi"},
    //     {"purchase", "4wyt6l"},
    //     {"purchase_1", "cut2cp"},
    //     {"purchase_2", "pwkvhb"},
    //     {"purchase_3", "p0ij2i"},
    //     {"purchase_4", "wh2aso"},
    //     {"purchase_5", "mbl15m"},
    //     {"purchase_6", "eb5xua"},
    //     {"first_purchase", "bdmr3e"},
    //     {"slot_enter", "43b2mc"},
    //     {"spin_funnel:001", "u76lbu"},
    //     {"spin_funnel:002", "vhr42b"},
    //     {"spin_funnel:003", "m32fss"},
    //     {"spin_funnel:005", "a5omia"},
    //     {"spin_funnel:010", "9au510"},
    //     {"spin_funnel:020", "h01ev4"},
    //     {"spin_funnel:050", "gdq0xt"},
    //     {"spin_funnel:100", "6s2ney"},
    //     {"tier_up:01", "fv0gcx"},
    //     {"tier_up:02", "8mvzbh"},
    //     {"tier_up:03", "pzyzk6"},
    //     {"tier_up:04", "u9yfsn"},
    //     {"tier_up:05", "iztj0c"},
    //     {"tier_up:06", "bjmy03"},
    //     {"tier_up:07", "5uv5hk"},
    //     {"tier_up:08", "ku5oq9"},
    //     {"tier_up:09", "u1iys7"},
    //     {"tier_up:10", "3v0qrk"}
    // };

    protected override void OnDestroy()
    {
        base.OnDestroy();

        CancelInvoke();
    }

    public void Initialize(Dictionary<string, string> newEventTokenDict)
    {
        eventTokenDict = newEventTokenDict;

        if(PlayerPrefs.HasKey(PREV_ADJUST_DATA_KEY))
        {
            OnAttributionCallback(PlayerPrefs.GetString(PREV_ADJUST_DATA_KEY, ""));
        }

        NativeHelper.Instance.InitAdjust(AdjustManager.Instance.gameObject.name, "OnAttributionCallback");
    }

    public void SendEvent(string eventID)
    {
        if(eventTokenDict.ContainsKey(eventID))
        {
            string eventToken = eventTokenDict[eventID];
            NativeHelper.Instance.SendAdjustEvent(eventID, eventToken);
        }
    }

    // from ThirdPartyAnalyticsManager.
    public void SendPurchaseEvent(Blackboard productBB, Blackboard purchaseResponseBB)
    {
        //TODO: Purchase ID as String
        double productPrice = productBB.GetValue<double>("price");
        int productID = purchaseResponseBB.GetValue<int>("purchaseId");

        SendRevenueEvent("purchase", productPrice, productID.ToString());
    }

    private void SendRevenueEvent(string eventID, double revenue, string purchaseID)
    {
        if(eventTokenDict.ContainsKey(eventID)) {
            string eventToken = eventTokenDict[eventID];
            NativeHelper.Instance.SendAdjustRevenueEvent(eventID, eventToken, revenue, purchaseID);
        }
    }

    public void OnAttributionCallback(string json)
    {
        if (!string.IsNullOrEmpty(json))
        {
            PlayerPrefs.SetString(PREV_ADJUST_DATA_KEY, json);

            CancelInvoke();
            InvokeRepeating("RepeatSendAdjustEvent", repeatDelay, repeatDelay);

            SendAdjust(json);
        }
    }

    public void SendAdjust(string json)
    {
        var dict = SlotSimpleJson.DeserializeObject<Dictionary<string, object>>(json);

        BagelCodeClientAPI.Adjust(dict,
        (response) =>
        {
            if (PlayerPrefs.HasKey(PREV_ADJUST_DATA_KEY))
                PlayerPrefs.DeleteKey(PREV_ADJUST_DATA_KEY);

            var trackerName = FindProperty(dict, "trackerName");
            if(!string.Equals(trackerName, ""))
            {
                var trackerNameBB = BlackboardUtils.GetOrCreateVariable<string>(MainBlackboard.Get(), "trackerName");
                trackerNameBB.value = trackerName;
            }
        },
        (error) =>
        {
            Debug.Log("[AdjustManager]Error Occur");
        });
    }

    private void RepeatSendAdjustEvent()
    {
        if(PlayerPrefs.HasKey(PREV_ADJUST_DATA_KEY))
        {
            string jsonData = PlayerPrefs.GetString(PREV_ADJUST_DATA_KEY, "");

            if(!string.IsNullOrEmpty(jsonData))
            {
                SendAdjust(jsonData);
            }
            else
            {
                PlayerPrefs.DeleteKey(PREV_ADJUST_DATA_KEY);
                CancelInvoke();
            }
        }
        else
        {
            CancelInvoke();
        }
    }

    static string FindProperty(Dictionary<string, object> dict, string key, string nullValue = "")
    {
        object value;
        if (dict.TryGetValue(key, out value))
            return (value != null) ? value.ToString() : nullValue;
        return nullValue;
    }
}

}
