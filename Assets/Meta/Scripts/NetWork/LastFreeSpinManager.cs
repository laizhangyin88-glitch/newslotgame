using BagelCode;
using BagelCode.ClientModels;
using Dreamteck.Splines.Primitives;
using Newtonsoft.Json.Bson;
using PlayFab;
using SimpleJSON;
using Sirenix.OdinInspector;
using SlotMaker;
using SlotMaker.Slots.Tasks.Actions.Game;
using System;
using System.Collections;
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
        //getHis();
    }



    Dictionary<int, List<string>> his = new Dictionary<int, List<string>>
    {
        { 21,
            new List<string>{
                "test_21_slot_spin_0",
                "test_21_slot_spin_f1",
                "test_21_slot_spin_f2",
                "test_21_slot_spin_f3",
            }
        },
        { 93,
            new List<string>{
                //"test1_93_slot_spin_0",
                //"test1_93_claim_bonus",
                //"test1_93_slot_spin_f1",
                //"test1_93_slot_spin_f2",
                //"test1_93_slot_spin_f3",


                "test2_93_slot_spin_0",
                "test2_93_claim_bonus_1",
                "test2_93_slot_spin_f_2",
                "test2_93_slot_spin_f_3",
                "test2_93_claim_bonus_4",
                "test2_93_slot_spin_f_5",
                "test2_93_claim_bonus_6",
                "test2_93_slot_spin_f_7",
                "test2_93_claim_bonus_8",
                "test2_93_slot_spin_f_9",
                "test2_93_slot_spin_f_10",
                "test2_93_claim_bonus_11",
                "test2_93_slot_spin_f_12",
                "test2_93_slot_spin_f_13",
                "test2_93_slot_spin_f_14",
                "test2_93_slot_spin_f_15",
                "test2_93_claim_bonus_16",
            }
        },
    };



    public List<string> hisRes = new List<string>();
    void getHis(int id)
    {
        bool isOK = true;


        hisRes = new List<string>();

        for (int i =0; i< his[id].Count; i++)
        {
            TextAsset jsn8 = Resources.Load<TextAsset>(his[id][i]);
            if (jsn8 != null && jsn8.text != null)
            {
                hisRes.Add(jsn8.text);
            }
            else
            {
                isOK = false;
                Debug.LogError($"【FreeSpinManager】：找不到文件 {his[id][i]}");
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
        getHis(globalStore.nowGameID);
        //BlackboardUtils.SetOrCreateValue(ContentBlackboard.Get(), "isLastFreeSpin", true);
       // Debug.Log($"【FreeSpinManager】： isLastFreeSpin = {BlackboardUtils.GetOrCreateVariable<bool>(ContentBlackboard.Get(), "isLastFreeSpin").value}");
    }


    public bool isLastFreeSpin
    {
        get { return BlackboardUtils.GetOrCreateVariable<bool>(ContentBlackboard.Get(), "isLastFreeSpin").value; }
    }


    public  void getResponseData(string rpc,Action<JSONNode> responseCallback)
    {
        if (responseCallback != null)
        {
            string res = hisRes[0];
            Debug.Log($"【LastFreeSpin】 : {rpc} = {res}");
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


    [Button]
    public void GetFreeSpinHistory(string spin)
    {

        StartCoroutine(get00());
    }


    private IEnumerator get00()
    {

        bool isFinish = false;
        int i = 1;
        bool isNext = false;
        while (!isFinish)
        {

            isNext = false;
            NetManager.Instance.Post(RPCName.freeSpinHistory, new Dictionary<string, object>{{ "spin_index",i} },
            (res) =>
            {

                string resStr = res.ToString();

                isNext = true;

            },
            (error) =>
            {
                isFinish = true;
            });
            i++;
            yield return new WaitUntil(()=> isNext);
        }

    }


}
