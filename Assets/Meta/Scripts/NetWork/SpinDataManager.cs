using ParadoxNotion;
using SimpleJSON;
using SlotMaker;
using System;
using System.Collections;
using UnityEngine;

public class SpinDataManager : MonoSingleton<SpinDataManager>
{
    private string _advanceSpinRes;

    public string advanceSpinRes
    {
        get
        {
            return _advanceSpinRes;
        }
        set
        {
            _advanceSpinRes = value;
            if (!string.IsNullOrEmpty(value))
            {
                OnAdvanceSpinRes(value);
            }
        }
    }

    private bool _isAdvanceSpinRes;
    public bool isAdvanceSpinRes
    {
        get {
            return _isAdvanceSpinRes && !string.IsNullOrEmpty(advanceSpinRes);
        }
        set {
            _isAdvanceSpinRes = value;
        }
    }

    public void OnAdvanceSpinRes(string res)
    {
        //Debug.Log("提前获取spin数据");
        JSONNode nod = JSONNode.Parse(res);
        if (nod.HasKey("game_result") && nod["game_result"].HasKey("jackpot_info"))
        {
            //game_result/jackpot_info/jackpot_reward1
            /*for (int i =0; i<6; i++)
            {
                string keyName = $"jackpot_reward{i}";

                if (nod["game_result"]["jackpot_info"].HasKey(keyName))
                {
                    string eventName = $"Jackpot{i}";
                    MessageDispatcher.Dispatch("OnContentEvent",
                        new EventData<long>(eventName, (long)nod["game_result"]["jackpot_info"][keyName]));
                }
            }*/

            for (int i = 1; i <= 3; i++)
            {
                string keyName1 = $"jackpot_reward{i}";
                string keyName2 = $"jackpot{i}";
                if (nod["game_result"]["jackpot_info"].HasKey(keyName1))
                {
                    string eventName = $"Jackpot{i}";
                    MessageDispatcher.Dispatch("OnContentEvent",
                        new EventData<long>(eventName, (long)nod["game_result"]["jackpot_info"][keyName1]));
                }else if (nod["game_result"]["jackpot_info"].HasKey(keyName2))
                {
                    string eventName = $"Jackpot{i}";
                    MessageDispatcher.Dispatch("OnContentEvent",
                        new EventData<long>(eventName, (long)nod["game_result"]["jackpot_info"][keyName2]));
                }
            }
        }
    }

    public void getSpinData(Action<JSONNode> responseCallback)
    {
        string res = _advanceSpinRes;
        _advanceSpinRes = "";
        StartCoroutine(_getResponseData(res, responseCallback));
    }

    private IEnumerator _getResponseData(string resStr, Action<JSONNode> responseCallback)
    {
        //string spinRes = TestManager.Instance.getSpin();
        yield return new WaitForSeconds(0.2f);

        try
        {
            SimpleJSON.JSONNode dataDict = SimpleJSON.JSONNode.Parse(resStr as string);
            SimpleJSON.JSONNode res = dataDict.HasKey("protocol_key") ? dataDict["data"] : dataDict;
            if (responseCallback != null)
            {
                responseCallback(res);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"【ERR】 data = {resStr}");
            Debug.LogException(e);
        }

    }

}
