using BagelCode;
using BagelCode.Tasks.Actions.BlackboardQuery;
using Newtonsoft.Json;
using NodeCanvas.Framework;
using NodeCanvas.Tasks.Actions;
using ParadoxNotion;
using SBoxApi;
using SimpleJSON;
using Sirenix.OdinInspector;
using SlotMaker;
using SlotMaker.Slots.Tasks.Actions.Game;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EventData = ParadoxNotion.EventData;

public class TestManager : MonoSingleton<TestManager>
{


    public GameObject inputCode;

    public GameObject inputList;

    public GameObject inputSpin;

    public GameObject inputAutoUrl;

    public GameObject textServer;

    public GameObject inputClaimBonus;

    public GameObject inputCustomSpinReq;



    private void Start()
    {
        if (inputAutoUrl != null)
        {
            inputAutoUrl.GetComponent<InputField>().text = PlayerPrefs.GetString("TestAutoUrl", "");
            Debug.Log($"【TestAutoUrl】1 = {PlayerPrefs.GetString("TestAutoUrl", "")}");
        }
        else
        {
            Debug.Log($"【TestAutoUrl】2 = {PlayerPrefs.GetString("TestAutoUrl", "")}");
        }
    }
    public string getSpin()
    {
        if (inputSpin == null)
            return "";

        string res = inputSpin.GetComponent<InputField>().text ?? "";
        inputSpin.GetComponent<InputField>().text = "";
        return res;
    }



    public string getClaimBonus()
    {
        if (inputClaimBonus == null)
            return "";

        string res = inputClaimBonus.GetComponent<InputField>().text ?? "";
        inputClaimBonus.GetComponent<InputField>().text = "";
        return res;
    }



    public int getCode()
    {
        if (inputCode == null)
            return 0;

        string res = inputCode.GetComponent<InputField>().text ?? "";
        inputCode.GetComponent<InputField>().text = "";

        if (res == "")
            return 0;
        return int.Parse(res);
    }
    public int[] getList()
    {
        if (inputList == null)
            return new int[] { };

        string lstStr = inputList.GetComponent<InputField>().text ?? "";
        inputList.GetComponent<InputField>().text = "";
        string[] lstStrs = lstStr.Replace(" ", "").Split(',') ?? new string[] { };

        List<int> temp = new List<int>();
        for (int i = 0; i < lstStrs.Length; i++)
        {
            if (lstStrs[i] != "" && lstStrs[i] != null)
            {
                temp.Add(int.Parse(lstStrs[i]));
            }
        }

        return temp.ToArray();
    }

    public string getAutoUrl()
    {
        if (inputAutoUrl == null)
            return "";
        String res = inputAutoUrl.GetComponent<InputField>().text ?? "";
        PlayerPrefs.SetString("TestAutoUrl", res);
        return res;
    }


    public string getCustomSpinReq()
    {
        if (inputCustomSpinReq == null)
            return "";

        string res = inputCustomSpinReq.GetComponent<InputField>().text ?? "";
        inputCustomSpinReq.GetComponent<InputField>().text = "";
        return res;
    }

    public bool isCustomSpinReq
    {
        get
        {
            if (inputCustomSpinReq == null)
                return false;
            string text = inputCustomSpinReq.GetComponent<InputField>().text ?? "";
            return text != null && text != "";
        }
    }

    [HideInInspector]
    public string customSpinRes;

    public bool isCustomSpinRes
    {
        get
        {
            return customSpinRes != null && customSpinRes != "";
        }
    }

    public void getCustomSpinRes(Action<JSONNode> responseCallback)
    {
        string res = customSpinRes;
        customSpinRes = "";
        StartCoroutine(_getResponseData(res, responseCallback));
    }





    public bool isTestSpin
    {
        get
        {
            if (inputSpin == null)
                return false;
            string text = inputSpin.GetComponent<InputField>().text ?? "";
            return text != null && text != "";
        }
    }


    public bool isTestClaimBonus
    {
        get
        {
            if (inputClaimBonus == null)
                return false;
            string text = inputClaimBonus.GetComponent<InputField>().text ?? "";
            return text != null && text != "";
        }
    }

    public void getClaimBonusData(Action<JSONNode> responseCallback)
    {
        StartCoroutine(_getResponseData(TestManager.Instance.getClaimBonus(), responseCallback));
    }


    public void getSpinData(Action<JSONNode> responseCallback)
    {
        StartCoroutine(_getResponseData(TestManager.Instance.getSpin(), responseCallback));
    }


    private IEnumerator _getResponseData(string resStr, Action<JSONNode> responseCallback)
    {
        //string spinRes = TestManager.Instance.getSpin();
        yield return new WaitForSeconds(0.2f);

        SimpleJSON.JSONNode dataDict = SimpleJSON.JSONNode.Parse(resStr as string);
        SimpleJSON.JSONNode res = dataDict["data"];
        if (responseCallback != null)
        {
            responseCallback(res);
        }
    }

    public void SetTextServer(string text)
    {
        if (textServer == null)
            return;
        textServer.GetComponent<Text>().text = text.Replace("https://", "").Replace("http://", "");
    }





    [Button]
    void test_LastFreeSpin()
    {
        //EventSender.SendGlobalEvent("OnLastFreeSpin");

        BlackboardUtils.GetOrCreateVariable<bool>(MainBlackboard.Get(), "isLastGameSpin").value = true;
    }


    /*
    [Button] // 给BB赋值
    void test_CreatField01()
    {
        BlackboardUtils.GetOrCreateVariable<bool>(null, "/isTest01").value = true;
    }
    [Button]
    void test_CreatField02()
    {
        BlackboardUtils.GetOrCreateVariable<bool>(MainBlackboard.Get(), "isTest02").value = true;
    }
    
    [Button]//关闭login mask 弹窗
    void test_CloseLoginMaskPop()
    {
        EventSender.SendGlobalEvent("OnCloseLoginMaskPop");
    }*/




    [Button]
    void test_timeEq10()
    {
        Time.timeScale = 10;
    }


    [Button]
    void test_timeEq1()
    {
        Time.timeScale = 1;
    }





    [Button]
    void test_GetCoinOutOrder()
    {
        Dictionary<string, object> req = new Dictionary<string, object> { };
        //Debug.Log("请求投币");
        NetManager.Instance.Post(RPCName.createCoinOutOrder, req,
        (res) =>
        {
            Debug.Log($" res = {res.ToString()}");
            Debug.Log($" 允许退票个数 = {res["money"]}");
        },
        (error) =>
        {
            Debug.LogError(" 查询投币个数失败");
        });
    }

}
