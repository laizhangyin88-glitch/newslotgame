
using BagelCode.ClientModels;
using Dreamteck.Splines.Primitives;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using NodeCanvas.Framework;
using ParadoxNotion;
using SimpleJSON;
using Sirenix.OdinInspector;
using SlotMaker;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Action = System.Action;
using EventData = ParadoxNotion.EventData;

public class TestManager : MonoSingleton<TestManager>
{


    public GameObject inputCode;

    public GameObject inputList;

    public GameObject inputSpin;

    public GameObject inputAutoUrl;

    public GameObject textServer;

    public GameObject inputClaimBonus;

    public GameObject inputCustomReels;

    public GameObject inputExcUI;

    public Toggle toggleCheckCredit;

    public GameObject inputCustomFlag; //k1:2#k2:3#k3:4

    public Toggle LobbyJackpot;
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

        if (toggleCheckCredit != null)
            toggleCheckCredit.isOn = true;

        StartCoroutine(CheckFlag());
    }

    Dictionary<string, string> flags = new Dictionary<string, string>();
    IEnumerator CheckFlag()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);
            string agrs = inputCustomFlag.GetComponent<InputField>().text ?? "";
            inputCustomFlag.GetComponent<InputField>().text = "";
            if (!string.IsNullOrEmpty(agrs))
            {
                string[] itemsStrs = agrs.Split('#') ?? new string[] { };  //k1:2#k2:3#k3:4

                foreach (string item in itemsStrs)
                {
                    string[] kv = item.Split(':') ?? new string[] { };
                    string key = kv[0];
                    string value = kv.Length > 1 ? kv[1] : "";

                    if (!flags.ContainsKey(key))
                    {
                        flags.Add(key, value);
                    }
                    else
                    {
                        flags[key] = value;
                    }
                }

                string res = "==@ [flags]";
                foreach (KeyValuePair<string, string> item in flags)
                {
                    res += $" {item.Key} : {item.Value};";
                }
                Debug.Log(res);
            }
        }
    }

    public bool HasFlag(string key)
    {
        return flags.ContainsKey(key);
    }

    public bool HasFlagOnce(string key)
    {
        bool isHas = flags.ContainsKey(key);
        flags.Remove(key);
        return isHas;
    }

    public string GetFlag(string key)
    {
        if (!flags.ContainsKey(key))
        {
            return "";
        }
        return flags[key];
    }
    public string GetFlagOnce(string key)
    {
        string res = "";
        if (flags.ContainsKey(key))
        {
            res = flags[key];
            //flags[key] = "";
            flags.Remove(key);
        }
        return res;
    }



    bool isAllOpen = true;

    public void OnTotalBtnClick()
    {
        isAllOpen = !isAllOpen;
        foreach (Transform chd in this.transform)
        {
            chd.gameObject.SetActive(isAllOpen);
        }
        transform.Find("TOTAL BTN").gameObject.SetActive(true);
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

    public string getExcUIConfig()
    {
        string res = null;
        if (inputExcUI == null)
        {
            res = "";
        }
        else
        {
            res = inputExcUI.GetComponent<InputField>().text ?? "";
            inputExcUI.GetComponent<InputField>().text = "";
        }

        if (res == "")
        {
            TextAsset jsn8 = Resources.Load<TextAsset>("ExcUI");
            res = PlayerPrefs.GetString("ExcUI", jsn8.text);
        }
        PlayerPrefs.SetString("ExcUI", res);
        PlayerPrefs.Save();

        return res;
    }

    public void ClearExcUI()
    {
        inputExcUI.GetComponent<InputField>().text = "";
        PlayerPrefs.DeleteKey("ExcUI");
        PlayerPrefs.Save();
    }


    public List<object> getCustomReels()
    {
        if (inputCustomReels == null)
            return null;

        string res = inputCustomReels.GetComponent<InputField>().text ?? "";
        inputCustomReels.GetComponent<InputField>().text = "";

        return _GetReelsContent(res);
    }



    public bool isCustomReels
    {
        get
        {
            if (inputCustomReels == null)
                return false;
            string text = inputCustomReels.GetComponent<InputField>().text ?? "";

            bool isOk = text != null && text != "";

            if (isOk)
            {
                Debug.LogWarning($"假滚轮数据 = {text}");
            }
            return isOk;
        }
    }


    private string _customReelsSpinRes;
    public string customReelsSpinRes
    {
        get
        {
            return _customReelsSpinRes;
        }
        set
        {
            _customReelsSpinRes = value;
            if (!string.IsNullOrEmpty(value))
            {
                SpinDataManager.Instance.OnAdvanceSpinRes(value);
            }
        }
    }


    public bool isCheckCredit
    {
        get
        {
#if !UNITY_EDITOR
            return true;
#endif
            if (toggleCheckCredit == null)
            {
                return true;
            }
            return toggleCheckCredit.isOn;
        }
    }



    public bool isCustomReelsSpinRes
    {
        get
        {
            return _customReelsSpinRes != null && _customReelsSpinRes != "";
        }
    }

    public void getCustomReelsSpinRes(Action<JSONNode> responseCallback)
    {
        string res = _customReelsSpinRes;
        _customReelsSpinRes = "";
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

        try
        {
            SimpleJSON.JSONNode dataDict = SimpleJSON.JSONNode.Parse(resStr as string);
            SimpleJSON.JSONNode res = dataDict.HasKey("protocol_key") ? dataDict["data"] : dataDict;
            //res.Add("is_custom_reels",true);
            JSONNode node = JSON.Parse("{}");
            node.Add("is_custom_reels", true);
            res.Add("user_config", node);
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



    [Button]
    void test_ShowReel()
    {

        List<Blackboard> reelSetList = BlackboardUtils.FindVariable<List<Blackboard>>(null, "./game/reelSetList").value;

        List<List<List<int>>> reelsLst = new List<List<List<int>>>();

        for (int i = 0; i < reelSetList.Count; ++i)
        {
            Debug.Log($" ==== reelSequenceList{i} : ");
            var reelSet = reelSetList[i];
            var reelSequenceList = reelSet.GetValue<List<Blackboard>>("reelSequenceList");

            List<List<int>> tempReels = new List<List<int>>();

            for (int j = 0; j < reelSequenceList.Count; ++j)
            {
                List<int> weights = reelSequenceList[j].GetValue<List<int>>("value");
                tempReels.Add(weights);
                string res = "";
                foreach (int item in weights)
                {
                    res += item;
                    res += ",";
                }
                Debug.Log($"单列滚轮 {GetHash(res)}  = {res}");
            }
            reelsLst.Add(tempReels);
        }
    }


    private static MD5 md5Hash = MD5.Create();
    private static string GetHash(string source)
    {
        if (string.IsNullOrEmpty(source)) return null;

        byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(source));
        StringBuilder sBuilder = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }
        return sBuilder.ToString();
    }



    [Button]
    void test_ChangeReel(string str = "2,3,4,5,6#2,3,4,5,6#2,3,4,5,6")
    {
        List<object> shuffling_list = _GetReelsContent(str);

        List<int> result = ChangeReel(shuffling_list);

        string res = "";
        foreach (int item in result)
        {
            res += item;
            res += ",";
        }
        Debug.Log($"每列显示首索引：{res}");
    }


    private List<object> _GetReelsContent(string str)
    {
        /*List<object> shuffling_list = new List<object>(){
            new List<object>(){8,5,7,2,3},
            new List<object>(){1,8,4,8,10},
            new List<object>(){7,1,9,4,5},
        };*/

        List<object> showReelsContent = new List<object>();

        str = str.Replace(" ", "");

        if (str.StartsWith("[")) //"[[8,5,7,2,3],[1,8,4,8,10],[7,1,9,4,5]]"
        {
            JSONNode node = JSONNode.Parse(str);
            for (int i = 0; i < node.Count; i++)
            {
                List<object> temp = new List<object>();
                for (int j = 0; j < node[i].Count; j++)
                {
                    temp.Add((int)node[i][j]);
                }
                showReelsContent.Add(temp);
            }
        }
        else //"8,5,7,2,3#1,8,4,8,10#7,1,9,4,5"
        {
            string[] lstStrs = str.Split('#') ?? new string[] { };
            for (int i = 0; i < lstStrs.Length; i++)
            {
                string[] itemsStrs = lstStrs[i].Split(',') ?? new string[] { };

                List<object> temp = new List<object>();
                for (int j = 0; j < itemsStrs.Length; j++)
                {
                    if (itemsStrs[j] != "" && itemsStrs[j] != null)
                    {
                        temp.Add(int.Parse(itemsStrs[j]));
                    }
                }
                showReelsContent.Add(temp);
            }
        }

        return showReelsContent;
    }

    List<List<List<int>>> reelsOldLst = null;

    public void ClearReelsCache()
    {
        reelsOldLst = null;
    }

    public List<int> ChangeReel(List<object> Show, Action<List<int>> cb = null)
    {
        /*List<object> Show = new List<object>(){
            new List<object>(){8,5,7,2,3},
            new List<object>(){1,8,4,8,10},
            new List<object>(){7,1,9,4,5},
        };*/

        //码表转换
        Variable<Dictionary<int, int>> changeCode = ContentBlackboard.Get().GetVariable<Dictionary<int, int>>("changeCode");
        if (changeCode != null && changeCode.value != null)
        {
            foreach (var item in changeCode.value)
            {
                foreach (List<object> raw in Show)
                {
                    for (int i = 0; i < raw.Count; i++)
                    {
                        if ((int)raw[i] == item.Key)
                        {
                            raw[i] = item.Value;
                        }
                    }
                }
            }
        }

        List<Blackboard> reelSetList = BlackboardUtils.FindVariable<List<Blackboard>>(null, "./game/reelSetList").value;

        List<List<List<int>>> reelsLst = new List<List<List<int>>>();

        for (int i = 0; i < reelSetList.Count; ++i)
        {
            //Debug.Log($" ==== reelSequenceList{i} : ");
            var reelSet = reelSetList[i];
            var reelSequenceList = reelSet.GetValue<List<Blackboard>>("reelSequenceList");

            List<List<int>> tempReels = new List<List<int>>();

            for (int j = 0; j < reelSequenceList.Count; ++j)
            {
                List<int> weights = reelSequenceList[j].GetValue<List<int>>("value");
                tempReels.Add(weights);
                /*
                string res = "";
                foreach (int item in weights)
                {
                    res += item;
                    res += ",";
                }
                Debug.Log(res);
                */
            }
            reelsLst.Add(tempReels);
        }

        if (reelsOldLst == null) //备份
        {
            Debug.Log("备份滚轮！");
            reelsOldLst = DeepCopy(reelsLst);  // new List<List<List<int>>>(reelsLst);
        }

        List<List<int>> reels0 = reelsLst[0]; //常规码表
        List<int> data = new List<int>();
        for (int i = 0; i < reels0.Count; i++)
        {
            data.Add(UnityEngine.Random.Range(0, reels0[i].Count - 1)); //产生每列的索引
        }

        // 修改常规码表
        for (int i = 0; i < data.Count; i++)
        {
            int k = data[i];
            for (int j = 0; j < Show.Count; j++)
            {
                int idx = k + j;
                if (idx >= reelsLst[0][i].Count) // i = 第i列  j = 第j行
                    idx -= reelsLst[0][i].Count;
                try
                {
                    reelsLst[0][i][idx] = (int)((Show[j] as List<object>)[i]);
                    //Debug.LogWarning($" 第{i}列 第{idx}行 = {reelsLst[0][i][idx]} ");
                }
                catch (Exception e)
                {
                    Debug.LogError($" reel{i}.Count = {reelsLst[0][i].Count} idx = {idx} i={i} j={j} {(int)((Show[j] as List<object>)[i])}");
                    Debug.LogError($" Err = {e}");
                }
            }
        }

        StartCoroutine(_SetReel(new List<List<List<int>>>(reelsLst),
        () =>
        {
            if (cb != null)
                cb(data);
        }));

        return data;
    }


    public void ResetReel(Action cb)
    {
        if (reelsOldLst != null)
        {
            List<List<List<int>>> temp = DeepCopy(reelsOldLst); // new List<List<List<int>>>(reelsOldLst);
            reelsOldLst = null;
            StartCoroutine(_SetReel(temp, cb));
            Debug.Log("复位滚轮！");
        }
        else
        {
            if (cb != null)
                cb();
        }
    }


    int ii = 0;

    IEnumerator _SetReel(List<List<List<int>>> reelsLst2, Action cb = null)
    {
        ii++;

        var gameBB = BlackboardUtils.GetOrCreateBlackboard(ContentBlackboard.Get(), "game");
        BlackboardUtils.DestroyBlackboardList(gameBB, "reelSetList");

        yield return new WaitUntil(() => gameBB.GetVariable<List<Blackboard>>("reelSetList") == null);

        BlackboardUtils.GetOrCreateBlackboardList(gameBB, "reelSetList");
        foreach (List<List<int>> reels in reelsLst2)
        {
            var reelSequenceListBB = BlackboardUtils.CreateBlackboard($"Object{ii}"); //Object
            BlackboardUtils.GetOrCreateBlackboardList(reelSequenceListBB, "reelSequenceList");
            foreach (List<int> reel in reels)
            {
                var reelBB = BlackboardUtils.CreateBlackboard("reel");  //List`1
                BlackboardUtils.SetOrCreateValue<List<int>>(reelBB, "value", reel);
                BlackboardUtils.AddToBlackboardList(reelSequenceListBB, "reelSequenceList", reelBB);
            }
            BlackboardUtils.AddToBlackboardList(gameBB, "reelSetList", reelSequenceListBB);
        }

        yield return SetReelStripsManager(cb);
    }

    IEnumerator SetReelStripsManager(Action cb = null)
    {
        /*
        GameObject go = GameObject.Find("ReelStrips Manager");
        for (int i = go.transform.childCount - 1; i>=0;i--)
        {
            //Destroy(go.transform.GetChild(i).gameObject);
            GameObject.DestroyImmediate(go.transform.GetChild(i).gameObject);
        }
        */


        GameObject goOld = GameObject.Find("ReelStrips Manager");
        GameObject.DestroyImmediate(goOld);

        //var mgr1 = goOld.AddComponent<GlobalReelStrips>();
        // mgr1.OnDestroy();

        yield return new WaitUntil(() => GameObject.Find("ReelStrips Manager") == null);

        var parent = ContentCustomData.Instance.transform;
        var go = new GameObject();
        go.name = "ReelStrips Manager";
        go.transform.SetParent(parent, false);



        var mgr = go.AddComponent<GlobalReelStrips>();

        mgr.stripsList = new List<ReelStrips>();

        var symbolMask = ContentCustomData.GetSlotData(0).symbolMask;

        var reelSetList = BlackboardUtils.FindVariable<List<Blackboard>>(null, "./game/reelSetList").value;
        for (int i = 0; i < reelSetList.Count; ++i)
        {
            go = new GameObject();
            go.name = "ReelStrips"; //滚轮区域
            go.transform.SetParent(mgr.transform, false);

            var reelStrips = go.AddComponent<ReelStrips>();
            reelStrips.reelStrips = new List<BaseReelStrip>();
            reelStrips.singleStrip = false;

            var reelSet = reelSetList[i];
            var reelSequenceList = reelSet.GetValue<List<Blackboard>>("reelSequenceList");
            for (int j = 0; j < reelSequenceList.Count; ++j)
            {
                go = new GameObject();
                go.name = "ReelStrip";  //单列滚轮
                go.transform.SetParent(reelStrips.transform, false);

                var reelStrip = go.AddComponent<ReelStrip>();
                reelStrip.stripIndex = j;
                reelStrip.strip = new List<SymbolInfo>();

                var indexList = reelSequenceList[j].GetValue<List<int>>("value");
                for (int k = 0; k < indexList.Count; ++k)
                {
                    reelStrip.strip.Add(SlotUtils.CreateSymbolInfo(indexList[k], symbolMask));
                }

                reelStrips.reelStrips.Add(reelStrip);
            }

            mgr.stripsList.Add(reelStrips);
        }

        var mark = new GameObject();
        mark.name = $"mark{ii}";
        mark.transform.SetParent(GameObject.Find("ReelStrips Manager").transform, false);

        if (cb != null)
            cb();
    }




    List<List<List<int>>> DeepCopy(List<List<List<int>>> original)
    {
        if (original == null)
        {
            return null;
        }

        List<List<List<int>>> copy = new List<List<List<int>>>(original.Count);
        foreach (var subList2D in original)
        {
            List<List<int>> subList2DCopy = new List<List<int>>(subList2D.Count);
            foreach (var subList1D in subList2D)
            {
                List<int> subList1DCopy = new List<int>(subList1D.Count);
                foreach (var item in subList1D)
                {
                    subList1DCopy.Add(item);
                }
                subList2DCopy.Add(subList1DCopy);
            }
            copy.Add(subList2DCopy);
        }

        return copy;
    }



    [Button]
    void test_GetReelCellByIndex(string indexs = "2,2,2,2,2")
    {
        string[] itemsStrs = indexs.Replace(" ", "").Split(',') ?? new string[] { };

        GameObject go = GameObject.Find("ReelStrips Manager");
        Transform reels = go.transform.GetChild(0);  //   [0]

        Dictionary<int, string> res = new Dictionary<int, string>();
        for (int i = 0; i < itemsStrs.Length; i++)
        {
            var item = itemsStrs[i];
            var reelStrip = reels.GetChild(i).GetComponent<ReelStrip>();
            for (int j = 0; j < 3; j++)
            {
                if (!res.ContainsKey(j))
                {
                    res.Add(j, "");
                }
                res[j] += $"{reelStrip.strip[int.Parse(item) + j].symbol},";
            }
        }
        foreach (var item in res)
        {
            Debug.Log($"==@{item.Value}");
        }
    }



    [Button]
    void test_ShowReels0Code()
    {
        GameObject go = GameObject.Find("ReelStrips Manager");
        Transform reels = go.transform.GetChild(0);

        for (int i = 0; i < reels.transform.childCount; i++)
        {
            var reelStrip = reels.GetChild(i).GetComponent<ReelStrip>();

            string res = "==@ {";
            for (int j = 0; j < reelStrip.strip.Count; j++)
            {
                res += "\"" + j + "\":" + reelStrip.strip[j].symbol + ",";
            }
            res += "}";
            res = res.Replace(",}", "}");
            Debug.Log(res);
        }
    }

    [Button]
    void test_ShowGlobalReelStripsInstance()
    {
        GameObject go = GameObject.Find("ReelStrips Manager");
        Transform reels = go.transform.GetChild(0);

        if (go.transform.childCount > 2)
        {
            var ch = go.transform.GetChild(2);
            Debug.LogError($"@@ i am GlobalReelStrips {ch.name}");
        }
        else
        {
            Debug.LogError($"@@ i am GlobalReelStrips !");
        }

        go = GlobalReelStrips.Instance.gameObject;

        if (go.transform.childCount > 2)
        {
            var ch = go.transform.GetChild(2);
            Debug.LogError($"@@ i am GlobalReelStrips {ch.name}");
        }
        else
        {
            Debug.LogError($"@@ i am GlobalReelStrips !");
        }

    }


    /*
    Dictionary<string, object> req = new Dictionary<string, object>(TestManager.Instance.spinAgrs);
    TestManager.Instance.spinAgrs = new Dictionary<string, object>();
    req.Add("bet", betCredit);
    req.Add("extra_bet", extraBetCredit);
    */

    public Dictionary<string, object> spinAgrs = new Dictionary<string, object>();

    [Button]
    void test_AddArg(string agrs)  // xxxx:xxx#
    {
        string[] itemsStrs = agrs.Replace(" ", "").Split('#') ?? new string[] { };

        spinAgrs = new Dictionary<string, object>();
        for (int i = 0; i < itemsStrs.Length; i++)
        {
            string[] res = itemsStrs[i].Split(':') ?? new string[] { "a", "-1" };
            spinAgrs.Add(res[0], int.Parse(res[1]));
        }
    }


    [Button]
    void test_responeNew()
    {

        var variableA = BlackboardUtils.FindVariable<Blackboard>(null, "./turn/spin");

        if (variableA != null)
            Debug.Log($" @@-1 = {variableA.value}");

        variableA = BlackboardUtils.FindVariable<Blackboard>(null, "./spin");

        if (variableA != null)
            Debug.Log($" @@0 = {variableA.value}");

        var variableA1 = BlackboardUtils.FindVariable<string>(null, "./spin/responseNew");

        if (variableA1 != null)
            Debug.Log($" @@1 = {variableA.value}");

        variableA1 = BlackboardUtils.FindVariable<string>(null, "./spin/response/responseNew");

        if (variableA1 != null)
            Debug.Log($" @@2 = {variableA.value}");
    }

    [Button]
    void test_ShowDoor()
    {
        Animator anim = GameObject.Find("Game Contents/Animator").GetComponent<Animator>();
        anim.SetTrigger("Door Appear");

        //MessageDispatcher.Dispatch("OnContentUIEvent", new EventData("Start Door"));
    }


    [Button]
    void test_ShowUI()
    {
        MessageDispatcher.Dispatch("OnContentUIEvent", new EventData("ShowUI"));
    }
    [Button]
    void test_HideUI()
    {
        MessageDispatcher.Dispatch("OnContentUIEvent", new EventData("HideUI"));
    }

    public bool GetLobbyJackpot()
    {
        if (LobbyJackpot != null)
        {
            return LobbyJackpot.isOn;
        }
        return false;
    }
}
