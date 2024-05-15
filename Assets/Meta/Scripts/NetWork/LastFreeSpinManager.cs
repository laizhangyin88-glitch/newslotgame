using Dreamteck.Splines.Primitives;
using Newtonsoft.Json.Bson;
using SimpleJSON;
using Sirenix.OdinInspector;
using SlotMaker;
using System;
using System.Collections.Generic;
using UnityEngine;


public class LastFreeSpinManager : MonoSingleton<LastFreeSpinManager> {

    /*
    private static FreeSpinManager instance;
    public static FreeSpinManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new FreeSpinManager();
            }
            return instance;
        }
    }*/

   void Start() {
        getHis();
    }


    List<string> his = new List<string>{
        "test1_93_slot_spin_0",
        "test1_93_claim_bonus",
        "test1_93_slot_spin_f1",
        "test1_93_slot_spin_f2",
        "test1_93_slot_spin_f3",
        //"g39__slot_spin__ko_1712582409715",
        //"g39__slot_spin__ko_1712583306724",
        //"g39__slot_spin__ko_1712583486348"
    };

    public List<string> hisRes = new List<string>();
    void getHis()
    {
        bool isOK = true;

        for (int i =0; i< his.Count; i++)
        {
            TextAsset jsn8 = Resources.Load<TextAsset>(his[i]);
            if (jsn8 != null && jsn8.text != null)
            {
                hisRes.Add(jsn8.text);
            }
            else
            {
                isOK = false;
                Debug.LogError($"【FreeSpinManager】：找不到文件 {his[i]}");
            }
        }

        if (isOK)
        {
            BlackboardUtils.SetOrCreateValue(ContentBlackboard.Get(), "isLastFreeSpin", true);
        }
        Debug.Log($"【FreeSpinManager】： isLastFreeSpin = {BlackboardUtils.GetOrCreateVariable<bool>(ContentBlackboard.Get(), "isLastFreeSpin").value}");
    }


    [Button]
    void test_getIsLastFreeSpin()
    {
        Debug.Log($"【FreeSpinManager】： isLastFreeSpin = { BlackboardUtils.GetOrCreateVariable<bool>(ContentBlackboard.Get(), "isLastFreeSpin").value}");
    }

    [Button]
    void test_setIsLastFreeSpin()
    {
        BlackboardUtils.SetOrCreateValue(ContentBlackboard.Get(), "isLastFreeSpin", true);
        Debug.Log($"【FreeSpinManager】： isLastFreeSpin = {BlackboardUtils.GetOrCreateVariable<bool>(ContentBlackboard.Get(), "isLastFreeSpin").value}");
    }


    public bool isLastFreeSpin
    {
        get { return BlackboardUtils.GetOrCreateVariable<bool>(ContentBlackboard.Get(), "isLastFreeSpin").value; }
    }


    public  void getResponseData(Action<JSONNode> responseCallback)
    {
        if (responseCallback != null)
        {
            string res = hisRes[0];
            SimpleJSON.JSONNode dataDict = SimpleJSON.JSONNode.Parse(res as string);

            hisRes.RemoveAt(0);
            if (hisRes.Count == 0) //最后一局不放慢
            {
               // Time.timeScale = 1;
                BlackboardUtils.SetOrCreateValue(ContentBlackboard.Get(), "isLastFreeSpin", false);
            }
            else
            {
               // Time.timeScale = 10;
            }
            responseCallback(dataDict["data"]);
        }
    }




}
