using BagelCode;
using BagelCode.Protobuf;
using BlizzEvent;
using Dreamteck.Splines.Primitives;
using Newtonsoft.Json;
using ParadoxNotion;
using SBoxApi;
using SimpleJSON;
using Sirenix.OdinInspector;
using SlotMaker;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using static SBoxApi.SBoxSandbox;
using EventData = ParadoxNotion.EventData;

public partial class SBoxSanboxController : EventMonoSingleton<SBoxSanboxController>
{


    void Start()
    {
        if (!ApplicationSettings.Instance.isMachine)
            return;

        AddButtonEvent();
        AddEventListener();

        MessageDispatcher.Register("MachineBtnEvent", OnMachineBtnEvent);
        MessageDispatcher.Register("NetManagerEvent", OnNetManageEvent);


        this._taskTimer = new System.Timers.Timer(3000);
        this._taskTimer.AutoReset = true; // 是否重复执行
        this._taskTimer.Elapsed += (object sender, ElapsedEventArgs e) =>
        {
            task = () =>  //延时，避免   OSA_LobbySlots.ResetCurSelect() 影响
            {
                Debug.Log("【BillLst】get bill list ...");
                GetBillLst();
                GetPrintList();
            };
        };
        //this._keepAliveTimer.Enabled = true; //开始执行
        this._taskTimer.Start();


        NetManager.Instance.On(RPCName.confirmCoinOutOrder, OnConfirmCoinOutOrder);
        NetManager.Instance.On(RPCName.confirmAddCoinOrder, OnConfirmAddCoin);
        NetManager.Instance.On(RPCName.createPrintOrder, OnCreatePrintOrder);

    }



    System.Action task;
    bool isRuning = false;
    public void Update()
    {
        if (!isRuning)
        {
            isRuning = true;
            if (task != null)
            {
                task();
                task = null;
            }
            isRuning = false;
        }
    }

    protected override void OnDestroy()
    {
        MessageDispatcher.UnRegister("MachineBtnEvent", OnMachineBtnEvent);  //"ShowLightSelectTip"
        MessageDispatcher.UnRegister("NetManagerEvent", OnNetManageEvent);

        NetManager.Instance.Off(RPCName.confirmCoinOutOrder, OnConfirmCoinOutOrder);
        NetManager.Instance.Off(RPCName.confirmAddCoinOrder, OnConfirmAddCoin);
        NetManager.Instance.Off(RPCName.createPrintOrder, OnCreatePrintOrder);

        if (this._taskTimer != null)
        {
            this._taskTimer.Stop();
            this._taskTimer.Dispose();
            this._taskTimer = null;
        }
    }

    protected System.Timers.Timer _taskTimer = null;

    private void OnMachineBtnEvent(ParadoxNotion.EventData eventData)
    {

        if (eventData.name == "LightBtnSelectShowTipOn")//"LightBtnSelectShowTip"  "LightBtnSelect"
        {
            StartCoroutine(OnLightBtnSelectShowTip((int)eventData.value));
        }

        if (eventData.name == "LightBtnSelectShowTipOff")
        {
            CloseLightTip();
        }
        /*
        if (eventData.name == "LightBtnOn")
        {
            LightOn((SBOX_SWITCH)eventData.value);
        }
        if (eventData.name == "LightBtnOff")
        {
            LightOff((SBOX_SWITCH)eventData.value);
        }
        */
        if (eventData.name == "LightBtnOpenSpin")
        {
            LightOn(SBOX_SWITCH.SWITCH_ENTER);
        }

        if (eventData.name == "LightBtnCloseSpin")
        {
            LightOff(SBOX_SWITCH.SWITCH_ENTER);
        }

        if (eventData.name == "ChangeSceneBtnLight")
        {
            string[] map = (string[])eventData.value;

            List<SBOX_SWITCH> include = new List<SBOX_SWITCH>();
            for (int i = 0; i < map.Length; i++)
            {
                if (!include.Contains(keyMap[map[i]]))
                {
                    include.Add(keyMap[map[i]]);
                }
            }

            sceneBtns = include;
            setSceneLightOn();
        }

    }

    readonly Dictionary<string, SBOX_SWITCH> keyMap = new Dictionary<string, SBOX_SWITCH>()
    {
        { "BtnSpin", SBOX_SWITCH.SWITCH_ENTER },
        { "BtnPre", SBOX_SWITCH.SWITCH_YELLOW },
        { "BtnNext", SBOX_SWITCH.SWITCH_BET4 },
        { "BtnExit", SBOX_SWITCH.SWITCH_SWITCH },
        { "BtnSwitch", SBOX_SWITCH.SWITCH_BET5 },
        { "BtnBetUp", SBOX_SWITCH.SWITCH_RED },
        { "BtnBetDown", SBOX_SWITCH.SWITCH_GREEN },
        { "BtnBetMax", SBOX_SWITCH.SWITCH_AUTO},
        { "BtnHelp", SBOX_SWITCH.SWITCH_ESC},
    };


    bool isLightBtnSelectShowTip = false;

    public List<SBOX_SWITCH> lightBtn = new List<SBOX_SWITCH>() {
        //上一排（从左到右）
        SBOX_SWITCH.SWITCH_RED,
        SBOX_SWITCH.SWITCH_GREEN,
        SBOX_SWITCH.SWITCH_YELLOW,
        SBOX_SWITCH.SWITCH_BET4,
        //下一排（从左到右）
        SBOX_SWITCH.SWITCH_AUTO,
        SBOX_SWITCH.SWITCH_ESC,
        SBOX_SWITCH.SWITCH_SWITCH,
        SBOX_SWITCH.SWITCH_BET5,
        //大健
        SBOX_SWITCH.SWITCH_ENTER,
    };


    List<SBOX_SWITCH> sceneBtns = new List<SBOX_SWITCH>();

    public void setSceneLightOn()
    {
        for (int i = 0; i < lightBtn.Count; i++)
        {
            if (sceneBtns.Contains(lightBtn[i]))
            {
                LightOn(lightBtn[i]);
            }
            else
            {
                LightOff(lightBtn[i]);
            }
        }
    }

    public IEnumerator OnLightBtnSelectShowTip(int num)
    {

        for (int i = 0; i < lightBtn.Count; i++)
        {
            LightOff(lightBtn[i]);
        }

        isLightBtnSelectShowTip = true;
        while (isLightBtnSelectShowTip)
        {
            for (int i = 0; i < num; i++)
            {
                LightOn(lightBtn[i]);
            }
            yield return new WaitForSeconds(1);

            for (int i = 0; i < num; i++)
            {
                LightOff(lightBtn[i]);
            }
            yield return new WaitForSeconds(0.8f);
        }
        setSceneLightOn();
        yield return null;
    }


    public IEnumerator OnSpin()
    {
        isLightBtnSelectShowTip = true;
        while (isLightBtnSelectShowTip)
        {

            LightOn(SBOX_SWITCH.SWITCH_ENTER);

            yield return new WaitForSeconds(1);


            LightOff(SBOX_SWITCH.SWITCH_ENTER);

            yield return new WaitForSeconds(0.8f);
        }

        yield return null;
    }

    /*
    void LightOnAll()
    {
        setSceneLightOn();

        LightOn(SBOX_SWITCH.SWITCH_ENTER); //Spin
    }*/

    void LightOn(SBOX_SWITCH key)
    {
#if UNITY_EDITOR
        MatchDebugManager.Instance.SendUdpMessage(EventHandle.SBOX_SADNBOX_SWITCH_ON, ((ulong)key).ToString());
#else
        SwitchOutStateOn((ulong)key);
#endif
    }
    void LightOff(SBOX_SWITCH key)
    {
#if UNITY_EDITOR
        MatchDebugManager.Instance.SendUdpMessage(EventHandle.SBOX_SADNBOX_SWITCH_OFF, ((ulong)key).ToString());
#else
        SwitchOutStateOff((ulong)key);
#endif
    }



    private void AddButtonEvent()
    {

#if UNITY_EDITOR
        EventCenter.Instance.AddEventListener<SBOX_SWITCH>(EventHandle.HARDWARE_KEY_DOWN, OnKeyDown);
        EventCenter.Instance.AddEventListener<SBOX_SWITCH>(EventHandle.HARDWARE_KEY_UP, OnKeyUp);
#else
        if (ApplicationSettings.Instance.isMachine){

            foreach (SBOX_SWITCH value in Enum.GetValues(typeof(SBOX_SWITCH)))
            {
                SBoxSandboxListener.Instance.AddButtonDown(value, () => {
                    OnKeyDown(value);
                });
                SBoxSandboxListener.Instance.AddButtonUp(value, () => {
                    OnKeyUp(value);
                });
            }
        }
#endif
    }


    /*

    【上面一排（从左到右）】：
    1: KeyDown SWITCH_RED
    2: KeyDown SWITCH_GREEN
    3: KeyDown SWITCH_YELLOW
    4: KeyDown SWITCH_BET4


    【下面一排（从左到右）】：
    1: KeyDown SWITCH_AUTO
    2: KeyDown SWITCH_ESC
    3: KeyDown SWITCH_SWITCH
    4: KeyDown SWITCH_BET5

    【右边大按钮】：
    KeyDown SWITCH_ENTER；

    【左边大按钮】：
    没有接

    【斜坡按钮】：
    没有接

    【上分/下分】：
    没有接

    【退票】：
    没有接

    */

    private void CloseLightTip()
    {
        if (isLightBtnSelectShowTip)
        {
            isLightBtnSelectShowTip = false;
            StopAllCoroutines();
            setSceneLightOn();
        }
        return;
    }

    private void OnKeyDown(SBOX_SWITCH sBOX_SWITCH)
    {


#if UNITY_EDITOR
        Debug.LogError("KeyDown " + sBOX_SWITCH);
#endif


        if (isLightBtnSelectShowTip && !lightBtn.Contains(sBOX_SWITCH))
        {
            return;
        }

        if (isLightBtnSelectShowTip && lightBtn.Contains(sBOX_SWITCH))
        {
            CloseLightTip();
            MessageDispatcher.Dispatch("MachineBtnEvent", new EventData<int>("LightBtnSelect", lightBtn.IndexOf(sBOX_SWITCH)));
            return;
        }


        switch (sBOX_SWITCH)
        {
            case SBOX_SWITCH.SWITCH_UP:
                //EventSender.SendGlobalEvent(MetaEventDefine.ON_MACHINE, new EventData<int>(MachineEventDefine.ON_KEY_UP, 0));
                break;
            case SBOX_SWITCH.SWITCH_DOWN:
                //EventSender.SendGlobalEvent(MetaEventDefine.ON_MACHINE, new EventData<int>(MachineEventDefine.ON_KEY_DOWN, 0));
                break;
            case SBOX_SWITCH.SWITCH_LEFT:
                //选择框左移
                //MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_LEFT));
                //降低押注
                //MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetDown"));
                break;
            case SBOX_SWITCH.SWITCH_RIGHT:
                //选择框右移
                //MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_RIGHT));
                //提高押注
                //MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetUp"));
                break;
            case SBOX_SWITCH.SWITCH_ROOT_SET:
                break;
            case SBOX_SWITCH.SWITCH_SET:
                break;
            case SBOX_SWITCH.SWITCH_DOOR_SWITCH:
                break;
            case SBOX_SWITCH.SWITCH_PAYOUT: //退票
                StartCoinOut();
                break;
            case SBOX_SWITCH.SWITCH_ENTER: //Spin
                /*if (PopupManager.Instance.popupCount > 0)
                {
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Return"));
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("Collect"));
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnClose"));
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnCollect"));
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnPointerClick"));
                    EventSender.SendGlobalEvent("OnCustomEvent", new EventData("OnStartFreeSpin"));
                }
                else
                    MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData<int>(MachineEventDefine.ON_KEY_START, 1));
               */

                MachineSelectManager.Instance.BtnSpinDown();

                /*if (this._taskTimer != null && sandBoxBillList == null)
                {
                    this._taskTimer.Start();
                }*/
                break;
            case SBOX_SWITCH.SWITCH_ESC:
                /*if (PopupManager.Instance.popupCount == 0)
                    EventSender.SendGlobalEvent("OpenPaytable");
                */
                MachineSelectManager.Instance.BtnHelp();
                break;
            case SBOX_SWITCH.SWITCH_SWITCH:
                /*//最大下注
                MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetMax"));
                //炮升级
                //EventSender.SendGlobalEvent(MetaEventDefine.ON_MACHINE, new EventData<int>(MachineEventDefine.ON_KEY_MAX_BET, data));
                */
                MachineSelectManager.Instance.BtnReturn();
                break;
            case SBOX_SWITCH.SWITCH_SCORE_UP:
                //MachineSelectManager.Instance.BtnAddCoin();
                /*MachineSelectManager.Instance.PurchaseCreditRequest(1, 10000);//加分*/

                //MachineSelectManager.Instance.PurchaseCreditRequest(1, 10);//加分*/


                // 下份键 和 2号投币机 共用一个信号线
                OnCoinIn(new CoinInData
                {
                    id = 2,
                    value = 1,
                });
                
                break;
            case SBOX_SWITCH.SWITCH_SCORE_DOWN:
                /*int credit = (int)(BlackboardUtils.FindVariable<long>(null, "/me/credit").value / RATE) * RATE;
                Debug.Log($"【printer】: All dollar = {BlackboardUtils.FindVariable<long>(null, "/me/credit").value / RATE}, credit = {credit} ");
                if(credit > 0)
                {
                    MachineSelectManager.Instance.PurchaseCreditRequest(2, credit, () =>
                    {
                        test_PrinterMessage(credit);
                    });//减分
                }*/

               // MachineSelectManager.Instance.PurchaseCreditRequest(2, 10);//加分*/

                PrintMoneyOrder();

                break;
            case SBOX_SWITCH.SWITCH_RED:
                MachineSelectManager.Instance.BtnBetUp();
                break;
            case SBOX_SWITCH.SWITCH_GREEN:
                MachineSelectManager.Instance.BtnBetDown();
                break;
            case SBOX_SWITCH.SWITCH_YELLOW:
                /*//选择框左移
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_LEFT));
                //降低押注
                MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetDown"));
                */
                MachineSelectManager.Instance.BtnPre();
                break;
            case SBOX_SWITCH.SWITCH_BET4:
                /*//选择框右移
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_RIGHT));
                //提高押注
                MessageDispatcher.Dispatch(MetaEventDefine.ON_CREDIT_EVENT, new EventData("BetUp"));
                */
                MachineSelectManager.Instance.BtnNext();
                break;
            case SBOX_SWITCH.SWITCH_BET5:
                /*if (BlackboardQueryUtils.IsSpin()
                    || BlackboardQueryUtils.IsAutoSpin())
                    return;
                //退出
                EventSender.SendGlobalEvent("OnLobby");
                //鱼房间退出
                MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(MachineEventDefine.ON_KEY_BET5));
                //鱼机 退出
                MessageDispatcher.Dispatch(MetaEventDefine.ON_MACHINE, new EventData(MachineEventDefine.ON_KEY_BET5));
                */
                MachineSelectManager.Instance.BtnSwitch();
                break;
            case SBOX_SWITCH.SWITCH_AUTO:
                MachineSelectManager.Instance.BtnBetMax();
                break;
            default:
                break;
        }
    }

    private void OnKeyUp(SBOX_SWITCH sBOX_SWITCH)
    {
#if UNITY_EDITOR
        Debug.LogError("KeyUp " + sBOX_SWITCH);
#endif


        if (isLightBtnSelectShowTip && !lightBtn.Contains(sBOX_SWITCH))
        {
            return;
        }


        switch (sBOX_SWITCH)
        {
            case SBOX_SWITCH.SWITCH_UP:
                break;
            case SBOX_SWITCH.SWITCH_DOWN:
                break;
            case SBOX_SWITCH.SWITCH_LEFT:
                break;
            case SBOX_SWITCH.SWITCH_RIGHT:
                break;
            case SBOX_SWITCH.SWITCH_ROOT_SET:
                break;
            case SBOX_SWITCH.SWITCH_SET:
                break;
            case SBOX_SWITCH.SWITCH_DOOR_SWITCH:
                break;
            case SBOX_SWITCH.SWITCH_PAYOUT:
                break;
            case SBOX_SWITCH.SWITCH_ENTER:
                MachineSelectManager.Instance.BtnSpinUp();
                //MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData<int>(MachineEventDefine.ON_KEY_START, 0));
                break;
            case SBOX_SWITCH.SWITCH_ESC:
                break;
            case SBOX_SWITCH.SWITCH_SWITCH:
                break;
            case SBOX_SWITCH.SWITCH_SCORE_UP:
                break;
            case SBOX_SWITCH.SWITCH_SCORE_DOWN:
                break;
            case SBOX_SWITCH.SWITCH_RED:
                break;
            case SBOX_SWITCH.SWITCH_GREEN:
                break;
            case SBOX_SWITCH.SWITCH_YELLOW:
                break;
            case SBOX_SWITCH.SWITCH_BET4:
                break;
            case SBOX_SWITCH.SWITCH_BET5:
                break;
            case SBOX_SWITCH.SWITCH_AUTO:
                break;
            default:
                break;
        }
    }

    private void AddEventListener()
    {

        Debug.Log("【BillLst】AddEventListener");
        BlizzEvent.EventCenter.Instance.AddEventListener<CoinInData>(SBoxSanboxEventHandle.COIN_IN, OnCoinIn);
        BlizzEvent.EventCenter.Instance.AddEventListener<int>(SBoxSanboxEventHandle.COIN_OUT, OnCoinOut);
        BlizzEvent.EventCenter.Instance.AddEventListener<int>(SBoxSanboxEventHandle.BILL_IN, OnBillIn);
        BlizzEvent.EventCenter.Instance.AddEventListener(SBoxSanboxEventHandle.BILL_STACKED, OnBillStacked);

        Register(MachineEventDefine.ON_LIGHT_CHANGE, OnLightChange);

        //纸钞机
        BlizzEvent.EventCenter.Instance.AddEventListener<List<string>>(SBoxEventHandle.SBOX_SADNBOX_BILL_LIST_GET, OnSboxSandBoxBillListGet);
        BlizzEvent.EventCenter.Instance.AddEventListener<int>(SBoxEventHandle.SBOX_SADNBOX_BILL_SELECT, OnSboxSandBoxBillSelect);
        //打印机
        EventCenter.Instance.AddEventListener<List<string>>(SBoxEventHandle.SBOX_SADNBOX_PRINTER_LIST_GET, OnPrinterListGet);
        EventCenter.Instance.AddEventListener<int>(SBoxEventHandle.SBOX_SADNBOX_PRINTER_SELECT, OnPrinterSelect);
        EventCenter.Instance.AddEventListener<int>(SBoxEventHandle.SBOX_SADNBOX_PRINTER_RESET, OnPrinterReset);
        EventCenter.Instance.AddEventListener<int>(SBoxEventHandle.SBOX_SADNBOX_PRINTER_FONTSIZE, OnPrinterFontsize);
        EventCenter.Instance.AddEventListener<int>(SBoxEventHandle.SBOX_SADNBOX_PRINTER_MESSAGE, OnPrinterMessage);
        EventCenter.Instance.AddEventListener<int>(SBoxEventHandle.SBOX_SADNBOX_PRINTER_DATESET, OnPrinterDateSet);
        EventCenter.Instance.AddEventListener<SBoxDate>(SBoxEventHandle.SBOX_SADNBOX_PRINTER_DATEGET, OnPrinterDateGet);
        EventCenter.Instance.AddEventListener<int>(SBoxEventHandle.SBOX_SADNBOX_PRINTER_PAPERCUT, OnPrinterCutPaper);



    }






    /*
    public IEnumerator PurchaseCreditRequest(UInt32 operateType, long purchase)
    {
        bool requestSuccess = false;
        bool requestFail = false;

#if NEW_NET

        string rpcName = operateType == 1 ? RPCName.addCredit : RPCName.decreaseCredit;
        //operateType == 1 ? "/v0/purchase/add_credit" : "/v0/purchase/sub_credit";

        Dictionary<string, object> req = new Dictionary<string, object>
            {
                {"balance",purchase},
            };

        NetManager.Instance.Post(rpcName, req,
        (res) =>
        {

            requestSuccess = true;
            globalStore.newCredit = res["balance"].AsLong;
            BlackboardQueryUtils.SetMyCredit(globalStore.newCredit);
            MessageDispatcher.Dispatch("OnCreditEvent", new EventData<bool>("UpdateNaviCredit", true));
        },
        (error) =>
        {
            switch (error.errorCode)
            {
                default:
                    GlobalErrorHandler.GlobalError(error);
                    break;
            }

            requestFail = true;
        });
#else
            BagelCodeClientAPI.PurchaseCreditRequest(operateType, purchase,
                (response) =>
                {
                    requestSuccess = true;
                    if (response.userSyncInfo != null)
                    {
                        BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                        BlackboardQueryUtils.ApplyUserSyncInfo();
                    }
                },
                (error) =>
                {
                    switch (error.errorCode)
                    {
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                    }

                    requestFail = true;
                });
#endif

        yield return new WaitUntil(() => requestSuccess || requestFail);
    }
    */

    /*
    private void OnCoinOut(int coinCount)
    {
        if (coinOutNum - coinCount < 0)
            coinCount = coinOutNum;
        coinOutNum -= coinCount;
        StartCoroutine(PurchaseCreditRequest(2, coinCount * 100));
        if (coinOutNum == 0)
            CoinOutStop(0);
    }*/

    private void OnLightChange(EventData data)
    {
        /*switch (data.value)
        {
            case "lobby":
                //开灯
                SwitchOutStateOn((ulong)SBOX_SWITCH.SWITCH_UP);
                SwitchOutStateOn((ulong)SBOX_SWITCH.SWITCH_RED);
                SwitchOutStateOn((ulong)SBOX_SWITCH.SWITCH_GREEN);
                SwitchOutStateOn((ulong)SBOX_SWITCH.SWITCH_YELLOW);
                SwitchOutStateOn((ulong)SBOX_SWITCH.SWITCH_BET4);
                SwitchOutStateOn((ulong)SBOX_SWITCH.SWITCH_BET5);
                SwitchOutStateOn((ulong)SBOX_SWITCH.SWITCH_ENTER);
                SwitchOutStateOn((ulong)SBOX_SWITCH.SWITCH_AUTO);
                break;
            case "slots":
                //关灯
                SwitchOutStateOff((ulong)SBOX_SWITCH.SWITCH_UP);
                break;
            default:
                break;
        }*/
    }


    string MachineGetString(string key, string defaultValue)
    {
        return PlayerPrefs.GetString(key, defaultValue);
        //之后给为算法卡
    }

    void MachineSetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
        //之后给为算法卡
    }
}



/// <summary>
/// ## 纸钞机
/// </summary>
public partial class SBoxSanboxController : EventMonoSingleton<SBoxSanboxController>
{
    public void GetBillLst()
    {
#if UNITY_EDITOR
        MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_BILL_LIST_GET);
#else
        SBoxSandbox.BillListGet();
#endif
    }

    string addMoneyOrder = "";
    public List<string> sandBoxBillList;
    private void OnSboxSandBoxBillListGet(List<string> sandBoxBillList)
    {
        this.sandBoxBillList = sandBoxBillList;

        int i = 0;
        foreach (var item in sandBoxBillList)
        {
            Debug.Log($"【BillLst】sandBoxBillList: idx = {i}  val = {item}");
            i++;
        }

        if (this._taskTimer != null)
        {
            this._taskTimer.Stop();
            this._taskTimer.Dispose();
            this._taskTimer = null;
        }

        test_SetBillSelect();
    }

    [Button]
    public void test_SetBillSelect()
    {
        // 本地读取
        int data = 3; //存本地
#if UNITY_EDITOR
        MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_BILL_SELECT, data.ToString());
#else
        SBoxSandbox.BillSelect(data);
#endif
    }

    private void OnSboxSandBoxBillSelect(int res)
    {
        if (res == 0) //定时
        {
            //存本地
        }

        Debug.Log($"【BillLst】OnSboxSandBoxBillSelect  res = {res}");
    }

    int credit = 0;
    private void OnBillIn(int credit)
    {
        if (credit <= 0)
        {
            return;
        }

        this.credit = credit;

        Debug.Log($"【BillLst】OnBillIn  credit = {credit}");

        Dictionary<string, object> req = new Dictionary<string, object>
        {
            {"money",credit}, //要充值的美金
        };
        Debug.Log("请求充值");
        NetManager.Instance.Post(RPCName.creatAddMoneyOrder, req,
        (res) =>
        {
            //订单号
            if (res.HasKey("order_id") && (string)res["order_id"] != "" && (string)res["order_id"] != null)
            {
                // addMoneyOrder = res["order_id"].ToString();
                addMoneyOrder = (string)res["order_id"];
#if UNITY_EDITOR
                MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_BILL_APPROVE);
#else
            SBoxSandbox.BillApprove();
#endif
            }
            else
            {
#if UNITY_EDITOR
                credit = 0;
                MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_BILL_REJECT);
#else
            SBoxSandbox.BillReject();
#endif
            }
        },
        (error) =>
        {
            Debug.LogError(" 请求充值失败");
            credit = 0;
            MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_BILL_REJECT);
        });

    }

    JSONNode _remainAddMoneyOrders = null;

    JSONNode remainAddMoneyOrders
    {
        get {
            if(_remainAddMoneyOrders == null)
            {
                string str = MachineGetString("Server__RemainAddMoneyOrders", "{}");
                _remainAddMoneyOrders = JSONNode.Parse(str);
            }
            return _remainAddMoneyOrders;
        }
        set {  _remainAddMoneyOrders = value; }
    }



    private void OnBillStacked()
    {

        // 发订单号
        if (credit != 0 && addMoneyOrder!= "" && addMoneyOrder != null)
        {

            // 0 待处理，1正在处理
            JSONNode nd = JSONNode.Parse(string.Format("{{\"stamp\":{0},\"count\":{1},\"device_index\":{2}}}", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),credit,0));
            remainAddMoneyOrders.Add(addMoneyOrder, nd);
            MachineSetString("Server_RemainAddMoneyOrders", remainAddMoneyOrders.ToString());

            Dictionary<string, object> req = new Dictionary<string, object>
                {
                   {"money",credit}, //充入的美到
                   {"order_id",addMoneyOrder}, //订单号
                   {"device_index",0 } //纸钞机只有1个
                };
            Debug.Log("开始充值");
            NetManager.Instance.Post(RPCName.confirmAddMoneyOrder, req,
            (res) =>
            {
                Debug.Log($" 充值成功");

                string orderID = res["order_id"];
                remainAddMoneyOrders.Remove(orderID);
                MachineSetString("Server_RemainAddMoneyOrders", remainAddMoneyOrders.ToString());
            },
            (error) =>
            {
                SimpleJSON.JSONNode res1 = SimpleJSON.JSONNode.Parse(error.response);
                string order = res1.HasKey("param") && res1["param"].HasKey("order_id") ? res1["param"]["order_id"] : null;
                if (order != null)
                {
                    remainAddMoneyOrders.Remove(order);
                    MachineSetString("Server_RemainAddMoneyOrders", remainAddMoneyOrders.ToString());
                }

                Debug.LogError(" 充值失败");

            });
            addMoneyOrder = "";
            credit = 0;
        }
    }
}

/// <summary>
/// ## 打印机
/// </summary>
public partial class SBoxSanboxController : EventMonoSingleton<SBoxSanboxController>
{


    bool isPrinterInit = false;
    public void GetPrintList()
    {
#if UNITY_EDITOR
        MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_PRINTER_LIST_GET);
#else
        SBoxSandbox.PrinterListGet();
#endif
    }

    void OnPrinterListGet(List<string> strList)
    {
        if (this._taskTimer != null)
        {
            this._taskTimer.Stop();
            this._taskTimer.Dispose();
            this._taskTimer = null;
        }

        strList.ForEach(str => Debug.Log(str));
        int data = 0;
#if UNITY_EDITOR
        MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_PRINTER_SELECT, data.ToString());
#else
        SBoxSandbox.PrinterSelect(data);
#endif
    }

    void OnPrinterSelect(int result)
    {
        if (result == 0)
        {
            Debug.Log("【printer】: : select succeed");

#if UNITY_EDITOR
            MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_PRINTER_RESET);
#else
        SBoxSandbox.PrinterReset();
#endif

        }
        else
        {
            Debug.LogWarning($"【printer】: : 打印机选择失败  index = 0 ");
        }
    }

    void OnPrinterReset(int result)
    {
        if (result == 0)
        {
            isPrinterInit = true;
        }
        else
        {
            Debug.LogWarning("【printer】: : 打印机复位失败");
        }
    }


    /// <summary>
    /// 剪纸
    /// </summary>
    /// <param name="result"></param>
    [Button]
    public void test_PrinterDateSet(int result)
    {
        Debug.Log("【printer】: setFontSize succeed");
        SBoxDate sBoxDate = new SBoxDate()
        {
            result = 0,
            month = DateTime.Now.Month,
            day = DateTime.Now.Day,
            hours = DateTime.Now.Hour,
            minutes = DateTime.Now.Minute,
            seconds = DateTime.Now.Second
        };

#if UNITY_EDITOR

        MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_PRINTER_DATESET, JsonConvert.SerializeObject(sBoxDate));
#else
            SBoxSandbox.PrinterDateSet(sBoxDate);
#endif
    }

    void OnPrinterDateSet(int result)
    {
        if (result == 0)
        {
            Debug.Log("【printer】: set Date succeed");
        }
    }



    [Button]
    public void test_PrinterCutPaper(int result)
    {
#if UNITY_EDITOR
        MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_PRINTER_PAPERCUT);
#else
        SBoxSandbox.PrinterPaperCut();
#endif

    }

    void OnPrinterCutPaper(int result)
    {
        
        if (result == 0)
        {
            Debug.Log("【printer】: cut paper succeed");
        }
    }


    [Button]
    public void test_PrinterDateGet(SBoxDate sBoxDate)
    {
#if UNITY_EDITOR
        MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_PRINTER_DATEGET);
#else
        SBoxSandbox.PrinterDateGet();
#endif
    }


    void OnPrinterDateGet(SBoxDate sBoxDate)
    {
        if (sBoxDate.result == 0)
        {
        }
    }


    [Button]
    public void test_PrinterMessage(int credit = 10000)
    {
        //Debug.Log($"【printer】: All dollar = {BlackboardUtils.FindVariable<long>(null, "/me/credit").value / 1000}");
        int dollar = credit/ 1000;
        string testMsg = "        K3K\r\n" +
            $"${dollar}\r\n" +
            $"Order number: {123456}\r\n" +
            $"Distributor: {"aa"}\r\n" +
            $"Business: {"bb"}\r\n";
#if UNITY_EDITOR

        MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_PRINTER_MESSAGE, testMsg);
#else
        SBoxSandbox.PrinterMessage(testMsg);
#endif

    }



    string printOrderId;
    int printMoney;


    Action printFunc = null;

    [Button]
    public void PrintMoneyOrder()
    {

        if (isTask("StartPrint"))
            return;
        DoTask("StartPrint",() => { }, 5000); //延时避免重复触发

        printFunc = CreatPrint();
        printFunc();
    }
    
    private void OnCreatePrintOrder(ParadoxNotion.EventData eventData)
    {

        JSONNode res = eventData.value as JSONNode;

        /*Debug.Log($"==@ isRemainData = {isRemainData}");
        if (isRemainData)
            return;*/

        if (res["err"] == 0)
        {
            Debug.Log("@【printer】请求退美元成功");

            printMoney = (int)res["earn_money"];
            printOrderId = (string)res["order_id"];
            agent_name = (string)res["agent_name"];

            if (printFunc != null)
            {
                printFunc();
            }
        }
        else
        {
            Debug.Log("@【printer】请求退美元失败");
        }

    }


    string agent_name = "";
    Action CreatPrint()
    {
        int _next = 0;
        Action FUNC = null;

        agent_name = "";

        FUNC  = () =>{

            if (!isPrinterInit)
            {
                Debug.LogError(" 打印机初始化失败");
                return;
            }
            int fontSize = 5;
            string testMsg = "";

            Dictionary<string, object> req;

            switch (_next)
            {
                case 0:

                    req = new Dictionary<string, object> { };
                    Debug.Log("@【printer】请求退美元");
                    NetManager.Instance.SendMsg(RPCName.createPrintOrder, req);
                    /*
                    NetManager.Instance.Post(RPCName.createPrintOrder, req,
                    (res) =>
                    {
                        Debug.Log("@【printer】请求退美元成功");

                        printMoney = res["earn_money"];
                        printOrderId = res["order_id"];

                        agent_name = res["agent_name"];

                        FUNC();
                    },
                    (error) =>
                    {
                        Debug.Log("@【printer】请求退美元失败");
                    });*/

                    break;
                case 1:
                    fontSize = 5;
#if UNITY_EDITOR
                    MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_PRINTER_FONTSIZE, fontSize.ToString());
#else
                    SBoxSandbox.PrinterFontSize(fontSize);
#endif

                    Debug.Log($"@【printer】fonSize = {fontSize}");
                    break;

                case 2:

                    testMsg = "\t\tK3K\r\n" +
                        $"${printMoney}\r\n" +
                        $"Order number: \r\n" +
                        $"{printOrderId}\r\n" +
                        $"Distributor: {agent_name}\r\n" +
                        $"Business: {"--"}\r\n";
#if UNITY_EDITOR

                    MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_PRINTER_MESSAGE, testMsg);
#else
                    SBoxSandbox.PrinterMessage(testMsg);
#endif
                    Debug.Log($"@【printer】testMsg = {testMsg}");
                    break;
                case 3:

                    req = new Dictionary<string, object>
                    {
                        { "order_id",printOrderId},
                        { "money",printMoney}
                    };
                    Debug.Log("@【printer】确认退美元");
                    NetManager.Instance.Post(RPCName.confirmPrintOrder, req,
                    (res) =>
                    {
                        string orderID = (string)res["order_id"];
                        remainPrinterOrders.Remove(orderID); 
                        MachineSetString("Server_RemainPrinterOrders", remainPrinterOrders.ToString());
                    },
                    (error) =>
                    {
                        SimpleJSON.JSONNode res1 = SimpleJSON.JSONNode.Parse(error.response);
                        string order = res1.HasKey("param") && res1["param"].HasKey("order_id") ? res1["param"]["order_id"] : null;
                        if (order != null)
                        {
                            remainPrinterOrders.Remove(order);
                            MachineSetString("Server_RemainPrinterOrders", remainPrinterOrders.ToString());
                        }
                        Debug.LogError(" 确认退美元失败");
                    });

                    printMoney = 0;
                    printOrderId = "";

                    break;

                default:
                    printMoney = 0;
                    printOrderId = "";
                    break;

            }



            /*
            switch (_next)
            {
                case 0:

                    req = new Dictionary<string, object> { };
                    Debug.Log("@【printer】请求退美元");
                    NetManager.Instance.Post(RPCName.createPrintOrder, req,
                    (res) =>
                    {
                        printMoney = res["money"];
                        printOrderId = res["order_id"];

                        agent_name = res["agent_name"];

                        FUNC();
                    },
                    (error) =>
                    {
                        Debug.LogError(" 请求退美元失败");
                    });

                    break;
                case 1:
                    fontSize = 15;
#if UNITY_EDITOR
                    MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_PRINTER_FONTSIZE, fontSize.ToString());
#else
                    SBoxSandbox.PrinterFontSize(fontSize);
#endif

                    Debug.Log($"@【printer】fonSize = {fontSize}");
                    break;
                case 2:
                    testMsg = ".       K3K\r\n";
#if UNITY_EDITOR

                    MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_PRINTER_MESSAGE, testMsg);
#else
                    SBoxSandbox.PrinterMessage(testMsg);
#endif
                    Debug.Log($"@【printer】testMsg = {testMsg}");
                    break;
                case 3:

                    fontSize = 30;
#if UNITY_EDITOR
                    MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_PRINTER_FONTSIZE, fontSize.ToString());
#else
                    SBoxSandbox.PrinterFontSize(fontSize);
#endif
                    Debug.Log($"@【printer】fonSize = {fontSize}");
                    break;

                case 4:
                    testMsg = $".    ${printMoney}\r\n";
#if UNITY_EDITOR

                    MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_PRINTER_MESSAGE, testMsg);
#else
                    SBoxSandbox.PrinterMessage(testMsg);
#endif
                    Debug.Log($"@【printer】testMsg = {testMsg}");
                    break;
                case 5:

                    fontSize = 5;
#if UNITY_EDITOR
                    MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_PRINTER_FONTSIZE, fontSize.ToString());
#else
                    SBoxSandbox.PrinterFontSize(fontSize);
#endif
                    Debug.Log($"@【printer】fonSize = {fontSize}");
                    break;
                case 6:

                    testMsg = $"Order number: \r\n" +
                    $"{printOrderId}\r\n" +
                    $"Distributor: {agent_name}\r\n" +
                    $"Business: {"--"}\r\n";
 #if UNITY_EDITOR
                MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_PRINTER_MESSAGE, testMsg);

#else
                SBoxSandbox.PrinterMessage(testMsg);
#endif
                    Debug.Log($"@【printer】testMsg = {testMsg}");
                    break;

                case 7:

                    req = new Dictionary<string, object>
                    {
                        { "order_id",printOrderId},
                        { "money",printMoney}
                    };
                    Debug.Log("@【printer】确认退美元");
                    NetManager.Instance.Post(RPCName.confirmPrintOrder, req,
                    (res) =>
                    {
                    },
                    (error) =>
                    {
                        Debug.LogError(" 确认退美元失败");
                    });
                    
                    printMoney = 0;
                    printOrderId = "";

                    break;

                default:
                    printMoney = 0;
                    printOrderId = "";
                    break;

            }*/

            _next++;
        };

        return FUNC;
    }

    private int step = 0;





    JSONNode _remainPrinterOrders = null;
    JSONNode remainPrinterOrders
    {
        get
        {
            if (_remainPrinterOrders == null)
            {
                string str = MachineGetString("Server__RemainPrinterOrders", "{}");
                _remainPrinterOrders = JSONNode.Parse(str);
            }
            return _remainPrinterOrders;
        }
        set { _remainPrinterOrders = value; }
    }

    void OnPrinterFontsize(int result)
    {
        if (result == 0 )
        {
            if (printFunc != null)
                printFunc();
        }
        else
        {
            Debug.LogWarning("【printer】: 打印机字体设置失败");
            printFunc = null;
        }
    }


    void OnPrinterMessage(int result)
    {
        if (result == 0)
        {

            if (printFunc != null &&  printOrderId != "" && printOrderId != null)
            {
                JSONNode node = JSONNode.Parse(string.Format("{{\"stamp\":{0},\"count\":{1}}}", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), printMoney));
                remainPrinterOrders.Add(printOrderId, node);
                MachineSetString("Server_RemainPrinterOrders", remainPrinterOrders.ToString());

                printFunc();
            }
            else
            {
                Debug.LogWarning(@"【machine】打印机被误触发了");
            }

        }
        else
        {
            Debug.LogWarning("【printer】: 打印机打印失败");
            printFunc = null;
        }
    }
}


public partial class SBoxSanboxController : EventMonoSingleton<SBoxSanboxController>
{

    Dictionary<string,Coroutine> coroutineDic = new Dictionary<string, Coroutine>();

    bool isTask(string taskName)
    {
        if (coroutineDic.ContainsKey(taskName) && coroutineDic[taskName] != null)
        {
            return true;
        }
        return false;
    }

    public void DoTask(string taskName, Action cb, int ms = 0)
    {
        ClearTask(taskName);
        coroutineDic.Add(taskName, StartCoroutine(_doTask(taskName, cb, ms)));
    }
    public void ClearTask(string taskName)
    {
        //StopCoroutine("doTask");
        if (coroutineDic.ContainsKey(taskName))
        {
            StopCoroutine(coroutineDic[taskName]);
            coroutineDic.Remove(taskName);
        }
    }

    public void ClearAllTask()
    {
        foreach(var item in coroutineDic)
        {
            StopCoroutine(item.Value);
        }
        coroutineDic.Clear();
    }

    IEnumerator _doTask(string taskName, Action cb, int ms = 0)
    {
        yield return new WaitForSeconds(ms / 1000f);
        if (cb != null)
            cb();
        coroutineDic.Remove(taskName);
    }
}


/// <summary>
/// ## 上下分
/// </summary>
public partial class SBoxSanboxController : EventMonoSingleton<SBoxSanboxController>
{

    public void AddCredit(long count, Action onsuccess = null)
    {
        PurchaseCreditRequest(1, count, onsuccess);
    }
    public void DecreaseCredit(long count, Action onsuccess = null)
    {
        PurchaseCreditRequest(2, count, onsuccess);
    }

    private void PurchaseCreditRequest(int operateType, long count, Action onsuccess = null)
    {
        string rpcName = operateType == 1 ? RPCName.addCredit : RPCName.decreaseCredit;
        Dictionary<string, object> req = new Dictionary<string, object>
        {
            {"count",count},
        };

        NetManager.Instance.Post(rpcName, req,
        (res) =>
        {
            globalStore.newCredit = res["balance"].AsLong;
            BlackboardQueryUtils.SetMyCredit(globalStore.newCredit);
            MessageDispatcher.Dispatch("OnCreditEvent", new EventData<bool>("UpdateNaviCredit", true));

            if (onsuccess != null)
                onsuccess();
        },
        (error) =>
        {
            switch (error.errorCode)
            {
                default:
                    GlobalErrorHandler.GlobalError(error);
                    break;
            }
        });
    }
}

/// <summary>
/// ## 投币机
/// </summary>
public partial class SBoxSanboxController : EventMonoSingleton<SBoxSanboxController>
{

    int lastCoinInId = -1;

    JSONNode _remainCoinInNumLst = null;

    readonly string DEFINE_COIN_IN_NUM = "{\"0\":{\"stamp\":0,\"count\":0},\"1\":{\"stamp\":0,\"count\":0},\"2\":{\"stamp\":0,\"count\":0}}";
    JSONNode remainCoinInNumLst
    {
        get
        {
            if (_remainCoinInNumLst == null)
            {     
                string str = MachineGetString("Server__RemainCoinInNum", DEFINE_COIN_IN_NUM);
                _remainCoinInNumLst = JSONNode.Parse(str);
            }
            return _remainCoinInNumLst;
        }
        set {_remainCoinInOrders = value;}
    }


    JSONNode _remainCoinInOrders = null;
    JSONNode remainCoinInOrders
    {
        get
        {
            if (_remainCoinInOrders == null)
            {
                string str = MachineGetString("Server__RemainCoinInOrders", "{}");
                _remainCoinInOrders = JSONNode.Parse(str);
            }
            return _remainCoinInOrders;
        }
        set {_remainCoinInOrders = value;}
    }

    private void OnCoinIn(CoinInData coinInData)
    {
        Debug.LogError($"CoinIn id = {coinInData.id} value = {coinInData.value}");

        if (coinInData.value <= 0)
        {
            return;
        }


        GSManager.Instance.GetHandler("Machine_Coin_In").Play();

        remainCoinInNumLst[$"{coinInData.id}"]["count"] += coinInData.value;
        remainCoinInNumLst[$"{coinInData.id}"]["stamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        MachineSetString("Server_RemainCoinInNum", remainCoinInNumLst.ToString());
      

        if (lastCoinInId != -1 && lastCoinInId != coinInData.id)
        {
            ClearTask("CoinIn");
            AddCoin(lastCoinInId);
            lastCoinInId = coinInData.id;
        }

        lastCoinInId = coinInData.id;
        DoTask("CoinIn",
        () =>
        {
            // 发送旧数据  lastCoinInId  
            AddCoin(lastCoinInId);
            lastCoinInId = -1;
        }, 301);
        
    }


    private void AddCoin(int id)//,int count)
    {
       NetManager.Instance.Post(RPCName.creatAddCoinOrder, new Dictionary<string, object>(),
      (res) =>
      {

          if (res.HasKey("order_id"))
          {

              string order = res["order_id"];
              int count = remainCoinInNumLst[$"{id}"]["count"];
              remainCoinInNumLst[$"{id}"]["count"] = 0;
              remainCoinInNumLst[$"{id}"]["stamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
              MachineSetString("Server_RemainCoinInNum", remainCoinInNumLst.ToString());

              JSONNode nd = SimpleJSON.JSONNode.Parse(string.Format("{{\"stamp\":{0},\"id\":{1},\"count\":{2}}}", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), id, count));
              //Debug.LogWarning($"order_id  =  {nd.ToString()}");

              //Debug.LogWarning($" remainCoinInOrders = {remainCoinInOrders.ToString()}   res.order_id = {order}");
              remainCoinInOrders.Add(order, nd);
              MachineSetString("Server_RemainCoinInOrders", remainCoinInOrders.ToString());

              //Debug.LogWarning($" remainCoinInOrders = {remainCoinInOrders.ToString()}   res.order_id = {order}");

              Dictionary<string, object> req = new Dictionary<string, object>
                {
                    {"device_index",id},
                    {"order_id",order},
                    {"money",count}
                };
              NetManager.Instance.SendMsg(RPCName.confirmAddCoinOrder, req);
          }
      },
      (error) =>
      {
          CloseMask();
          Debug.LogError(" 查询退票个数失败");
      });

    }



    void OnConfirmAddCoin(EventData eventData)
    {
        JSONNode res = eventData.value as JSONNode;

        /*if (isRemainData)
            return;*/

        if (res["err"] == 0)
        {
            Debug.Log($"【投币】 上行-投币成功  投币个数 = {res["count"].AsLong}  设备号 = {res["device_index"]}");

            string order = res["order_id"];

            if (remainCoinInOrders.HasKey(order))
            {
                remainCoinInOrders.Remove(order);
                MachineSetString("Server_RemainCoinInOrders", remainCoinInOrders.ToString());
            }
        }
        else
        {
            //bool isFinished = res.HasKey("param") && res["param"].HasKey("have_been_accept") && res["param"]["have_been_accept"] == 1;
            string order = res.HasKey("param") && res["param"].HasKey("order_id") ? res["param"]["order_id"] : null;
            if (remainCoinInOrders.HasKey(order))
            {
                remainCoinInOrders.Remove(order);
                MachineSetString("Server_RemainCoinInOrders", remainCoinInOrders.ToString());
            }

            Debug.LogError(" 投币失败");
        }
    }

}




/// <summary>
/// ## 退票机
/// </summary>
public partial class SBoxSanboxController : EventMonoSingleton<SBoxSanboxController>
{


    Coroutine _taskCoinOutOutTime = null;

    public void DoTaskCoinOutOutTime(Action cb, int ms = 0)
    {
        ClearTaskCoinOutOutTime();
        _taskCoinOutOutTime = StartCoroutine(_DoTaskCoinOutOutTime(cb, ms));
    }
    public void ClearTaskCoinOutOutTime()
    {
        //StopCoroutine("doTask");
        if (_taskCoinOutOutTime != null)
        {
            StopCoroutine(_taskCoinOutOutTime);
            _taskCoinOutOutTime = null;
        }
    }
    IEnumerator _DoTaskCoinOutOutTime(Action cb, int ms = 0)
    {
        yield return new WaitForSeconds(ms / 1000f);
        if (cb != null)
            cb();
        _taskCoinOutOutTime = null;
    }


    bool isCoinOuting = false;

    void ResetArg()
    {
        this.finishCoinOutNum = 0;
        this.coinOutNum = 0;
        this.coinOutOrder = "";
        this.cointOutRate = 0;
    }

    int coinOutNum = 0;
    string coinOutOrder = "";
    int cointOutRate = 0;
    public void StartCoinOut()
    {

        if (BlackboardQueryUtils.IsIngame() && BlackboardQueryUtils.IsSpin())
        {
            ErrorPopupInfo info = new ErrorPopupInfo();
            info.text = "<size=32>No refund is allowed while the game is in progress.</size>";
            info.type = ErrorPopupType.OK;
            info.buttonText1 = "OK";
            info.callback1 = delegate
            {
                //Debug.Log("i am here1");
            };
            ErrorPopupHandler.Instance.OpenError(info);
            return;
        }


        if (isCoinOuting)
            return;
        isCoinOuting = true;


        ResetArg();


        Dictionary<string, object> req = new Dictionary<string, object> { };
        //Debug.Log("请求投币");
        NetManager.Instance.Post(RPCName.createCoinOutOrder, req,
        (res) =>
        {

            //{"protocol_key":"agent_create_outcredit_ticket_order","data":{"order_id":"6884773f-465e-41c3-b0f1-875a148c4754","agent_name":"agent1","money":1,"outcredit_rate_of_exchange":1000,"err":0}}

            //{order_id:order_id,agent_name:agent_name,money:money}

            Debug.Log($" res = {res.ToString()}");
            Debug.Log($" 允许退票个数 = {res["money"]}");

            int num = res["money"];
            if (num > 0)
            {

                //“弹窗退票中”
                OpenMask();

                this.coinOutNum = num;
                this.coinOutOrder = res["order_id"];
                this.cointOutRate = res["outcredit_rate_of_exchange"];

#if UNITY_EDITOR
                CointOutData cointOutData = new CointOutData()
                {
                    id = 0,
                    count = this.coinOutNum,
                    type = 0
                };
                MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_COIN_OUT_START, JsonConvert.SerializeObject(cointOutData));

#else
                CoinOutStart(0, this.coinOutNum, 0);
#endif
                DoTaskCoinOutOutTime(
                    () =>
                    {
                        Debug.Log("退票超时!");
                        StopCoinOut();
                        CloseMask();
                    }, 3001);
            }
            else
            {
                CloseMask();
                Debug.Log("退票积分不足");
            }
        },
        (error) =>
        {
            CloseMask();
            Debug.LogError(" 查询退票个数失败");
        });
    }


    [Button]
    void OpenMask()
    {
        isCoinOuting = true;
        //打开弹窗
        /* ErrorPopupInfo info = new ErrorPopupInfo();
         info.text = $"<size=32>Processing ticket refund</size>";
         info.type = ErrorPopupType.TextOnly;
         ErrorPopupHandler.Instance.OpenError(info);*/
    }

    [Button]
    void CloseMask()
    {
        //关闭“弹窗退票中”
        isCoinOuting = false;

        //EventSender.SendGlobalEvent("OnClose"); //关闭弹窗
    }



    JSONNode _remainCoinOutOrders = null;
    JSONNode remainCoinOutOrders
    {
        get
        {
            if (_remainCoinOutOrders == null)
            {
                string str = MachineGetString("Server__RemainCoinOutOrders", "{}");
                _remainCoinOutOrders = JSONNode.Parse(str);
            }
            return _remainCoinOutOrders;
        }
        set { _remainCoinOutOrders = value; }

    }


    int finishCoinOutNum = 0;

    private void OnCoinOut(int coinOutNum01)
    {
        if (coinOutNum01 <= 0)
            return;

        finishCoinOutNum += coinOutNum01;

        if (finishCoinOutNum > this.coinOutNum)
        {
            finishCoinOutNum = this.coinOutNum;
        }

        //剩余 存入缓存
        if (!remainCoinOutOrders.HasKey(coinOutOrder))
        {
            SimpleJSON.JSONNode node = SimpleJSON.JSONNode.Parse("{}");
            node.Add("stamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            node.Add("count", finishCoinOutNum);
            node.Add("rate", cointOutRate);
            remainCoinOutOrders.Add(coinOutOrder, node);
        }
        else
        {
            remainCoinOutOrders[coinOutOrder]["count"] = finishCoinOutNum;
        }
        MachineSetString("Server_RemainCoinOutOrders", remainCoinOutOrders.ToString());

        Debug.LogWarning($"OnCoinOut 被调用 coinOutNum = {coinOutNum01} 累计数量 = {this.finishCoinOutNum} rate = {cointOutRate}");



        // 前端刷新分数显示
        ChangeCreditShow();


        DoTaskCoinOutOutTime(
        () =>
        {
            StopCoinOut();

            //弹窗，通知退票失败，
            //--还欠多少个币
            Dictionary<string, object> req = new Dictionary<string, object>
            {
                {"money",finishCoinOutNum}, //退票个数
                {"order_id",coinOutOrder}
            };
            NetManager.Instance.SendMsg(RPCName.confirmCoinOutOrder, req);

            Debug.Log("退票超时,结束");
            //isCoinOuting = false; //?
        }, 3001);
    }


    void OnConfirmCoinOutOrder(EventData eventData)
    {
        JSONNode res = eventData.value as JSONNode;

        /* Debug.Log($"==@ isRemainData = {isRemainData}");
         if (isRemainData)
             return;*/

        if (res["err"] == 0)
        {

            Debug.Log($"【退票】 上行-退票成功 退票个数 = {res["money"].AsLong}  退票订单 = {res["order_id"]}");

            //remainCoinOutOrders.Remove($"{res["order_id"]}");  //会有问题
            remainCoinOutOrders.Remove((string)res["order_id"]);
            MachineSetString("Server_RemainCoinOutOrders", remainCoinOutOrders.ToString());
            //Debug.Log($"【退票】检查 退票数据 = {MachineGetString("Server__RemainCoinOutOrders", "???")}    @@@ = {remainCoinOutOrders.ToString()}");
            CloseMask();
            ChangeCreditShow();
        }
        else
        {
            //bool isFinished = res.HasKey("param") && res["param"].HasKey("have_been_accept") && res["param"]["have_been_accept"] == 1;//订单是否完成

            string order = res.HasKey("param") && res["param"].HasKey("order_id") ? res["param"]["order_id"] : null;

            if (order != null && remainCoinOutOrders.HasKey(order))
            {
                remainCoinOutOrders.Remove(order);
                MachineSetString("Server_RemainCoinOutOrders", remainCoinOutOrders.ToString());
                ChangeCreditShow();
            }
        }
    }



    private void StopCoinOut()
    {
#if UNITY_EDITOR
        MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_COIN_OUT_STOP, "0");
#else
        CoinOutStop(0);
#endif
    }


    public long getCoinOutCredit()
    {
        long res = 0;
        foreach (KeyValuePair<string, SimpleJSON.JSONNode> node in remainCoinOutOrders)
        {
            long orderTime = node.Value["stamp"].AsLong;
            long coinOutNum = node.Value["count"].AsLong;
            long rate = node.Value["rate"].AsLong;

            if (coinOutNum > 0)
            {
                res -= coinOutNum * rate;
            }
        }
        return res;
    }

    public void ChangeCreditShow()
    {
        // 前端刷新分数显示
        if (!isTask("ChangeCredit"))
        {
            DoTask("ChangeCredit", () =>
            {
                //test_ShowRemain();
                Debug.Log("@@修改金额");
                NetManager.Instance.SetMyCredit();
            }, 500);
        }
    }
}

/// <summary>
/// ## 重启时把剩余数量发给服务器
/// </summary>
public partial class SBoxSanboxController : EventMonoSingleton<SBoxSanboxController>
{


    //bool isRemainData = false;


    long initStamp = 0;


    private IEnumerator _SendRemain(long _initStamp)
    {

        bool isFirst = true;  //重连网络或上电时，将缓存立马发给服务器同步。

        yield return new WaitUntil(() => globalStore.gameState == GameState.Hall || globalStore.gameState == GameState.Game);

#if !UNITY_EDITOR
        if (!ApplicationSettings.Instance.isMachine)
            yield break;
#endif

        //yield return new WaitUntil(() =>globalStore.gameState == GameState.Hall || globalStore.gameState == GameState.Game);

        Debug.Log($"【重启订单补发】：开始重发 {_initStamp}");

        test_ShowRemain();

        while (true)  //循环清除订单
        {

            yield return new WaitUntil(() => globalStore.gameState == GameState.Hall || globalStore.gameState == GameState.Game);


            if (initStamp != _initStamp)
            {
                Debug.Log($"退出订单补发循环 {_initStamp}");
                yield break;
            }


            Debug.Log($"@【查询订单缓存】{_initStamp}");

            /*
            while (true)
            {
                yield return new WaitForSeconds(3);

                if (!NetManager.Instance.isHasRequest(RPCName.confirmCoinOutOrder)
                    && !NetManager.Instance.isHasRequest(RPCName.confirmAddCoinOrder)
                    && !NetManager.Instance.isHasRequest(RPCName.confirmAddMoneyOrder)
                    && !NetManager.Instance.isHasRequest(RPCName.confirmPrintOrder)

                    && !NetManager.Instance.isHasRequest(RPCName.creatAddCoinOrder)
                    && !NetManager.Instance.isHasRequest(RPCName.creatAddMoneyOrder)
                    && !NetManager.Instance.isHasRequest(RPCName.createCoinOutOrder)
                    && !NetManager.Instance.isHasRequest(RPCName.createPrintOrder)
                {
                    break;
                }
            }*/


            long nowTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            bool isChange = false;


            if (!NetManager.Instance.isHasRequest(RPCName.creatAddCoinOrder))
            {
                foreach (KeyValuePair<string, SimpleJSON.JSONNode> node in remainCoinInNumLst)
                {

                    long orderTime = node.Value["stamp"].AsLong;
                    int id = int.Parse(node.Key);
                    int count = (int)node.Value["count"];
                    //Debug.Log($"@【补发 - 投币余量??】{node.Key} count = {count} time =  {nowTime} - {orderTime} ==  {nowTime - orderTime} ");
                    if (count > 0 && (nowTime - orderTime > 10000  || isFirst)) 
                    {

                        Debug.Log($"@【补发 - 投币余量】{node.Key} = {count}");

                        NetManager.Instance.Post(RPCName.creatAddCoinOrder, new Dictionary<string, object>(),
                        (res) =>
                        {

                            string order = res["order_id"];
                            remainCoinInNumLst[$"{id}"]["count"] = 0;
                            remainCoinInNumLst[$"{id}"]["stamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                            MachineSetString("Server_RemainCoinInNum", remainCoinInNumLst.ToString());

                            JSONNode nd = JSONNode.Parse(string.Format("{{\"stamp\":{0},\"id\":{1},\"count\":{2}}}", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), id, count));
                            remainCoinInOrders.Add(order, nd);
                            MachineSetString("Server_RemainCoinInOrders", remainCoinInOrders.ToString());

                            Dictionary<string, object> req = new Dictionary<string, object>
                            {
                            {"device_index",id},
                            {"order_id",order},
                            {"money",count}
                            };
                            NetManager.Instance.Post(RPCName.confirmAddCoinOrder, req,
                            (res1) =>
                            {
                                remainCoinInOrders.Remove((string)res1["order_id"]);
                                MachineSetString("Server_RemainCoinInOrders", remainCoinInOrders.ToString());
                            },
                            (error) =>
                            {
                                SimpleJSON.JSONNode res1 = SimpleJSON.JSONNode.Parse(error.response);
                                string od = res1.HasKey("param") && res1["param"].HasKey("order_id") ? res1["param"]["order_id"] : null;
                                if (od != null)
                                {
                                    remainCoinInOrders.Remove(od);
                                    MachineSetString("Server_RemainCoinInOrders", remainCoinInOrders.ToString());
                                }
                            });

                        },
                        (error) =>
                        {
                            //CloseMask();
                            //Debug.LogError(" 查询退票个数失败");
                        });
                    }
                }
            }


            foreach (KeyValuePair<string, SimpleJSON.JSONNode> node in remainCoinOutOrders)
            {
                long orderTime = node.Value["stamp"].AsLong;
                long coinOutNum = node.Value["count"].AsLong;

                if (coinOutNum > 0  && (nowTime - orderTime > 10000 || isFirst))
                {
                    isChange = true;
                    node.Value["stamp"] = nowTime;


                    Debug.Log($"@【补发 - 退票订单】：{node.Key} - {coinOutNum}");

                    Dictionary<string, object> req = new Dictionary<string, object>
                    {
                        {"money",coinOutNum}, //出票个数
                        {"order_id",(string)node.Key}
                    };

                    NetManager.Instance.Post(RPCName.confirmCoinOutOrder, req,
                    (res) =>
                    {
                        remainCoinOutOrders.Remove((string)res["order_id"]);
                        MachineSetString("Server_RemainCoinOutOrders", remainCoinOutOrders.ToString());
                        ChangeCreditShow();
                    },
                    (error) =>
                    {
                        SimpleJSON.JSONNode res1 = SimpleJSON.JSONNode.Parse(error.response);
                        string order = res1.HasKey("param") && res1["param"].HasKey("order_id") ? res1["param"]["order_id"] : null;
                        if (order != null)
                        {
                            remainCoinOutOrders.Remove(order);
                            MachineSetString("Server_RemainCoinOutOrders", remainCoinOutOrders.ToString());
                            ChangeCreditShow();
                        }
                    });
                }
            }

            if (isChange)
            {
                isChange = false;
                MachineSetString("Server_RemainCoinOutOrders", remainCoinOutOrders.ToString());
            }


            foreach (KeyValuePair<string, SimpleJSON.JSONNode> node in remainCoinInOrders)
            {
                long orderTime = node.Value["stamp"].AsLong;
                string order = node.Key;
                int id = node.Value["id"];
                int count = node.Value["count"];

                if (count > 0 && (nowTime - orderTime > 10000 || isFirst))
                {

                    isChange = true;
                    node.Value["stamp"] = nowTime;

                    Debug.Log($"@【补发 - 投币订单】 {node.Key}  - count : {count} - device_index {id} ");

                    Dictionary<string, object> req = new Dictionary<string, object>
                        {
                            {"device_index",id},
                            {"order_id",order},
                            {"money",count}
                        };
                    NetManager.Instance.Post(RPCName.confirmAddCoinOrder, req,
                    (res1) =>
                    {

                        string order1 = res1["order_id"];
                        remainCoinInOrders.Remove(order1);
                        MachineSetString("Server_RemainCoinInOrders", remainCoinInOrders.ToString());
                    },
                    (error) =>
                    {

                        SimpleJSON.JSONNode res1 = SimpleJSON.JSONNode.Parse(error.response);
                        string od = res1.HasKey("param") && res1["param"].HasKey("order_id") ? res1["param"]["order_id"] : null;
                        if (od != null)
                        {
                            remainCoinInOrders.Remove(od);
                            MachineSetString("Server_RemainCoinInOrders", remainCoinInOrders.ToString());
                        }
                    });

                }

            }

            if (isChange)
            {
                isChange = false;
                MachineSetString("Server_RemainCoinInOrders", remainCoinInOrders.ToString());
            }




            //纸钞订单
            foreach (KeyValuePair<string, SimpleJSON.JSONNode> node in remainAddMoneyOrders)
            {

                long orderTime = node.Value["stamp"].AsLong;
                string addMoneyOrder = node.Key;
                int credit = node.Value["count"];
                int deviceIndex = node.Value["device_index"];
                // 发订单号
                if (credit > 0 && nowTime - orderTime > 10000)
                {
                    isChange = true;
                    node.Value["stamp"] = nowTime;

                    Debug.Log($"@【补发 - 纸钞订单】：{node.Key}");

                    Dictionary<string, object> req = new Dictionary<string, object>
                    {
                       {"money",credit}, //充入的美到
                       { "order_id",addMoneyOrder}, //订单号
                       {"device_index",deviceIndex } //纸钞机只有1个
                    };
                    NetManager.Instance.Post(RPCName.confirmAddMoneyOrder, req,
                    (res) =>
                    {
                        string orderID = res["order_id"];
                        remainAddMoneyOrders.Remove(orderID);
                        MachineSetString("Server_RemainAddMoneyOrders", remainAddMoneyOrders.ToString());
                    },
                    (error) =>
                    {
                        SimpleJSON.JSONNode res1 = SimpleJSON.JSONNode.Parse(error.response);
                        string order = res1.HasKey("param") && res1["param"].HasKey("order_id") ? res1["param"]["order_id"] : null;
                        if (order != null)
                        {
                            remainAddMoneyOrders.Remove(order);
                            MachineSetString("Server_RemainAddMoneyOrders", remainAddMoneyOrders.ToString());
                        }
                    });
                }
            }

            if (isChange)
            {
                isChange = false;
                MachineSetString("Server_RemainAddMoneyOrders", remainAddMoneyOrders.ToString());
            }


            //打印机订单
            foreach (KeyValuePair<string, SimpleJSON.JSONNode> node in remainPrinterOrders)
            {
                long orderTime = node.Value["stamp"].AsLong;
                int printMoney = node.Value["count"];
                string printOrderId = node.Key;
                if (printMoney > 0 && (nowTime - orderTime > 10000 || isFirst))
                {
                    isChange = true;
                    node.Value["stamp"] = nowTime;


                    Debug.Log($"@【补发 - 打印机订单】：{node.Key} - {printMoney}");
                    Dictionary<string, object> req = new Dictionary<string, object>
                    {
                        { "order_id",printOrderId},
                        { "money",printMoney}
                    };
                    NetManager.Instance.Post(RPCName.confirmPrintOrder, req,
                    (res) =>
                    {
                        string orderID = res["order_id"];
                        remainPrinterOrders.Remove(orderID);
                        MachineSetString("Server_RemainPrinterOrders", remainPrinterOrders.ToString());
                    },
                    (error) =>
                    {
                        SimpleJSON.JSONNode res1 = SimpleJSON.JSONNode.Parse(error.response);
                        string order = res1.HasKey("param") && res1["param"].HasKey("order_id") ? res1["param"]["order_id"] : null;
                        if (order != null)
                        {
                            remainPrinterOrders.Remove(order);
                            MachineSetString("Server_RemainPrinterOrders", remainPrinterOrders.ToString());
                        }
                    });
                }
            }

            if (isChange)
            {
                isChange = false;
                MachineSetString("Server_RemainPrinterOrders", remainPrinterOrders.ToString());
            }

            isFirst = false;
            yield return new WaitForSeconds(10); // 延时读算法卡
        }
    }


    Coroutine _CorSendRemain = null;
    [Button]
    void SendRemain()
    {
        if (_CorSendRemain != null) { 
            StopCoroutine(_CorSendRemain);
            _CorSendRemain = null;
        }
        initStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        _CorSendRemain = StartCoroutine(_SendRemain(initStamp));
    }

    void OnNetManageEvent(ParadoxNotion.EventData eventData)
    {
        if (eventData.name == "isChecked")
        {
            SendRemain();
        }
    }

    [Button]
    void test_ClearRemain()
    {


        MachineSetString("Server_RemainPrinterOrders", "{}");
        remainPrinterOrders = null;

        MachineSetString("Server_RemainAddMoneyOrders", "{}");
        remainAddMoneyOrders = null;

        MachineSetString("Server_RemainCoinOutOrders", "{}");
        remainCoinOutOrders = null;

        MachineSetString("Server_RemainCoinInOrders","{}");
        remainCoinInOrders = null;

        MachineSetString("Server_RemainCoinInNum", DEFINE_COIN_IN_NUM);
        remainCoinInNumLst = null;

    }


    [Button]
    void test_ShowRemain()
    {
        /* */
        Debug.Log($"RemainPrinterOrders = {MachineGetString("Server__RemainPrinterOrders", "{}")}");

        Debug.Log($"RemainAddMoneyOrders = {MachineGetString("Server__RemainAddMoneyOrders", "{}")}");

        Debug.Log($"RemainCoinOutOrders = {MachineGetString("Server__RemainCoinOutOrders", "{}")}");

        Debug.Log($"RemainCoinInOrders = {MachineGetString("Server__RemainCoinInOrders", "{}")}");

        Debug.Log($"RemainCoinInNumLst = {MachineGetString("Server__RemainCoinInNum", DEFINE_COIN_IN_NUM)}");
       

        /*
        Debug.Log($"RemainPrinterOrders = {remainPrinterOrders.ToString()}");

        Debug.Log($"RemainAddMoneyOrders = {remainAddMoneyOrders.ToString()}");

        Debug.Log($"RemainCoinOutOrders = {remainCoinOutOrders.ToString()}");

        Debug.Log($"RemainCoinInOrders = {remainCoinInOrders.ToString()}");

        Debug.Log($"RemainCoinInNumLst = {remainCoinInNumLst.ToString()}");
        */
    }


}
