using BagelCode;
using BagelCode.ClientModels;
using BagelCode.Tasks.Actions.ClientAPI;
using Dreamteck.Splines.Primitives;
using GameUtil;
using Newtonsoft.Json.Bson;
using NodeCanvas.Framework;
using ParadoxNotion;
using PlayFab;
using SimpleJSON;
using Sirenix.OdinInspector;
using SlotMaker;
using SlotMaker.Slots;
using SlotMaker.Slots.Tasks.Actions.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using static GameUtil.Timer;
using Action = System.Action;

public class LastFreeGameManager : MonoSingleton<LastFreeGameManager>
{
    /*
    private static LastFreeGameManager instance;
    public static LastFreeGameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LastFreeGameManager();
            }
            return instance;
        }
    }*/
    private DelayTimer _delayTimer;
    private LoginMaskController _loginMaskController;

    void Start()
    {
        _loginMaskController = FindObjectOfType<LoginMaskController>();
    }


    Dictionary<int, List<string>> test_his = new Dictionary<int, List<string>>
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


    Coroutine _task = null;
    public void DoTask(Action cb, int ms = 0)
    {
        ClearTask();
        _task = StartCoroutine(doTask(cb, ms));
    }
    public void ClearTask()
    {
        //StopCoroutine("doTask");
        if (_task != null)
        {
            StopCoroutine(_task);
            _task = null;
        }
    }
    IEnumerator doTask(Action cb, int ms = 0)
    {
        yield return new WaitForSeconds(ms / 1000f);
        if (cb != null)
            cb();
        _task = null;
    }


    Coroutine _StartLastFreeSpin = null;
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (MachineSelectManager.Instance.isPopCommon())
            {
                DoTask(() =>
                {
                    //ConfirmPopCommon();

                    if (!MachineSelectManager.Instance.isPopCommon())
                        return;
                    MachineSelectManager.Instance.ConfirmPopCommon();
                }, 1000);
            }
            if (MachineSelectManager.Instance.IsNodeMiniGame())
            {
                DoTask(() =>
                {
                    if (!MachineSelectManager.Instance.IsNodeMiniGame())
                        return;
                    MachineSelectManager.Instance.ConfirmNodeMiniGameSpin();
                }, 1000);
            }

        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (MachineSelectManager.Instance.isPopCommon())
                Debug.LogError("PopCommon");
            if (MachineSelectManager.Instance.isPopFreeGameTimeSelect())
                Debug.LogError("PopFreeGameTimeSelect");
            if (MachineSelectManager.Instance.IsNodeMiniGame())
                Debug.LogError("NodeMiniGame");
        }

        if (globalStore.nowGameID != -1 && _isLastGameSpin)
        {

            if (isStartLastFreeSpin == false)
            {
                if (_StartLastFreeSpin != null)
                {
                    StopCoroutine(_StartLastFreeSpin);
                }
                _StartLastFreeSpin = StartCoroutine(StartLastFreeSpin());
            }

            if (_task == null)
            {
                if (MachineSelectManager.Instance.isPopFreeGameTimeSelect())
                {
                    DoTask(() =>
                    {
                        ConfirmPopFreeGameSelect();
                    }, 1000);
                }/*
                  else if (MachineSelectManager.Instance.isNodeMiniGameSelect()){

                }*/
                else if (MachineSelectManager.Instance.IsNodeMiniGame())
                {
                    DoTask(() =>
                    {
                        if (!MachineSelectManager.Instance.IsNodeMiniGame())
                            return;
                        MachineSelectManager.Instance.ConfirmNodeMiniGameSpin();
                    }, 1000);
                }
                else if (MachineSelectManager.Instance.isNodeMiniGameSelect())
                {
                    DoTask(() =>
                    {
                        if (!MachineSelectManager.Instance.isNodeMiniGameSelect())
                            return;

                        NodeMiniGameAutoSelect();
                    }, 1000);
                }
                else if (MachineSelectManager.Instance.isPopCommon())
                {
                    DoTask(() =>
                    {
                        //ConfirmPopCommon();

                        if (!MachineSelectManager.Instance.isPopCommon())
                            return;
                        MachineSelectManager.Instance.ConfirmPopCommon();
                    }, 1000);
                }
            }

        }

    }


    public FirstSpinInfo FreeSpinInfo { get; private set; }


    bool isStartLastFreeSpin = false;
    private IEnumerator StartLastFreeSpin()
    {
        if (historyRes.Count <= 0) yield break;
        isStartLastFreeSpin = true;
        // 获取第一包spin的数据
        FreeSpinInfo = new FirstSpinInfo(historyRes);

        if (FreeSpinInfo.betCredit == 0)
        {
            Debug.LogError($"找不到bet_credit  数据 = {historyRes[0]}");
            yield break;
        }


        // 设置押注倍数
        InGameBetController IGBC = null;
        VideoPokerHandController videoPokerHandController = null;

        while (IGBC == null)
        {
            IGBC = GameObject.Find("In Game/Anchor/In Game Bottom/Anchor/Layout/Bet")?.GetComponent<InGameBetController>();
            if (IGBC == null)
            {
                IGBC = GameObject.Find("In Game/Anchor/In Game Bottom/Anchor/Layout/Left/Bet")?.GetComponent<InGameBetController>();///竖屏游戏的bet路径
            }

            if (videoPokerHandController == null)
            {
                videoPokerHandController = GameObject.Find("In Game/Anchor/In Game Bottom/Anchor/Layout/Button Hands")?.GetComponent<VideoPokerHandController>();
                if(videoPokerHandController != null && videoPokerHandController.gameObject.active)
                {
                    videoPokerHandController.UpdateHandsGroupIndex(FreeSpinInfo.handCount);
                }
            }
            yield return new WaitForSeconds(0.5f);
        }

        int index = 0;
        for (int i = 0; i < IGBC.BetList.Count; i++)
        {
            if (FreeSpinInfo.betCredit == IGBC.BetList[i])
            {
                index = i; break;
            }
        }
        MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData<int>("UpdateBetIndex", index));


        //设置Spin按钮Auto状态
        //SpinButton SpinBtn = null;
        //while (SpinBtn == null)
        //{
        //    SpinBtn = GameObject.Find("In Game/Anchor/In Game Bottom/Anchor/Layout/Button Spin")?.GetComponent<SpinButton>();
        //    if (SpinBtn == null)
        //    {
        //        SpinBtn = GameObject.Find("In Game/Anchor/In Game Bottom/Anchor/Layout/Center/Button Spin")?.GetComponent<SpinButton>();///竖屏游戏的spinbutton路径
        //    }
        //    yield return new WaitForSeconds(0.5f);
        //}
        /*if (SpinBtn == null)
        {
            isStartLastFreeSpin = false;
            yield break;
        }*/
        ///设置为自动 spin


        //while (SpinBtn.GetSpinButtonState() != NextSpinState.ToStopAuto)
        ////{
        //MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData<int>(MachineEventDefine.ON_KEY_START, 1));
        //yield return new WaitForSeconds(1f);
        //MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData<int>(MachineEventDefine.ON_KEY_START, 0));
        //yield return new WaitForSeconds(1f);
        ////}
        //MessageDispatcher.Dispatch("OnSpinButtonEvent", new EventData("OnSpinButtonEvent"));
        _delayTimer = this.DelayAction(6f, () => 
        {
            BlackboardUtils.SetOrCreateValue(ContentBlackboard.Get(), "autoSpin", true);
            MessageDispatcher.Dispatch("OnSpinButtonEvent", new EventData("OnSpinButtonEvent"));
        }, null, true);
        _delayTimer.Restart(UpdateMode.RealTime);
    }
    /// <summary>
    /// 获取额外押注的金额
    /// </summary>
    private long GetExtraBet(string spin)
    {
        string pattern = "\"extra_bet\":\\s*(\\d+)";
        Match match = Regex.Match(spin, pattern);
        long bet = 0;

        while (match.Success && bet <= 0)
        {
            string str = match.Groups[1].Value;
            bet = long.Parse(str);
            match = match.NextMatch();
            return bet;
        }
        return 0;
    }


    int GetTotalCount()
    {
        // 正则表达式，匹配bet_credit后面的数字  
        string pattern = "\"total_count\":\\s*(\\d+)";

        if (globalStore.nowGameID == 142)
        {
            pattern = "\"extra_bet\":\\s*(\\d+)";
        }

        for (int i = 0; i < historyRes.Count; i++)
        {
            // 搜索匹配项  
            Match match = Regex.Match(historyRes[i], pattern);

            if (match.Success)
            {
                // 提取数字  
                string betCredit = match.Groups[1].Value;
                return int.Parse(betCredit);
            }
        }
        return 0;
    }

    /// <summary>
    /// 小游戏节点的自动选择写在这里
    /// </summary>
    /// <remarks>
    /// 通常在断线重连时调用
    /// </remarks>
    public void NodeMiniGameAutoSelect()
    {
        switch (globalStore.nowGameID)
        {
            case 92://金猪
                EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData("LastCoinPick"));
                EventSender.SendGlobalEvent("OnContentUIEvent", new ParadoxNotion.EventData("EndCoinPick"));
                break;
            case 153://超人
                EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Click"));
                EventSender.SendGlobalEvent("OnCustomEvent", new EventData("0Clicked"));
                List<int> selectIndexList = LastFreeGameManager.Instance.FreeSpinInfo.SelectTypeList;
                int currentSelectCount = BlackboardUtils.GetOrCreateVariable<int>("./bonusSelectCount").value;
                int selectTypeCurrentIndex = currentSelectCount - currentSelectCount / 4 - 1;
                if (selectTypeCurrentIndex >= selectIndexList.Count || selectTypeCurrentIndex < 0)
                    break;
                int selectType = selectIndexList[selectTypeCurrentIndex] % 52;
                string eventName = selectType >= 26 ? "HighClicked" : "LowClicked";
                EventSender.SendGlobalEvent("OnCustomEvent", new EventData(eventName));
                break;
            case 35:
                    EventSender.SendGlobalEvent("OnContentUIDetailEvent", new EventData("GemClicked"));
                    EventSender.SendGlobalEvent("OnContentUIEvent", new EventData("FinishFirebolt"));
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnStartFreeSpin"));
                break;
            default:
                break;
        }
    }

    public void ConfirmPopFreeGameSelect()
    {
        if (!MachineSelectManager.Instance.isPopFreeGameTimeSelect())
            return;

        int totalCount = GetTotalCount();
        int _curSelectNumb = 0;
        string name = "";

        Debug.Log($"【LastFreeSpin】 ： totalCount = {totalCount}");
        if (globalStore.nowGameID == 37)  //GOLDEN_PICTURES
        {
            /* switch (_curSelectNumb)
             {
                 case 0:
                     name = "OnClick1";
                     break;
                 case 1:
                     name = "OnClick2";
                     break;
                 case 2:
                     name = "OnClick3";
                     break;
             }*/
            //Debug.Log($"EVT = OnSelection{_curSelectNumb}");
            //"bet": 1000,
            name = "OnClick1"; //(已经给定选择结果，选那个都一样)
            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData(name));
        }

        if (globalStore.nowGameID == 93)  // Happy Dollars
        {
            List<int> temp = new List<int>() { 6, 5, 4, 3, 2 };
            List<int> select = new List<int>() { 12, 10, 8, 6, 4 }; // 0
            //List<int> select = new List<int>() {18,15,12,9,6 };  // 1
            //List<int> select = new List<int>() {24, 20, 16,12, 8 };  // 2

            for (int i = 0; i < temp.Count; i++)
            {
                select[i] += temp[i] * FreeSpinInfo.triggeredTypeIndex_id93;
            }

            if (select.Contains(totalCount))
            {
                _curSelectNumb = select.IndexOf(totalCount);
            }

            name = $"OnSelection{_curSelectNumb}";
            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData(name));
        }


        if (globalStore.nowGameID == 105) //SUNSET_SAFARI 狮子斑马
        {

            /*switch (_curSelectNumb)
            {
                case 0:
                    name = "SelectFreeSpin";
                    break;
                case 1:
                    name = "SelectLinkBonus";  //这个是小游戏（数据不保留redis）
                    break;
            }*/
            int index = GetSelectIndex();
            name = "SelectFreeSpin";
            if(index == 1)
            {
                name = "SelectLinkBonus";
            }
            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData(name));
        }


        if (globalStore.nowGameID == 116) //魔术师 - 选牌
        {
            //(已经给定选择结果，选那个都一样)
            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData<int>("MachineSelectEvent", _curSelectNumb));
        }

        if (globalStore.nowGameID == 149) //白虎 
        {
            List<int> select = new List<int>() { 32, 16, 8, 4 };
            if (select.Contains(totalCount))
            {
                _curSelectNumb = select.IndexOf(totalCount);
            }
            name = $"OnSelection{_curSelectNumb}";
            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData(name));
        }

        if (globalStore.nowGameID == 73)
        {
            EventSender.SendGlobalEvent("OnContentUIDetailEvent", new ParadoxNotion.EventData("ScatterClicked"));
            //与免费游戏重连窗口的关闭事件名冲突了，这里加上1
            EventSender.SendGlobalEvent("OnCustomEvent", new EventData("StartClosePopup"));
            EventSender.SendGlobalEvent("OnCustomEvent", new EventData("ClosePopup1"));
        }
        else if (globalStore.nowGameID == 54)
        {
            EventSender.SendGlobalEvent("OnContentUIDetailEvent", new EventData<int>("OnClick", 0, 1));
            EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnStartFreeSpin"));
        }
        
        else if (globalStore.nowGameID == 128)
        {
            EventSender.SendGlobalEvent("OnCustomEvent", new EventData("StartClosePopup"));
            EventSender.SendGlobalEvent("OnCustomEvent", new EventData("ClosedPopup"));
        }
        else if (globalStore.nowGameID == 99)
        {
            EventSender.SendGlobalEvent("OnCustomEvent", new EventData("StartClosePopup"));
            EventSender.SendGlobalEvent("OnCustomEvent", new EventData("ClosedPopup"));
        }
        else if (globalStore.nowGameID == 83)
        {
            int selecIndex = GetSelectIndex(); 
            EventSender.SendGlobalEvent("OnCustomEvent", new EventData($"Click{5 - selecIndex}"));
            //From:whh - 2024年9月12日
            //补充免费游戏中弹出小游戏的断线重连逻辑
            EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnClick"));
            EventSender.SendGlobalEvent("OnCustomEvent", new EventData("collectEvent"));
        }
        else if (globalStore.nowGameID == 62)
        {
            EventSender.SendGlobalEvent("OnCustomEvent", new EventData($"Click{FreeSpinInfo.SelectedIndex + 1}"));
        }
        else if (globalStore.nowGameID == 31)
        {
            EventSender.SendGlobalEvent("OnContentUIDetailEvent", new EventData("FinishedShowTTS"));
            EventSender.SendGlobalEvent("OnCustomEvent", new EventData("TapScreen"));

            EventSender.SendGlobalEvent("OnContentUIDetailEvent", new EventData("TrySpinWheel"));
        }
        else if (globalStore.nowGameID == 142)////丛林火焰
        {
            int index = (int)(FreeSpinInfo.extraBetCredit / (FreeSpinInfo.betCredit)) - 1; 
            EventSender.SendGlobalEvent("OnCustomEvent", new ParadoxNotion.EventData<int>("Change", index));
            EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnStartFreeSpin"));
        }
        else if (globalStore.nowGameID == 10)  ////幸运财富
        {
            string data = historyRes[0];
            string pattern = "\"selected_index\":\\s*(\\d+)";
            Match match = Regex.Match(data, pattern);
            if (match.Success)
            {
                string str = match.Groups[1].Value;
                int value = int.Parse(str);
                if (value == 0)
                {
                    EventSender.SendGlobalEvent(EVTType.ON_CUSTOM_EVENT, new EventData("OnRoyalFreeSpinClick"));
                }
                else
                {
                    EventSender.SendGlobalEvent(EVTType.ON_CUSTOM_EVENT, new EventData("OnMultiplierFreeSpinClick"));
                }
            }
        }
    }

    private int GetSelectIndex()
    {
        string pattern = "\"selected_index\":\\s*(\\d+)";
        for (int i = 0; i < historyRes.Count; i++)
        {
            Match match = Regex.Match(historyRes[i], pattern);
            if (match.Success)
            {
                string str = match.Groups[1].Value;
                int value = int.Parse(str);
                return value;
            }
        }
        return 0;
    }

    public int Get142GameExtraBetIndex()
    {
        int index = (int)(FreeSpinInfo.extraBetCredit / (FreeSpinInfo.betCredit)) - 1;
        return index;
    }

    public int Get175GameExtraBetIndex()
    {

        int result = 0;
        string data = historyRes[0];
        string pattern = "\"extra_bet_index\":\\s*(\\d+)";
        Match match = Regex.Match(data, pattern);
        if (match.Success)
        {
            string str = match.Groups[1].Value;
            int value = int.Parse(str);
            result = value;
        }
        return result;
    }
    public List<string> historyRes = new List<string>();


    void test_getHistory(int id)
    {
        bool isOK = true;

        historyRes = new List<string>();

        for (int i = 0; i < test_his[id].Count; i++)
        {
            TextAsset jsn8 = Resources.Load<TextAsset>(test_his[id][i]);
            if (jsn8 != null && jsn8.text != null)
            {
                historyRes.Add(jsn8.text);
            }
            else
            {
                isOK = false;
                Debug.LogError($"【LastFreeSpinManager】：找不到文件 {test_his[id][i]}");
            }
        }

        if (isOK)
        {
            isLastGameSpin = true;
        }
        Debug.Log($"【LastFreeSpinManager】： isLastGameSpin = {isLastGameSpin}");
    }


    [Button]
    void test_getisLastGameSpin()
    {
        Debug.Log($"【LastFreeSpinManager】： isLastGameSpin = {isLastGameSpin}");
    }

    [Button]
    void test_setisLastGameSpin()
    {
        test_getHistory(globalStore.nowGameID);
    }



    bool _isLastGameSpin = false;
    public bool isLastGameSpin
    {
        get
        {
            _isLastGameSpin = BlackboardUtils.GetOrCreateVariable<bool>(MainBlackboard.Get(), "isLastGameSpin").value;
            return _isLastGameSpin;
        }
        set
        {
            _isLastGameSpin = value;
            BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(), "isLastGameSpin", _isLastGameSpin);
        }
    }


    public void getResponseData(string rpc, Action<JSONNode> responseCallback)
    {
        StartCoroutine(_getResponseData(rpc, responseCallback));
    }

    /// <summary>
    /// 标记是否是断线重连的第一条数据
    /// </summary>
    private bool isFirstData = true;
    private IEnumerator _getResponseData(string rpc, Action<JSONNode> responseCallback)
    {
        yield return new WaitForSeconds(0.2f);
        MessageDispatcher.Dispatch("OnSpinButtonEvent", new EventData("OnSpinButtonEvent"));///置灰操作按钮
        if (_loginMaskController == null)
        {
            _loginMaskController = FindObjectOfType<LoginMaskController>();    
        }
        if (_loginMaskController != null)
        {
            _loginMaskController.SetSliderTotal(historyRes.Count);
        }
        if (responseCallback != null)
        {
            string res = historyRes[0];
            Debug.Log($"==@【LastFreeSpin】 : {rpc} = {res}");
            SimpleJSON.JSONNode dataDict = SimpleJSON.JSONNode.Parse(res);
            historyRes.RemoveAt(0);
            if (_loginMaskController != null)
            {
                _loginMaskController.AddSliderValue();
            }

            if (isFirstData)    
            {
                isFirstData = false;
                ///第一条数据，重置玩家的金额数据
                long balance = dataDict["data"]["before_balance"].AsLong;
                JSONNode client_data = dataDict["client_data"];
                long bet = client_data["bet"].AsLong;
                long extra_bet = client_data["extra_bet"].AsLong;
                long bet_per_hand = client_data["bet_per_hand"].AsLong;
                balance -= (bet + extra_bet + bet_per_hand);
                BlackboardQueryUtils.SetMyCredit(balance);
                MessageDispatcher.Dispatch("OnCreditEvent", new EventData<bool>("UpdateNaviCredit", true));
                if (globalStore.nowGameID == 103 || globalStore.nowGameID == 182)///NIGHTS_OF_BINGO     id：103 游戏断线重连的时候，需要设置滚轮表的序号
                {
                    GlobalReelStrips.Instance.index = dataDict["data"]["contents"]["reel_set_index"]["current_index"].AsInt;
                }
            }
            if (historyRes.Count == 0) //最后一局不放慢
            {
                Time.timeScale = 1;

                //From:whh - 2024年10月11日
                //断线重连后恢复autoSpin，避免进行非玩家意愿的spin
                BlackboardUtils.SetOrCreateValue(ContentBlackboard.Get(), "autoSpin", false);
                isLastGameSpin = false;
                FinishEvent();
                EventSender.SendGlobalEvent("OnCloseLoginMaskPop");

                //恢复声音
                GSManager.Instance.MusicVolume = PlayerPrefs.GetFloat("MUTE_MUSIC", 1);
                GSManager.Instance.SfxVolume = PlayerPrefs.GetFloat("MUTE_SFX", 1);
            }
            else
            {
                Time.timeScale = 10;
            }
            responseCallback(dataDict["data"]);
        }
    }

    private void FinishEvent()
    {
        switch (globalStore.nowGameID)
        {
            case 152:
                EventSender.SendGlobalEvent("OnContentUIEvent", new EventData("SubSymbolMechanicsDone")); 
                break;
            default:
                EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Finalize"));
                EventSender.SendGlobalEvent("OnCustomEvent", new EventData("MachineSpinClick"));
                break;
        }
    }

    private void Clear()
    {
        if (_StartLastFreeSpin != null)
        {
            StopCoroutine(_StartLastFreeSpin);
            _StartLastFreeSpin = null;
        }
        ClearTask();
        isStartLastFreeSpin = false;
        isLastGameSpin = false;
    }

    [Button]
    public void GetFreeSpinHistory(string firstSpin = "")
    {

        //声音设置为0
        GSManager.Instance.MusicVolume = 0;
        GSManager.Instance.SfxVolume = 0;

        Clear();
        historyRes = new List<string>();
        historyRes.Add(firstSpin);
        StartCoroutine(getHistory());
    }

    //last_regular_message
    private IEnumerator getHistory()
    {
        bool isFinish = false;
        int i = 1;
        bool isNext = false;
        while (!isFinish)
        {
            Debug.Log($"【history】: Next");
            isNext = false;
            NetManager.Instance.Post(RPCName.freeSpinHistory, new Dictionary<string, object> { { "step_index", i } },
            (res) =>
            {
                //string resStr = res.ToString();
                //Debug.Log($"【history】 = {resStr}");
                if (res["spin_history"] != null)
                {
                    //historyRes.Add(res["spin_history"].ToString());
                    historyRes.Add(res["spin_history"]);
                }
                else
                {
                    isFinish = true;
                }
                isNext = true;
            },
            (error) =>
            {
                isNext = true;
                isFinish = true;
            });
            i++;
            yield return new WaitUntil(() => isNext);
        }
        isLastGameSpin = true;
        
        if (_loginMaskController != null)
        {
            _loginMaskController.SetSliderTotal(historyRes.Count - 1);
        }
    }
    /// <summary>
    /// 获取game id 52 的小游戏类型
    /// </summary>
    /// <returns></returns>
    public int GetGame52ClaimType()
    {
        string pattern = "\"bonus_id\":\\s*(\\d+)";
        //Match match = Regex.Match(spin, pattern)
        for (int i = 0; i < historyRes.Count; i++)
        {
            string response = historyRes[i];
            if (response.Contains("claim_bonus"))
            {
                Match match = Regex.Match(response, pattern);
                if (match.Success)
                {
                    string str = match.Groups[1].Value;
                    int index = int.Parse(str);
                    switch(index)
                    {
                        case 5201:
                            return 0;
                        case 5202:
                            return 1;
                        case 5203:
                            return 2;
                        case 5204:
                            return 3;
                    }
                }
            }
            //Match match = Regex.Match(spin, pattern);
        }
        return -1;
    }

    /// <summary>
    /// 获取 game id 52 中 Potions Bonus 小游戏的操作结果
    /// </summary>
    /// <returns></returns>
    public int GetGame52PotionsBonusResult()
    {
        string pattern = "\"potion_value\":\\s*(-?\\d+)";
        for (int i = 0; i < historyRes.Count; i++)
        {
            string response = historyRes[i];
            if (response.Contains("claim_bonus"))
            {
                Match match = Regex.Match(response, pattern);
                if (match.Success)
                {
                    string str = match.Groups[1].Value;
                    int index = int.Parse(str);
                    return index;
                }
            }
            //Match match = Regex.Match(spin, pattern);
        }
        return -100;
    }

    public bool GetGame52IsSuccess()
    {
        string pattern = "\"earn_credit\":\\s*(\\d+)";
        for (int i = 0; i < historyRes.Count; i++)
        {
            string response = historyRes[i];
            if (response.Contains("claim_bonus"))
            {
                Match match = Regex.Match(response, pattern);
                if (match.Success)
                {
                    string str = match.Groups[1].Value;
                    long earn_credit = long.Parse(str);
                    long bet = BlackboardUtils.FindVariable<long>("./totalBetCredit").value;
                    return earn_credit > bet;
                }
            }
        }
        return false;
    }


    private List<int> KenoIndexList = new List<int>();
    public List<int> GetKENOIndeices()
    {
        if (KenoIndexList.Count == 0)
        {
            if (historyRes.Count > 0)
            {
                JSONNode node = JSONNode.Parse(historyRes[0]);
                if (node != null)
                {
                    JSONNode data = node["data"];
                    JSONNode contents = data["contents"];
                    JSONNode win_result = contents["win_result"][0];
                    JSONNode listNode = win_result["pick_info"];
                    for (int j = 0; j < listNode.Count; j++)
                    {
                        KenoIndexList.Add(listNode[j].AsInt);
                    }
                }
            }
        }

        return KenoIndexList;
    }
}

/// <summary>
/// 由服务器下行数据，推算出玩家押注，选择操作。
/// </summary>
public class FirstSpinInfo
{
    public FirstSpinInfo(List<string> historyRes)
    {
        historyJsonRes = ParseHistory(historyRes);
        GetFirstSpinInfo(historyRes[0]);
        if (historyRes.Count > 1)
        {
            GetSecondClaimInfo(historyRes[1]); 
        }
        SelectTypeList = GetSelectIndexList(historyJsonRes);
    }

    public List<JSONNode> historyJsonRes;

    /// <summary>
    /// 选择游戏的类型
    /// </summary>
    public int triggeredTypeIndex_id93 = 0;

    /// <summary>
    /// 下注金额
    /// </summary>
    public long betCredit = 0;

    /// <summary>
    /// 额外下注金额
    /// </summary>
    public long extraBetCredit = 0;

    /// <summary>
    /// 免费游戏前的选择索引（可通用）
    /// </summary>
    /// <remarks>
    /// 断线重连中第二条协议，claim中ClientDataD的SelectedIndex
    /// </remarks>
    public int SelectedIndex { get; private set; }

    public int handCount;

    public List<int> SelectTypeList { get; private set; }

    /// <summary>
    /// 解析history字符串为json格式
    /// </summary>
    /// <param name="historyRes"></param>
    private List<JSONNode> ParseHistory(List<string> historyRes)
    {
        if (historyRes == null || historyRes.Count <= 0)
            return null;

        var ret = new List<JSONNode>();
        foreach (var item in historyRes)
        {
            JSONNode jsonRes = JSONNode.Parse(item);
            ret.Add(jsonRes);
        }

        return ret;
    }

    private void GetFirstSpinInfo(string spin)
    {
        //From:whh - 2024年9月19日
        //有些游戏用的是bet，有些游戏用的是bet_credit
        //之前的逻辑是根据游戏id来做逻辑分支
        //为免以后再出现类似问题，直接优化逻辑如下

        ///string patternBank = "\"bet_credit\":\\s*(\\d+)";
        ///获取下注金额统一使用  bet  来识别，因为 bet_credit 这个数值会包含 额外下注的金额，数值不正确的
        string pattern = "\"bet\":\\s*(\\d+)";
        Match match = Regex.Match(spin, pattern);
        while (match.Success)
        {
            string str = match.Groups[1].Value;
            betCredit = long.Parse(str);
            match = match.NextMatch();
        }
        if (betCredit <= 0)  ///基诺类型的游戏的押注字段
        {
            string pattern1 = "\"bet_per_ticket\":\\s*(\\d+)";
            Match match1 = Regex.Match(spin, pattern1);
            while (match1.Success)
            {
                string str = match1.Groups[1].Value;
                betCredit = long.Parse(str);
                match1 = match1.NextMatch();
            }
        }
        if (betCredit <= 0)///扑克牌类型的押注字段
        {
            string pattern2 = "\"bet_per_hand\":\\s*(\\d+)";
            Match match2 = Regex.Match(spin, pattern2);
            while (match2.Success)
            {
                string str = match2.Groups[1].Value;
                betCredit = long.Parse(str);
                match2 = match2.NextMatch();
            }
        }
        ////获取额外下注金额 
        pattern = "\"extra_bet\":\\s*(\\d+)";
        match = Regex.Match(spin, pattern);
        while (match.Success)
        {
            string str = match.Groups[1].Value;
            extraBetCredit = long.Parse(str);
            match = match.NextMatch();
        }

        if (globalStore.nowGameID == 93)
        {
            pattern = "\"triggered_type_index\":\\s*(\\d+)";
            match = Regex.Match(spin, pattern);

            if (match.Success)
            {
                string str = match.Groups[1].Value;
                triggeredTypeIndex_id93 = int.Parse(str);
            }
            else
            {
                Debug.LogError("triggered_type_index is not find");
            }
        }
        pattern = "\"hand_count\":\\s*(\\d+)";
        if (handCount == 0)
        {
            match = Regex.Match(spin, pattern);
            if (match.Success)
            {
                string hand = match.Groups[1].Value;
                handCount = int.Parse(hand);
            }
        }
    }
    private void GetSecondClaimInfo(string claim)
    {
        string pattern = "\"selected_index\":\\s*(\\d+)";
        Match match = Regex.Match(claim, pattern);
        if (match.Success)
        {
            string str = match.Groups[1].Value;
            SelectedIndex = int.Parse(str);
        }
    }
    /// <summary>
    /// 获取所有Cliaim中的SelectIndex数据，顺序存储到list中
    /// </summary>
    /// <remarks>
    /// 用于断线重连中还原用户操作
    /// </remarks>
    /// <param name="historyJson"></param>
    /// <returns></returns>
    private List<int> GetSelectIndexList(List<JSONNode> historyJson)
    {
        if (historyJson == null || historyJson.Count <= 0)
            return null;

        List<int> ret = new List<int>();
        foreach (var item in historyJson)
        {
            if (item["protocol_key"] != "claim_bonus")
                continue;

            if (item.HasKey("client_data") == false)
                continue;

            JSONNode clientDataNode = item["client_data"];
            if (clientDataNode.HasKey("decision_info") == false)
                continue;

            JSONNode decisionInfoNode = clientDataNode["decision_info"];
            int selectIndex = decisionInfoNode["selected_index"].AsInt;
            ret.Add(selectIndex);
        }

        return ret;
    }
}
